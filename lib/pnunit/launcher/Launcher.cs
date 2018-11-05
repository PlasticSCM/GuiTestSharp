using System;
using System.Collections;
using System.Collections.Generic;

using log4net;

using NUnit.Core;

using PNUnit.Framework;
using PNUnit.Launcher.FileReport;

namespace PNUnit.Launcher
{
    internal class Launcher
    {
        internal Runner[] RunTests(
            TestGroup group,
            List<string> testsList,
            int maxRetry,
            bool bShellMode,
            int retryOnFailure,
            string failedfile,
            TestSuiteLogger testSuiteLogger,
            int testsTimeout,
            TestRange testRange,
            Hashtable userValues,
            LogWriter logWriter,
            string listenAddress,
            string reportFile)
        {
            if (testRange == null)
            {
                mLog.Warn("No tests are selected to run. Exiting.");
                return new Runner[0];
            }

            int testCount = testRange.EndTest - testRange.StartTest + 1;
            int testToExecuteCount = (testsList != null) ? testsList.Count : testCount;

            mStatus = new LauncherStatus(testCount, testToExecuteCount);

            Runner[] runners = new Runner[testCount];
            List<ParallelTest> failedGroups = new List<ParallelTest>();

            for (int currentTest = testRange.StartTest; currentTest <= testRange.EndTest; )
            {
                int ini = Environment.TickCount;

                ParallelTest test = group.ParallelTests[currentTest] as ParallelTest;

                if (!IsSelectedTest(test, testsList))
                {
                    mLog.ErrorFormat(
                        "Test with name [{0}] is not invited to this party.",
                        test.Name);

                    ++currentTest;
                    mStatus.Increment();
                    continue;
                }

                mStatus.SetCurrentTestName(test.Name);

                int retryCount = 0;

                bool bRetry = true;

                while (bRetry && retryCount < maxRetry)
                {
                    bRetry = false;

                    LogTestProgress(group, testRange, testCount, currentTest);

                    Runner runner = new Runner(
                        test, userValues, testsTimeout, logWriter, mStatus, listenAddress);

                    if (bShellMode)
                        runner.ShellMode = bShellMode;

                    runners[currentTest - testRange.StartTest] = runner;

                    if (reportFile != null)
                        StatusReport.Write(reportFile, mStatus, false);

                    runner.Run();

                    TestResult[] runnerResults = runner.GetTestResults();

                    if (runnerResults == null)
                    {
                        mLog.InfoFormat("Error. Results for test [{0}] are NULL",
                            test.Name);

                        ++currentTest;
                        mStatus.Increment();
                        mStatus.IncrementExecuted();
                        continue;
                    }

                    bool isRepeated = retryCount > 0;

                    if (reportFile == null)
                        LogTestResultsToTTS(
                            testSuiteLogger, runnerResults, test.Name, isRepeated);
                    else
                        LogTestResultsToFile(
                            reportFile, runnerResults, test.Name, isRepeated, true);

                    bRetry = RetryTest(runnerResults);
                    bool bFailed = FailedTest(runnerResults);

                    if (bRetry ||
                        ((bFailed && (retryOnFailure > 0) &&
                         ((retryCount + 1) < maxRetry)) /* so that list time is printed*/))
                    {
                        bRetry = true;
                        ++retryCount;
                        mLog.Info("Test failed with retry option, trying again");

                        mStatus.AddRepeated(test.Name);

                        continue;
                    }

                    if (bFailed)
                    {
                        failedGroups.Add(test);
                        WriteGroup(failedGroups, failedfile);
                        mStatus.AddFailed(test.Name);
                    }

                    if (IgnoredTest(runnerResults))
                        mStatus.AddIgnored(test.Name);
                }

                // updated at the bottom so it's not affected by retries
                mStatus.Increment();
                mStatus.IncrementExecuted();
                ++currentTest;

                mLog.DebugFormat("Test {0} time {1} ms",
                    test.Name, Environment.TickCount - ini);
            }

            if (reportFile != null)
                StatusReport.Write(reportFile, mStatus, true);

            return runners;
        }

        internal LauncherStatus GetStatus()
        {
            return mStatus;
        }

        void LogTestProgress(
            TestGroup group,
            TestRange testRange,
            int testCount,
            int i)
        {
            if (testCount != group.ParallelTests.Count)
                mLog.InfoFormat("Test {0} of {1}. {2}/{3}",
                    i, group.ParallelTests.Count, i - testRange.StartTest + 1,
                    testCount);
            else
                mLog.InfoFormat("Test {0} of {1}", i + 1,
                    group.ParallelTests.Count);
        }

        void LogTestResultsToTTS(
            TestSuiteLogger testSuiteLogger,
            TestResult[] runnerResults,
            string testName,
            bool isRepeated)
        {
            if (testSuiteLogger == null)
                return;

            int ini = Environment.TickCount;

            testSuiteLogger.LogTestRunResults(
                runnerResults,
                testSuiteLogger.SuiteParams.SuiteType,
                testName,
                isRepeated);

            mLog.DebugFormat("Time logging to TTS {0} ms", Environment.TickCount - ini);
        }

        static void LogTestResultsToFile(
            string outputFile,
            TestResult[] runnerResults,
            string testName,
            bool bIsRepeated,
            bool bLogSuccessful)
        {
            TestLogReport.LogTest(
                outputFile + FileReportConstants.LOG_FILE_EXTENSION,
                TestSuiteEntry.Calculate(runnerResults, bLogSuccessful),
                testName,
                bIsRepeated);
        }

        bool FailedTest(TestResult[] results)
        {
            if (results == null || results.Length < 1)
                return true;

            foreach (TestResult res in results)
            {
                if (res == null || !res.Executed)
                    continue;
                if (!res.IsSuccess)
                    return true;
            }
            return false;
        }

        bool IgnoredTest(TestResult[] results)
        {
            foreach (TestResult res in results)
            {
                if (res == null)
                    continue;

                if (!res.Executed)
                    return true;
            }

            return false;
        }

        bool RetryTest(TestResult[] results)
        {
            foreach (TestResult res in results)
            {
                if (res == null)
                    continue;
                if (res is PNUnitTestResult)
                {
                    return ((PNUnitTestResult)res).RetryTest;
                }
            }
            return false;
        }

        void WriteGroup(List<ParallelTest> failedTests, string filename)
        {
            TestGroup group = new TestGroup();
            group.ParallelTests = failedTests;
            TestConfLoader.WriteToFile(group, filename);
        }

        bool IsSelectedTest(ParallelTest test, List<string> testsList)
        {
            if (testsList == null)
                return true;

            return testsList.Contains(test.Name);
        }

        LauncherStatus mStatus;

        static readonly ILog mLog = LogManager.GetLogger("launcher");
    }
}
