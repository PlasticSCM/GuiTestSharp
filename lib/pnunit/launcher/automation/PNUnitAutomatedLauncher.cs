using System;
using System.Collections;
using System.Collections.Generic;

using log4net;

namespace PNUnit.Launcher.Automation
{
    internal class PNUnitAutomatedLauncher
    {
        public PNUnitAutomatedLauncher(string listenAddress)
        {
            mListenAddress = listenAddress;
        }

        internal string RunTest(
            string testFile,
            List<string> testsToRun,
            string resultLogFile,
            string errorLogFile,
            string failedConfFile,
            int numRetriesOnFailure,
            TestSuiteLoggerParams loggerParams,
            TestRange testRange,
            string[] cliArgs,
            int testTimeout)
        {
            mLoggerParams = loggerParams;

            try
            {
                mGroup = TestConfLoader.LoadFromFile(testFile, cliArgs);

                if ((mGroup == null) || (mGroup.ParallelTests.Count == 0))
                {
                    mLog.WarnFormat("[{0}] no tests to run", testFile);
                    return "No tests to run";
                }

                mTestsList = testsToRun;

                if (testRange == null)
                {
                    testRange = new TestRange(0, mGroup.ParallelTests.Count - 1);
                }

                if (testRange.EndTest >= mGroup.ParallelTests.Count)
                {
                    testRange.EndTest = mGroup.ParallelTests.Count - 1;
                }

                mUserValues = CliArgsReader.GetUserValues(cliArgs);

                mLauncherArgs = new LauncherArgs();

                mLauncherArgs.ResultLogFile = resultLogFile;
                mLauncherArgs.ErrorLogFile = errorLogFile;
                mLauncherArgs.FailedConfigFile = failedConfFile;
                mLauncherArgs.RetryOnFailure = numRetriesOnFailure;
                mLauncherArgs.MaxRetry = numRetriesOnFailure;
                mLauncherArgs.TestRange = testRange;
                mLauncherArgs.TestsTimeout = testTimeout;

                System.Threading.ThreadPool.QueueUserWorkItem(
                    new System.Threading.WaitCallback(Run));

                return string.Format(
                    "Trying to launch {0} tests",
                    mGroup.ParallelTests.Count);
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("RunTest error {0}", e.Message);
                mLog.Debug(e.StackTrace);
                return e.Message;
            }
        }

        internal AutomatedLauncherStatus GetStatus()
        {
            mLog.DebugFormat("GetStatus invoked");

            if (mLauncher == null)
            {
                return new AutomatedLauncherStatus();
            }

            AutomatedLauncherStatus result = new AutomatedLauncherStatus();

            LauncherStatus launcherStatus = mLauncher.GetStatus();

            if (launcherStatus == null)
            {
                return new AutomatedLauncherStatus();
            }

            result.TestCount = launcherStatus.TestCount;
            result.TestToExecuteCount = launcherStatus.TestToExecuteCount;
            result.CurrentTestName = launcherStatus.CurrentTestName;
            result.CurrentTest = launcherStatus.CurrentTest;
            result.ExecutedTests = launcherStatus.ExecutedTests;
            result.RepeatedTests = launcherStatus.GetRepeatedTests();
            result.FailedTests = launcherStatus.GetFailedTests();
            result.IgnoredTests = launcherStatus.GetIgnoredTests();
            result.WarningMsg = launcherStatus.WarningMsg;
            result.Finished = mbFinished;

            return result;
        }

        void Run(object o)
        {
            NUnitResultCollector nunitReport = new NUnitResultCollector();
            LogWriter logWriter = new LogWriter(mLauncherArgs.ResultLogFile, mLauncherArgs.ErrorLogFile);

            TestSuiteLogger testSuiteLogger = null;

            if (mLoggerParams != null && mLoggerParams.IsInitialized())
            {
                testSuiteLogger = new TestSuiteLogger(mLoggerParams);
                testSuiteLogger.SaveBuild();
                testSuiteLogger.CreateSuite();
            }

            mLauncher = new Launcher();

            mLauncher.RunTests(
                mGroup,
                mTestsList,
                mLauncherArgs.MaxRetry,
                mLauncherArgs.ShellMode,
                mLauncherArgs.RetryOnFailure,
                mLauncherArgs.FailedConfigFile,
                testSuiteLogger,
                mLauncherArgs.TestsTimeout,
                mLauncherArgs.TestRange,
                mUserValues,
                logWriter,
                mListenAddress,
                null);

            mbFinished = true;
        }

        Launcher mLauncher;
        TestGroup mGroup;
        LauncherArgs mLauncherArgs;
        TestSuiteLoggerParams mLoggerParams;
        Hashtable mUserValues;
        List<string> mTestsList;
        bool mbFinished = false;

        static readonly ILog mLog = LogManager.GetLogger("AutomatedLauncher");
        private string mListenAddress;
    }
}