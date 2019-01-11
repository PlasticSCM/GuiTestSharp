using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using log4net;

using NUnit.Core;

using PNUnit.Framework;

namespace PNUnit.Agent
{
    public class PNUnitAgent : MarshalByRefObject, IPNUnitAgent
    {
        public void RunTest(PNUnitTestInfo testInfo)
        {
            try
            {
                int ini = Environment.TickCount;

                mLog.InfoFormat(
                    ">>RunTest called for Test {0}, AssemblyName {1}, TestToRun {2}",
                    testInfo.TestName, testInfo.AssemblyName, testInfo.TestToRun);

                mTestCounter.Increment();

                CreateTestOutputFile(testInfo);

                // spawn the test in a new process
                string testInfoPath = WriteTestInfo(testInfo);

                mProcessPool.RunProcess(testInfo, testInfoPath);

                mLog.InfoFormat(
                    "<<RunTest {0} - {1} launched in {2} ms.",
                    testInfo.TestName, testInfo.TestToRun, Environment.TickCount - ini);
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("<<RunTest {0} - {1} failed {2}",
                    testInfo.TestName, testInfo.TestToRun, e.Message);

                NotifyError(e, testInfo);
            }
        }

        public bool IsTestRunning(string testName)
        {
            try
            {
                return mProcessPool.IsTestRunning(testName);
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("<<IsTestRunning [{0}] Failed: {1}",
                    testName, e.Message);

                return false;
            }

        }

        public void SetTestsTimeout(int value)
        {
            if (mProcessPool == null)
                return;

            if (value > 0)
                mProcessPool.SetTimeout(value);
        }

        public bool IsUp()
        {
            return true;
        }

        public void CleanPreviousRunningTests()
        {
            if (mProcessPool == null)
                return;

            mProcessPool.CleanPreviousRunningTests();
        }

        public override object InitializeLifetimeService()
        {
            // Lives forever
            return null;
        }

        internal void Run(
            AgentConfig config, bool bDaemonMode, string preloadTestRunners)
        {
            try
            {
                DoRun(config, bDaemonMode, preloadTestRunners);
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("PNUnit agent crashed!!! :{0} - {1}", e.Message, e.StackTrace);
            }
        }

        void DoRun(
            AgentConfig config, bool bDaemonMode, string preloadTestRunners)
        {
            mConfig = config;

            ConfigureRemoting.Configure(mConfig.Port);

            mProcessPool = new ProcessPool(
                preloadTestRunners,
                Path.GetFullPath(mConfig.PathToAssemblies),
                mConfig.NoTimeout);

            // publish
            RemotingServices.Marshal(this, PNUnit.Framework.Names.PNUnitAgentServiceName);

            // otherwise in .NET 2.0 memory grows continuosly
            FreeMemory();

            mProcessPool.Start();

            if( bDaemonMode )
            {
                // wait continously
                while (true)
                {
                    Thread.Sleep(10000);
                }
            }

            Shell shell = new Shell();

            shell.Run(mTestCounter);
        }

        string WriteTestInfo(PNUnitTestInfo info)
        {
            string infoConfigPath = Path.GetTempFileName();

            using (FileStream fs = new FileStream(infoConfigPath, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter(new RemotingSurrogateSelector(),
                    new StreamingContext(StreamingContextStates.CrossAppDomain));
                bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                bf.Serialize(fs, info);
            }

            return infoConfigPath;
        }

        internal static void NotifyError(Exception e, PNUnitTestInfo info)
        {
            TestLogInfo testLogInfo = new TestLogInfo();
            testLogInfo.SetOSVersion(Environment.OSVersion.Platform.ToString());

            TestName testName = new TestName();
            testName.Name = info.TestName;
            testName.FullName = info.TestName;
            testName.TestID = new TestID();

            PNUnitTestResult result = new PNUnitTestResult(
                testName, info.GetTestOutput(),
                testLogInfo.OSVersion, testLogInfo.BackendType, true);

            string fullMessage = string.Format(
                "TestName: {0}; Error: {1}; EXCEPTION TYPE: {2}; STACK TRACE: {3}",
                testName.Name, e.Message, e.GetType(), e.StackTrace);

            result.Failure(fullMessage, string.Empty);

            info.DeleteTestOutput();

            IPNUnitServices services =
                PNUnitServices.GetPNunitServicesProxy(info.PNUnitServicesServer);

            services.NotifyResult(info.TestName, result);
        }

        void FreeMemory()
        {
            GC.GetTotalMemory(true);
        }

        void CreateTestOutputFile(PNUnitTestInfo info)
        {
            string outputTestFilePath = Path.GetTempFileName();

            using (StreamWriter sw = new StreamWriter(outputTestFilePath, false))
            {
                sw.WriteLine("");
            }

            info.OutputFile = outputTestFilePath;
        }

        internal class TestCounter
        {
            internal void Increment()
            {
                Interlocked.Increment(ref mTestCount);
            }

            internal int Get()
            {
                return mTestCount;
            }

            int mTestCount = 0;
        }

        TestCounter mTestCounter = new TestCounter();
        ProcessPool mProcessPool;
        AgentConfig mConfig;

        static readonly ILog mLog = LogManager.GetLogger("PNUnitAgent");
    }
}