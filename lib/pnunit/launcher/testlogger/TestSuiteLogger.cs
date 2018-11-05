using System;

using NUnit.Core;
using log4net;

using TestLogger;

namespace PNUnit.Launcher
{
    internal class TestSuiteLogger
    {
        internal TestSuiteLoggerParams SuiteParams { get { return mTestSuiteLoggerParams; } }

        internal TestSuiteLogger(TestSuiteLoggerParams testSuiteLoggerParams)
        {
            mTestSuiteLoggerParams = testSuiteLoggerParams;
        }

        internal void SaveBuild()
        {
            TestLoggerClient client = new TestLoggerClient();
            try
            {
                mBuildId = client.SaveBuild(
                    mTestSuiteLoggerParams.BuildType,
                    mTestSuiteLoggerParams.BuildName,
                    mTestSuiteLoggerParams.Cset.ToString(),
                    mTestSuiteLoggerParams.Comment);
            }
            catch (Exception e)
            {
                log.Error("ERROR LOGGING TEST STATS IN DATABASE: " + e.Message);
            }
        }

        internal void CreateSuite()
        {
            TestLoggerClient client = new TestLoggerClient();
            try
            {
                mSuiteRunId = client.SaveSuiteRun(
                    mBuildId,
                    mTestSuiteLoggerParams.SuiteType,
                    mTestSuiteLoggerParams.SuiteName,
                    mTestSuiteLoggerParams.Host,
                    mTestSuiteLoggerParams.VMachine);
            }
            catch (Exception e)
            {
                log.Error("ERROR LOGGING TEST STATS IN DATABASE: " + e.Message);
            }
        }

        internal void LogTestRunResults(
            TestResult[] testResults, string suiteType, string testName, bool isRepeated)
        {
            TestSuiteEntry.Data entry = TestSuiteEntry.Calculate(
                testResults, mTestSuiteLoggerParams.LogSuccessfulTests);

            TestLoggerClient client = new TestLoggerClient();
            try
            {
                client.SaveTestRun(
                    mBuildId,
                    mSuiteRunId,
                    suiteType,
                    testName,
                    entry.ClientConfig,
                    entry.ServerConfig,
                    entry.BackendType,
                    entry.ExecTime,
                    entry.Status,
                    entry.Log,
                    isRepeated);
            }
            catch (Exception e)
            {
                log.Error("ERROR LOGGING TEST STATS IN DATABASE: " + e.Message);
            }
        }

        readonly ILog log = LogManager.GetLogger("launcher");
        Guid mBuildId;

        TestSuiteLoggerParams mTestSuiteLoggerParams;
        Guid mSuiteRunId;
    }
}