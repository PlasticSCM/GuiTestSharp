using System;
using System.Collections;
using System.Collections.Generic;

using log4net;

namespace PNUnit.Launcher.Automation
{
    class LauncherService: MarshalByRefObject, ILauncher
    {
        public LauncherService(string ipToBind, int port)
        {
            mListenAddress = string.Format("{0}:{1}", ipToBind, port);
        }

        // live forever
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string RunTest(
            string testFile,
            List<string> testsToRun,
            string resultLogFile,
            string errorLogFile,
            string failedConfFile,
            int numRetriesOnFailure,
            TestSuiteLoggerParams loggerParams,
            TestRange testRange,
            List<string> cliArgs,
            int testTimeout)
        {
            mLog.InfoFormat("Request to run tests on file [{0}]", testFile);

            if (mAutomatedLauncher != null)
            {
                string msg = "Can't launch another test suite because a test suite is currently running";
                mLog.Error(msg);
                return msg;
            }

            mAutomatedLauncher = new PNUnitAutomatedLauncher(mListenAddress);

            return mAutomatedLauncher.RunTest(
                testFile,
                testsToRun,
                resultLogFile,
                errorLogFile,
                failedConfFile,
                numRetriesOnFailure,
                loggerParams,
                testRange,
                cliArgs == null ? null : cliArgs.ToArray(),
                testTimeout);
        }

        public AutomatedLauncherStatus GetStatus()
        {
            mLog.DebugFormat("GetStatus invoked");

            if (mAutomatedLauncher == null)
            {
                return new AutomatedLauncherStatus();
            }

            return mAutomatedLauncher.GetStatus();
        }

        public void Exit()
        {
            CanExit = true;
        }

        public bool CanExit = false;

        string mListenAddress;

        PNUnitAutomatedLauncher mAutomatedLauncher = null;

        static readonly ILog mLog = LogManager.GetLogger("AutomatedLauncher");
    }
}
