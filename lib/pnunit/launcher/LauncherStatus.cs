using System.Collections.Generic;

namespace PNUnit.Launcher
{
    internal class LauncherStatus
    {
        internal int TestCount;
        internal int TestToExecuteCount;
        internal int CurrentTest;
        internal int ExecutedTests;
        internal string CurrentTestName;
        internal string WarningMsg;

        internal LauncherStatus(int testCount, int testToExecuteCount)
        {
            TestCount = testCount;
            TestToExecuteCount = testToExecuteCount;
        }

        internal void SetCurrentTestName(string testName)
        {
            CurrentTestName = testName;
        }

        internal void SetWarningMessage(string warningMessage)
        {
            WarningMsg = warningMessage;
        }

        internal List<string> GetRepeatedTests()
        {
            lock (this)
            {
                return new List<string>(mRepeatedTests);
            }
        }

        internal List<string> GetFailedTests()
        {
            lock (this)
            {
                return new List<string>(mFailedTests);
            }
        }

        internal List<string> GetIgnoredTests()
        {
            lock (this)
            {
                return new List<string>(mIgnoredTests);
            }
        }

        internal void Increment()
        {
            ++CurrentTest;
        }

        internal void IncrementExecuted()
        {
            ++ExecutedTests;
        }

        internal void AddFailed(string failed)
        {
            lock (this)
            {
                if (mFailedTests.Contains(failed))
                    return;

                mFailedTests.Add(failed);
            }
        }

        internal void AddIgnored(string ignored)
        {
            lock (this)
            {
                if (mIgnoredTests.Contains(ignored))
                    return;

                mIgnoredTests.Add(ignored);
            }
        }

        internal void AddRepeated(string repeated)
        {
            lock (this)
            {
                if (mRepeatedTests.Contains(repeated))
                    return;

                mRepeatedTests.Add(repeated);
            }
        }

        internal void UpdateProgress(
            int runTestsCount, int totalTestCount,
            List<string> failed, List<string> ignored)
        {
            lock (this)
            {
                CurrentTest = runTestsCount;
                ExecutedTests = runTestsCount;
                TestCount = totalTestCount;
                TestToExecuteCount = totalTestCount;
                mFailedTests = failed;
                mIgnoredTests = ignored;
            }
        }

        List<string> mRepeatedTests = new List<string>();
        List<string> mFailedTests = new List<string>();
        List<string> mIgnoredTests = new List<string>();
    }
}