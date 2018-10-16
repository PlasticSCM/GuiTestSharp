using System;
using System.Collections;
using System.Collections.Generic;

using log4net;

using NUnit.Core;

using PNUnit.Framework;

namespace PNUnit.Launcher
{
    public class Runner
    {
        internal Runner(
            ParallelTest test,
            Hashtable userValues,
            int testTimeout,
            LogWriter logWriter,
            LauncherStatus launcherStatus,
            string listenAddress)
        {
            mTestGroup = test;
            mUserValues = userValues;
            mTestTimeout = testTimeout;
            mLogWriter = logWriter;
            mStatus = launcherStatus;
            mListenAddress = listenAddress;

            mTestsRun = new TestsRun();
        }

        public string TestGroupName
        {
            get{ return mTestGroup.Name; }
        }

        public bool ShellMode
        {
            get { return mbShellMode; }
            set { mbShellMode = value; }
        }

        public void Run()
        {
            if( mTestGroup.Tests.Length == 0 )
            {
                mLog.Fatal("No tests to run, exiting");
                return;
            }

            mLog.DebugFormat(
                "Run TestGroup {0} with {1} tests",
                mTestGroup.Name, mTestGroup.Tests.Length);

            PNUnitService service = new PNUnitService(
                mTestGroup, mLogWriter, mTestsRun, mStatus);

            service.Publish();

            List<SendTestToAgentParams> testsGroupToRun =
                new List<SendTestToAgentParams>(mTestGroup.Tests.Length);

            foreach (TestConf test in mTestGroup.Tests)
            {
                if (test.Machine.StartsWith(AGENT_KEY))
                    test.Machine = mTestGroup.Agents[int.Parse(test.Machine.Substring(AGENT_KEY.Length))];

                mLogWriter.Log(string.Format("Starting {0} test {1} on {2}",
                    mTestGroup.Name, test.Name, test.Machine));

                SendTestToAgentParams testParams = new SendTestToAgentParams();

                testParams.TestGroupName = mTestGroup.Name;
                testParams.Agent = GetAgent(test);
                testParams.TestConf = test;

                testParams.TestInfo = new PNUnitTestInfo(
                    test.Name, test.Assembly,
                    test.TestToRun,
                    test.TestParams,
                    test.StartBarrier,
                    test.EndBarrier,
                    test.WaitBarriers,
                    mListenAddress);

                testParams.TestInfo.UserValues = mUserValues;

                testsGroupToRun.Add(testParams);
            }

            LaunchTestOnAgents(testsGroupToRun, service);

            if (mbShellMode)
            {
                CommandLoop();
            }

            WaitForTestToFinish(service);

            service.Unpublish();

            mLog.DebugFormat("Finish for TestGroup {0}", mTestGroup.Name);
        }

        internal static TestConf GetTestConfFromName(
            ParallelTest testGroup, string testName)
        {
            foreach (TestConf testConf in testGroup.Tests)
                if (testConf.Name == testName)
                    return testConf;

            return null;
        }

        internal TestResult[] GetTestResults()
        {
            return mTestsRun.GetTestResults();
        }

        void WaitForTestToFinish(PNUnitService service)
        {
            if (!HasToWait())
                return;

            mLog.DebugFormat(
                "Thread going to wait for results for TestGroup {0}",
                mTestGroup.Name);

            int iniWait = Environment.TickCount;

            bool bAliveLastTime = true;

            // wait for all tests to end
            while (!service.WaitToFinish(10000))
            {
                // wake up every 10 seconds and check if at least
                // the tests called back :-)

                if (HasBarriers() && !LaunchedTestsCalledBack(service, iniWait))
                {
                    if (!TestsAreAlive())
                        return;
                }

                if (Environment.TickCount - iniWait > 20000)
                {
                    if (TestsAreAlive())
                    {
                        bAliveLastTime = true;
                    }
                    else
                    {
                        if (!bAliveLastTime)
                        {
                            mLog.ErrorFormat(
                                "Tests in {0} are not alive" +
                                " and they were not alive in the last check" +
                                " so this test test will be finished",
                                mTestGroup.Name);
                            return;
                        }

                        bAliveLastTime = false;
                    }
                }
            }
        }

        bool TestsAreAlive()
        {
            foreach (TestConf test in mTestGroup.Tests)
            {
                if (test.Machine.StartsWith(AGENT_KEY))
                    test.Machine = mTestGroup.Agents[int.Parse(test.Machine.Substring(AGENT_KEY.Length))];

                IPNUnitAgent agent = GetAgent(test);

                try
                {
                    if (!agent.IsTestRunning(test.Name))
                    {
                        mLog.WarnFormat("Test {0} is not running on {1}", test.Name, test.Machine);
                        return false;
                    }
                }
                catch (Exception e)
                {
                    mLog.ErrorFormat("Error trying to check if test {0} is running on {1}. {2}",
                        test.Name, test.Machine, e.Message);
                    mLog.Debug(e.StackTrace);

                    return false;
                }
            }

            return true;
        }

        bool HasBarriers()
        {
            foreach (TestConf test in mTestGroup.Tests)
            {
                if (!string.IsNullOrEmpty(test.EndBarrier) ||
                    !string.IsNullOrEmpty(test.StartBarrier) ||
                    (test.WaitBarriers != null && test.WaitBarriers.Length > 0))
                    return true;
            }

            return false;
        }

        bool LaunchedTestsCalledBack(PNUnitService service, int iniWait)
        {
            List<string> testsThatContacted = service.GetTestsThatContacted();

            if (testsThatContacted.Count == mTestGroup.Tests.Length)
            {
                mStatus.SetWarningMessage(string.Empty);
                return true;
            }

            string warnMsg = string.Format(
                "Something weird is going on, it's been {0} ms since" +
                " the wait started and not all tests contacted back.",
                Environment.TickCount - iniWait);

            if (testsThatContacted.Count > 0)
            {
                warnMsg += " Tests that contacted: ";

                foreach (string contactedTest in testsThatContacted)
                    warnMsg += " " + contactedTest;
            }

            mStatus.SetWarningMessage(warnMsg);

            mLog.Warn(warnMsg);

            return false;
        }

        void LaunchTestOnAgents(List<SendTestToAgentParams> testsToRun, PNUnitService service)
        {
            foreach (SendTestToAgentParams testToRun in testsToRun)
                CleanPreviousRunningTestsOnAgent(testToRun);

            foreach (SendTestToAgentParams testToRun in testsToRun)
                LaunchTestOnAgent(testToRun, service);
        }

        void CleanPreviousRunningTestsOnAgent(SendTestToAgentParams testToRun)
        {
            string testConfName = string.Empty;
            try
            {
                testConfName = testToRun.TestConf.Name;
                testToRun.Agent.IsUp();
                testToRun.Agent.CleanPreviousRunningTests();
            }
            catch (Exception e)
            {
                mLogWriter.LogWarn(string.Format(
                    "An error occurred trying to clean previous " +
                    "running tests on agent before running tests [{0}]. " +
                    "Message: [{1}]", testConfName, e.Message));
            }
        }

        void LaunchTestOnAgent(SendTestToAgentParams testToRun, PNUnitService service)
        {
            try
            {
                mTestsRun.IncrementTestsLaunched();

                testToRun.Agent.IsUp();

                testToRun.Agent.SetTestsTimeout(mTestTimeout);

                testToRun.Agent.RunTest(testToRun.TestInfo);
            }
            catch (Exception e)
            {
                mLogWriter.LogError(string.Format(
                    "Test: [{0}] - An error occurred trying to contact {1} [{2}]",
                    testToRun.TestGroupName + "-" + testToRun.TestConf.Name,
                    testToRun.TestConf.Machine, e.Message));

                mTestsRun.DecrementTestsLaunched();

                NotifyException(service, testToRun.TestConf, e);
            }
        }

        void NotifyException(
            PNUnitService service, TestConf test, Exception e)
        {
            TestName tn = new TestName();
            tn.Name = test.Name;

            string fullMessage = e.Message + "; STACK TRACE: " + e.StackTrace;
            PNUnitTestResult tr = new PNUnitTestResult(tn, fullMessage,
                string.Empty, string.Empty);
            tr.Failure(fullMessage, e.StackTrace);

            service.NotifyResult(test.Name, tr);
        }

        void CommandLoop()
        {
            string code;
            do
            {
                code = Console.ReadLine().Trim();

                if (code == "launchedtests")
                {
                    Console.WriteLine("Launched tests: " + mTestsRun.LaunchedCount);
                }
                else if (code == "pendingtests")
                {
                    int numTests = 0;
                    foreach (TestConf test in mTestGroup.Tests)
                    {
                        if (!mTestsRun.IsTestExecuted(test.Name))
                        {
                            string machine = GetTestConfFromName(mTestGroup, test.Name).Machine;

                            Console.WriteLine(
                                "Test {0} has not finished yet. Agent: {1}",
                                test.Name, machine);

                            numTests++;
                        }
                    }
                    Console.WriteLine("Pending tests: " + numTests);
                }
            } while (code != "exit");
        }

        bool HasToWait()
        {
            return mTestsRun.PendingTestsToFinish();
        }

        static IPNUnitAgent GetAgent(TestConf test)
        {
            IPNUnitAgent agent = (IPNUnitAgent)Activator.GetObject(
                typeof(IPNUnitAgent),
                string.Format("tcp://{0}/{1}",
                    test.Machine,
                    Names.PNUnitAgentServiceName));
            return agent;
        }

        TestsRun mTestsRun;

        static readonly ILog mLog = LogManager.GetLogger("launcher");
        const string AGENT_KEY = "_AGENT";

        int mInitialTime = Environment.TickCount;

        ParallelTest mTestGroup;

        Hashtable mUserValues;
        bool mbShellMode = false;
        int mTestTimeout = 0;
        LogWriter mLogWriter;
        string mListenAddress;

        LauncherStatus mStatus;

        class SendTestToAgentParams
        {
            internal string TestGroupName;
            internal IPNUnitAgent Agent;
            internal TestConf TestConf;
            internal PNUnitTestInfo TestInfo;
        }
    }
}
