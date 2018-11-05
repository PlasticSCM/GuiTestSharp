using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PNUnit.Launcher.Automation
{
    public interface ILauncher
    {
        string RunTest(
            string testFile,
            List<string> testsToRun,
            string resultLogFile,
            string errorLogFile,
            string failedConfFile,
            int numRetriesOnFailure,
            TestSuiteLoggerParams loggerParams,
            TestRange testRange,
            List<string> cliArgs,
            int testTimeout);

        AutomatedLauncherStatus GetStatus();

        void Exit();
    }

    [Serializable]
    public class AutomatedLauncherStatus
    {
        public int TestCount;
        public int TestToExecuteCount;
        public int CurrentTest;
        public int ExecutedTests;
        public string CurrentTestName = string.Empty;
        public List<string> RepeatedTests = new List<string>();
        public List<string> FailedTests = new List<string>();
        public List<string> IgnoredTests = new List<string>();
        public bool Finished = false;
        public string WarningMsg = string.Empty;
    }
}
