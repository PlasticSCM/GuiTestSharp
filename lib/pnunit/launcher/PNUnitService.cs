using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using System.Runtime.Remoting;

using log4net;

using NUnit.Core;

using PNUnit.Framework;

namespace PNUnit.Launcher
{
    internal class PNUnitService : MarshalByRefObject, IPNUnitServices
    {
        internal PNUnitService(
            ParallelTest testGroup,
            LogWriter logWriter,
            TestsRun testsRun,
            LauncherStatus launcherStatus)
        {
            mTestGroup = testGroup;
            mLogWriter = logWriter;
            mTestsRun = testsRun;

            mbBarriersInitialized = false;

            mLauncherStatus = launcherStatus;

            mFinish = new ManualResetEvent(false);
        }

        internal void Publish()
        {
            RemotingServices.Marshal(this, PNUnitServices.PNUNIT_SERVICES_NAME);
        }

        internal void Unpublish()
        {
            RemotingServices.Disconnect(this);
        }

        internal bool WaitToFinish(int millisecondsTimeout)
        {
            return mFinish.WaitOne(millisecondsTimeout);
        }

        internal List<string> GetTestsThatContacted()
        {
            lock (this)
            {
                return new List<string>(mTestThatContactedBack);
            }
        }

        #region MarshallByRefObject
        // Lives forever
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion

        #region IPNUnitServices

        public void NotifyResult(string testName, PNUnitTestResult result)
        {
            mLog.DebugFormat("NotifyResult called for TestGroup {0}, Test {1}",
                mTestGroup.Name, testName);

            int count = 0;

            mLog.DebugFormat(
                "NotifyResult lock entered for TestGroup {0}, Test {1}",
                mTestGroup.Name, testName);

            mTestsRun.AddExecutedTest(testName);
            mTestsRun.AddTestResult(result);

            count = mTestsRun.TestsResultsCount;

            lock (mBarriers)
            {
                if (mBarriersOfTests.ContainsKey(testName))
                {
                    mLog.DebugFormat("Going to abandon barriers of test {0}",
                        testName);
                    IList list = (IList)mBarriersOfTests[testName];
                    foreach (string barrier in list)
                    {
                        mLog.DebugFormat("Abandoning barrier {0}", barrier);
                        mBarriers[barrier].Abandon();
                    }
                }
            }

            mLog.DebugFormat(
                "NotifyResult finishing for TestGroup {0}, Test {1}.",
                mTestGroup.Name, testName);

            string machine = Runner.GetTestConfFromName(mTestGroup, testName).Machine;

            string resultText = result.IsSuccess ? "PASS" : "FAIL";

            if (!result.Executed)
            {
                resultText = "IGNORED";
            }

            string message = string.Format(
                "Result for TestGroup {0}, Test {1}: {2}. Time {3} ms. {4}/{5} tests finished. Agent: {6}",
                mTestGroup.Name,
                testName,
                resultText,
                Environment.TickCount - mInitialTime,
                count,
                mTestsRun.LaunchedCount,
                machine);

            string logTestName = string.Format("{0}.{1}", mTestGroup.Name, testName);

            if (!result.Executed)
            {
                mLogWriter.LogWarn(message);
                mLogWriter.WriteTestLog(logTestName, result, machine);
                return;
            }

            if (result.IsSuccess)
            {
                mLogWriter.Log(message);
                mLogWriter.WriteTestLog(logTestName, result, machine);
            }
            else
            {
                mLogWriter.LogError(message);
                mLogWriter.WriteFailedTestLog(logTestName, result, machine);
            }

            if (mTestsRun.AllTestsFinished())
            {
                mLog.DebugFormat(
                    "All the tests notified the results, waking up. mResults.Count == {0}",
                    mTestsRun.TestsResultsCount);
                mFinish.Set();
            }
        }

        object mInitBarriersSync = new object();
        bool mbBarriersInitialized = false;
        public void InitBarriers(string testName)
        {
            AddTestThatContacted(testName);
            mLog.DebugFormat("InitBarriers invoked by {0}", testName);
            lock (mInitBarriersSync)
            {
                if (mbBarriersInitialized)
                    return;

                Hashtable barriers = new Hashtable();
                for (int i = 0; i < mTestGroup.Tests.Length; i++)
                {
                    AddBarrier(mTestGroup.Tests[i].StartBarrier, barriers);
                    AddBarrier(mTestGroup.Tests[i].EndBarrier, barriers);
                    AddBarrier(mTestGroup.Tests[i].WaitBarriers, barriers);

                    InitTestBarriers(mTestGroup.Tests[i].Name, mTestGroup.Tests[i].StartBarrier);
                    InitTestBarriers(mTestGroup.Tests[i].Name, mTestGroup.Tests[i].EndBarrier);
                    InitTestBarriers(mTestGroup.Tests[i].Name, mTestGroup.Tests[i].WaitBarriers);
                }

                foreach (string key in barriers.Keys)
                {
                    DoInitBarrier(key, (int)barriers[key], false);
                }

                mbBarriersInitialized = true;
            }
        }

        public void InitBarrier(string testName, string barrier, int max)
        {
            mLog.DebugFormat("InitBarrier invoked by {0}", testName);
            DoInitBarrier(barrier, max, true);
        }

        public void EnterBarrier(string testName, string barrier)
        {
            mLog.DebugFormat(">Entering Barrier {0} - invoked by {1}", barrier, testName);
            mBarriers[barrier].Enter();
            mLog.DebugFormat("<Entered Barrier {0} - invoked by {1}", barrier, testName);
        }

        public void SendMessage(string tag, int receivers, object message)
        {
            mMessageQueue.Send(tag, receivers, message);
        }

        public object ReceiveMessage(string tag)
        {
            return mMessageQueue.Receive(tag);
        }

        public void ISendMessage(string tag, int receivers, object message)
        {
            mMessageQueue.ISend(tag, receivers, message);
        }

        public object IReceiveMessage(string tag)
        {
            return mMessageQueue.IReceive(tag);
        }

        public void NotifyNUnitProgress(
            int runTestsCount, int totalTestCount,
            List<string> failed, List<string> ignored, bool bFinished)
        {
            mLauncherStatus.UpdateProgress(
                runTestsCount, totalTestCount, failed, ignored);
        }

        #endregion

        void DoInitBarrier(string barrier, int Max, bool bOverwrite)
        {
            lock (mBarriers)
            {
                if (mBarriers.ContainsKey(barrier))
                {
                    if (bOverwrite)
                        mBarriers[barrier] = new Barrier(barrier, Max);
                    return;
                }

                mBarriers.Add(barrier, new Barrier(barrier, Max));
            }
        }

        void InitTestBarriers(string testName, string barrier)
        {
            if (mBarriersOfTests.ContainsKey(testName))
            {
                List<string> listofbarriers = mBarriersOfTests[testName];
                listofbarriers.Add(barrier);
                mLog.DebugFormat("Adding barrier {0} to {1}", barrier, testName);
            }
            else
            {
                List<string> list = new List<string>();
                list.Add(barrier);
                mLog.DebugFormat("Adding barrier {0} to {1}", barrier, testName);
                mBarriersOfTests.Add(testName, list);
            }
        }

        void InitTestBarriers(string testName, string[] barriers)
        {
            if (barriers != null && barriers.Length > 0)
            {
                foreach (string barrier in barriers)
                    InitTestBarriers(testName, barrier);
            }
        }

        void AddBarrier(string barrier, Hashtable barriers)
        {
            if (barrier != null && barrier.Trim() != string.Empty)
            {
                if (barriers.Contains(barrier))
                    barriers[barrier] = (int)barriers[barrier] + 1;
                else
                    barriers[barrier] = 1;
            }
        }

        void AddBarrier(string[] barrier, Hashtable barriers)
        {
            if (barrier != null && barrier.Length > 0)
            {
                foreach (string b in barrier)
                    AddBarrier(b, barriers);
            }
        }

        void AddTestThatContacted(string testName)
        {
            lock (this)
            {
                if (mTestThatContactedBack.Contains(testName))
                    return;

                mTestThatContactedBack.Add(testName);
            }
        }

        ParallelTest mTestGroup;

        Dictionary<string, Barrier> mBarriers = new Dictionary<string, Barrier>();

        MessageQueue mMessageQueue = new MessageQueue();

        Dictionary<string, List<string>> mBarriersOfTests = new Dictionary<string, List<string>>();
        TestsRun mTestsRun;

        LogWriter mLogWriter;

        ManualResetEvent mFinish;

        int mInitialTime = Environment.TickCount;

        List<string> mTestThatContactedBack = new List<string>();

        LauncherStatus mLauncherStatus;

        static readonly ILog mLog = LogManager.GetLogger("launcher");
    }
}
