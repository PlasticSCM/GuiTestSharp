using System;
using System.Text;

using NUnit.Core;

using PNUnit.Framework;

namespace PNUnit.Launcher
{
    internal static class TestSuiteEntry
    {
        internal class Data
        {
            internal string ServerConfig = string.Empty;
            internal string BackendType = string.Empty;
            internal string ClientConfig = string.Empty;
            internal int ExecTime = 0;
            internal string Status = FAILURE_STATUS;
            internal string Log = string.Empty;
        }

        internal static Data Calculate(
            TestResult[] testResults, bool bLogSuccessful)
        {
            Data result = new Data();

            foreach (PNUnitTestResult tr in testResults)
            {
                if (string.IsNullOrEmpty(tr.BackendType))
                {
                    //<ParallelTest>: client test
                    result.ClientConfig = tr.OSVersion;
                    result.ExecTime = (int)Math.Round(tr.Time);
                    result.Status = tr.ResultState.ToString();

                    if (tr.Killed)
                        result.Status = KILLED_STATUS;

                    if (bLogSuccessful || tr.ResultState != ResultState.Success)
                    {
                        result.Log += CollectOutput(tr);
                        result.Log = result.Log.Replace("'", "''").Replace("\"", "''");
                    }
                }
                else
                {
                    //<ParallelTest>: server test
                    result.ServerConfig = tr.OSVersion;
                    result.BackendType = tr.BackendType;
                }

                if (tr.ResultState != ResultState.Success &&
                    tr.ResultState != ResultState.Ignored)
                {
                    result.Log += "---------------------------";
                    result.Log += "ERROR: " + tr.Message;
                    result.Log += "---------------------------";
                }
            }
            return result;
        }

        static string CollectOutput(PNUnitTestResult tr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(tr.Output);

            if (tr.Results != null && tr.Results.Count > 0)
                foreach (TestResult child in tr.Results)
                    DoCollectOutput(child, sb);

            return sb.ToString();
        }

        static void DoCollectOutput(TestResult res, StringBuilder sb)
        {
            if (res.Results != null && res.Results.Count > 0)//skip non-leaf nodes
            {
                foreach (TestResult child in res.Results)
                    DoCollectOutput(child, sb);

                return;
            }

            sb.AppendLine();

            sb.AppendFormat(
                "Test: {0}\n  Result: {1,-14} Assert Count: {2,-2} Time: {3,5}",
                res.FullName,
                GetExecutionResult(res),
                res.AssertCount,
                res.Time);

            sb.AppendLine();

            if (!res.IsSuccess)
            {
                sb.AppendLine("Message: " + res.Message);
                sb.AppendLine("Stack Trace: ");
                sb.AppendLine(res.StackTrace);
            }
        }

        static string GetExecutionResult(TestResult res)
        {
            if (res.IsSuccess) return "SUCCESS";
            if (res.IsFailure) return "FAILURE";
            if (!res.Executed) return "NOT EXECUTED";
            return "UNKNOWN";
        }

        const string FAILURE_STATUS = "Failure";
        const string KILLED_STATUS = "Killed";
    }
}
