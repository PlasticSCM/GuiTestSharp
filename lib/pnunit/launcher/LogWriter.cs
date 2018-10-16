using System;
using System.Collections;
using System.IO;

using NUnit.Core;

using PNUnit.Framework;

using log4net;

namespace PNUnit.Launcher
{
    class LogWriter
    {
        internal LogWriter(string smokeResultFile, string smokeErrorsFile)
        {
            mSmokeResultFile = smokeResultFile;
            mSmokeErrorsFile = smokeErrorsFile;
        }

        internal void WriteTestLog(
            string logTestName,
            TestResult result,
            string machine)
        {
            WriteTestLog(logTestName, result, machine, mSmokeResultFile);
        }

        internal void WriteFailedTestLog(
            string logTestName,
            TestResult result,
            string machine)
        {
            WriteTestLog(logTestName, result, machine, mSmokeErrorsFile);
        }

        internal void Log(string msg)
        {
            mLog.Info(msg);
            mTotalLog += string.Concat(msg, "\r\n");
        }

        internal void LogWarn(string msg)
        {
            mLog.Warn(msg);
            mTotalLog += string.Concat(msg, "\r\n");
        }

        internal void LogError(string msg)
        {
            mLog.Error(msg);
            mTotalLog += string.Concat(msg, "\r\n");
        }

        static internal void PrintResults(
            Runner[] runners,
            DateTime beginTimeStamp,
            DateTime endTimeStamp,
            LogWriter logWriter)
        {
            double TotalBiggerTime = 0;
            int TotalTests = 0;
            int TotalExecutedTests = 0;
            int TotalIgnoredTests = 0;
            int TotalFailedTests = 0;
            int TotalSuccessTests = 0;

            IList failedTests = new ArrayList();

            int j;
            foreach (Runner runner in runners)
            {
                if (runner == null) continue;

                int ExecutedTests = 0;
                int FailedTests = 0;
                int SuccessTests = 0;
                int IgnoredTests = 0;
                double BiggerTime = 0;
                TestResult[] results = runner.GetTestResults();
                logWriter.Log(string.Format(
                    "==== Tests Results for Parallel TestGroup {0} ===", runner.TestGroupName));
                j = 0;

                foreach (TestResult res in results)
                {
                    if (!res.Executed)
                    {
                        ++IgnoredTests;
                        continue;
                    }
                    if (res.Executed)
                        ++ExecutedTests;
                    if (res.IsFailure)
                        ++FailedTests;
                    if (res.IsSuccess)
                        ++SuccessTests;

                    PrintResult(++j, res, logWriter, 0);

                    if (res.Time > BiggerTime)
                        BiggerTime = res.Time;

                    if (res.IsFailure)
                        failedTests.Add(res);
                }

                logWriter.Log("Summary:");
                logWriter.Log(string.Format(
                    "\tTotal: {0}\r\n" +
                    "\tExecuted: {1}\r\n" +
                    "\tIgnored: {2}\r\n" +
                    "\tFailed: {3}\r\n" +
                    "\tSuccess: {4}\r\n" +
                    "\t% Success: {5}\r\n" +
                    "\tBiggest Execution Time: {6} s\r\n",
                    results.Length, ExecutedTests, IgnoredTests, FailedTests, SuccessTests,
                    results.Length > 0 ? 100 * SuccessTests / results.Length : 0,
                    BiggerTime));

                TotalTests += results.Length;
                TotalExecutedTests += ExecutedTests;
                TotalIgnoredTests += IgnoredTests;
                TotalFailedTests += FailedTests;
                TotalSuccessTests += SuccessTests;
                TotalBiggerTime += BiggerTime;
            }

            // print all failed tests together
            if (failedTests.Count > 0)
            {
                logWriter.Log("==== Failed tests ===");
                for (j = 0; j < failedTests.Count; ++j)
                    PrintResult(j, failedTests[j] as PNUnitTestResult, logWriter, 0);
            }

            if (runners.Length > 1)
            {
                logWriter.Log("Summary for all the parallel tests:");
                logWriter.Log(string.Format(
                    "R00:\tTotal: {0}\r\nR01:\t" +
                    "Executed: {1}\r\nR02:\t" +
                    "Ignored: {2}\r\nR03:\t" +
                    "Failed: {3}\r\nR04:\t" +
                    "Success: {4}\r\nR05:\t%" +
                    "Success: {5}\r\nR06:\tBiggest Execution Time: {6} s\r\n",
                    TotalTests, TotalExecutedTests, TotalIgnoredTests, TotalFailedTests,
                    TotalSuccessTests,
                    TotalTests > 0 ? 100 * TotalSuccessTests / TotalTests : 0,
                    TotalBiggerTime));
            }

            TimeSpan elapsedTime = endTimeStamp.Subtract(beginTimeStamp);
            logWriter.Log(string.Format("Launcher execution time: {0} seconds", elapsedTime.TotalSeconds));
        }

        internal void WriteFullLog(string resultfile)
        {
            if (resultfile == null || resultfile == string.Empty)
                return;

            if (File.Exists(resultfile))
            {
                File.Delete(resultfile);
            }

            FileStream fs = new FileStream(resultfile,
                FileMode.OpenOrCreate, FileAccess.ReadWrite);

            StreamWriter writer = new StreamWriter(fs);

            try
            {
                writer.Write(mTotalLog);
            }
            finally
            {
                writer.Flush();
                writer.Close();
                fs.Close();
            }
        }

        void WriteTestLog(
            string testGroup,
            TestResult result,
            string machine,
            string fileName)
        {
            if (result == null)
                return;

            lock (mWriteTestLogLock)
            {
                DoWriteTestLog(testGroup, result, machine, fileName);
            }
        }

        static void PrintResult(
            int testNumber,
            TestResult res,
            LogWriter logWriter,
            int indentation)
        {
            string[] messages = logWriter.GetErrorMessages(res);

            logWriter.Log(string.Format("{0}({1}) {2}",
                "".PadLeft(indentation), testNumber, messages[0]));

            if (!res.IsSuccess)
                logWriter.Log(messages[1]);

            if (res.Results == null || res.Results.Count == 0)
                return;

            indentation+=4;

            foreach (TestResult child in res.Results)
            {
                PrintResult(testNumber, child, logWriter, indentation);
            }
        }

        string[] GetErrorMessages(TestResult res)
        {
            string[] result = new string[2];

            result[0] = string.Format(
                "Name: {0}\n  Result: {1,-12} Assert Count: {2,-2} Time: {3,5}",
                res.Name,
                res.IsSuccess ? "SUCCESS" : (res.IsFailure ? "FAILURE" : (!res.Executed ? "NOT EXECUTED" : "UNKNOWN")),
                res.AssertCount,
                res.Time);

            if (!res.IsSuccess)
                result[1] = string.Format(
                    "\nMessage: {0}\nStack Trace:\n{1}\r\n\r\n",
                        res.Message, res.StackTrace);

            return result;
        }

        void DoWriteTestLog(
            string testGroup,
            TestResult result,
            string machine,
            string fileName)
        {
            FileStream fs = null;
            StreamWriter writer = null;

            try
            {
                fs = new FileStream(
                    fileName,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite);

                fs.Seek(0, SeekOrigin.End);
                writer = new StreamWriter(fs);

                writer.WriteLine("==============================================================");

                if (result.IsFailure)
                {
                    writer.WriteLine("Errors for test [{0} {1}] run at agent [{2}]",
                        testGroup, result.Name, machine);

                    string[] messages = GetErrorMessages(result);

                    writer.WriteLine(messages[0]);
                    writer.WriteLine(messages[1]);
                }
                else
                {
                    writer.WriteLine("Log for test [{0} {1}] run at agent [{2}]",
                        testGroup,
                        result.Name, machine);
                }

                writer.WriteLine("\nOutput:");
                if (result is PNUnitTestResult)
                {
                    writer.Write(((PNUnitTestResult)result).Output);
                }
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("Error writing to {0}. {1}",
                    fileName, e.Message);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }

                if (fs != null)
                    fs.Close();
            }
        }

        string mTotalLog = string.Empty;
        string mSmokeResultFile;
        string mSmokeErrorsFile;

        static object mWriteTestLogLock = new object();
        static readonly ILog mLog = LogManager.GetLogger("launcher");
    }
}
