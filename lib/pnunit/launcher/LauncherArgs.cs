namespace PNUnit.Launcher
{
    internal class LauncherArgs
    {
        internal string ConfigFile;
        internal string ResultLogFile;
        internal string ErrorLogFile;
        internal string ResultFile = null;
        internal string FailedConfigFile = null;
        internal int RetryOnFailure = 0;
        internal int MaxRetry = DEFAULT_TEST_RETRY;
        internal bool ShellMode = false;
        internal TestRange TestRange = null;
        internal int TestsTimeout = 0;
        internal string ListTestsFile;
        internal string UseFileReport;

        const int DEFAULT_TEST_RETRY = 3;
    }
}