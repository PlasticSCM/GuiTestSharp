using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using log4net;

namespace PNUnit.Launcher
{
    internal class CliArgsReader
    {
        internal static LauncherArgs ProcessArgs(string[] args, TestGroup group)
        {
            LauncherArgs result = new LauncherArgs();

            result.ConfigFile = args[0];

            string testPath = Path.GetDirectoryName(Path.GetFullPath(result.ConfigFile));

            result.TestRange = new TestRange(0, group.ParallelTests.Count - 1);

            result.FailedConfigFile = Path.Combine(testPath, "smokefailed.conf");
            result.ResultLogFile = Path.Combine(testPath, SMOKE_RESULT_FILE);
            result.ErrorLogFile = Path.Combine(testPath, SMOKE_ERRORS_FILE);

            if (args.Length <= 1)
                return result;

            foreach (string arg in args)
            {
                if (arg.StartsWith("--result="))
                {
                    result.ResultFile = Path.GetFullPath(arg.Substring(9));
                    continue;
                }

                if (arg.StartsWith("--failed="))
                {
                    result.FailedConfigFile = Path.GetFullPath(arg.Substring(9));
                    continue;
                }

                if (arg.StartsWith("--retry="))
                {
                    result.RetryOnFailure = int.Parse(arg.Substring("--retry=".Length));
                    mLog.InfoFormat("Retry on failure activated. {0} retries", result.RetryOnFailure);
                    result.MaxRetry = result.RetryOnFailure;
                    continue;
                }

                if (arg.StartsWith("--max_barrier_wait_time="))
                {
                    int maxBarrierTime = int.Parse(arg.Substring("--max_barrier_wait_time=".Length));
                    mLog.InfoFormat("Max Barrier wait time set to: {0} seconds", maxBarrierTime);
                    Barrier.SetMaxWaitTime(maxBarrierTime);
                    continue;
                }

                if (arg.Equals("--shell"))
                {
                    result.ShellMode = true;
                    continue;
                }

                if (arg.StartsWith("--test="))
                {
                    result.TestRange = LaunchATest(arg, group);
                    continue;
                }

                if (arg.StartsWith("--range="))
                {
                    result.TestRange = LaunchARange(arg, group);
                    continue;
                }

                if (arg.StartsWith("--pattern="))
                {
                    LaunchAPattern(arg, group);

                    //update test range
                    result.TestRange.SetTestRange(0, group.ParallelTests.Count - 1);
                    continue;
                }

                if (arg.StartsWith("--timeout"))
                {
                    result.TestsTimeout = SetTestTimeout(arg);
                }

                if (arg.StartsWith("--testslist="))
                {
                    result.ListTestsFile = Path.GetFullPath(arg.Substring("--testslist=".Length));
                }

                if (arg.StartsWith("--usefilereport="))
                {
                    result.UseFileReport = Path.GetFullPath(arg.Substring("--usefilereport=".Length));
                    continue;
                }
            }

            return result;
        }

        internal static string GetArgumentValue(string argument, string[] args)
        {
            foreach (string arg in args)
            {
                if (!arg.StartsWith(argument))
                    continue;

                 return arg.Substring(argument.Length);
            }
            return null;
        }

        internal static TestSuiteLoggerParams ProcessTestSuiteLoggerArgs(string[] args)
        {
            string buildType = string.Empty;
            string buildName = string.Empty;
            int cset = -1;
            string buildComment = string.Empty;
            string suiteType = string.Empty;
            string suiteName = string.Empty;
            string host = string.Empty;
            string vmachine = string.Empty;
            bool bLogSuccessfulTests = false;
            bool bInitialized = false;

            foreach (string arg in args)
            {
                if (arg.IndexOf('=') < 0)
                    continue;

                string field = arg.Substring(0, arg.IndexOf("=")).ToLower();

                switch (field)
                {
                    case "--buildtype":
                        bInitialized = true;
                        buildType = arg.Substring("--buildtype=".Length).ToLower();
                        break;
                    case "--buildname":
                        bInitialized = true;
                        buildName = arg.Substring("--buildname=".Length).ToLower();
                        break;
                    case "--cset":
                        bInitialized = true;
                        cset = Int32.Parse(arg.Substring("--cset=".Length).ToLower());
                        break;
                    case "--buildcomment":
                        bInitialized = true;
                        buildComment = arg.Substring("--buildcomment=".Length).ToLower();
                        break;
                    case "--suitetype":
                        bInitialized = true;
                        suiteType = arg.Substring("--suitetype=".Length).ToLower();
                        break;
                    case "--suitename":
                        bInitialized = true;
                        suiteName = arg.Substring("--suitename=".Length).ToLower();
                        break;
                    case "--host":
                        bInitialized = true;
                        host = arg.Substring("--host=".Length).ToLower();
                        break;
                    case "--vmachine":
                        bInitialized = true;
                        vmachine = arg.Substring("--vmachine=".Length).ToLower();
                        break;
                    case "--logsuccess":
                        bInitialized = true;
                        bLogSuccessfulTests =
                            Boolean.Parse(arg.Substring("--logsuccess=".Length).ToLower());
                        break;
                }
            }

            if (bInitialized)
                return new TestSuiteLoggerParams(
                    buildType,
                    buildName,
                    cset,
                    buildComment,
                    suiteType,
                    suiteName,
                    host,
                    vmachine,
                    bLogSuccessfulTests);

            return new TestSuiteLoggerParams();
        }


        private const string USER_VALUE_KEY = "-val:";

        internal static Hashtable GetUserValues(string[] args)
        {
            Hashtable result = new Hashtable();

            foreach (string s in args)
            {
                if (!s.ToLower().StartsWith(USER_VALUE_KEY))
                    continue;

                string[] v = s.Substring(USER_VALUE_KEY.Length).Split('=');

                if (v.Length >= 1)
                {
                    string name = v[0];
                    string val = string.Empty;

                    if (v.Length == 2)
                        val = v[1];

                    result.Add(name, val);
                }
            }

            return result;
        }

        static TestRange LaunchATest(string arg, TestGroup group)
        {
            string testName = arg.Substring("--test=".Length);
            int index = -1;
            for (int i = 0; i < group.ParallelTests.Count; i++)
            {
                if (group.ParallelTests[i].Name != testName)
                    continue;

                index = i;
                break;
            }

            if (index == -1)
            {
                Console.WriteLine("The specified test was not found");
                return null;
            }

            return new TestRange(index, index);
        }

        static int SetTestTimeout(string arg)
        {
            return Int32.Parse(arg.Substring("--timeout=".Length));
        }

        static TestRange LaunchARange(string arg, TestGroup group)
        {
            string rangeText = arg.Substring("--range=".Length);
            string[] limits = rangeText.Split('-');

            if (!CheckValidRangeValues(rangeText, limits))
            {
                return null;
            }

            TestRange result = CalculateRange(limits, group);

            if (!CheckValidInterval(result, group))
            {
                return null;
            }

            mLog.InfoFormat("Starting test range [{0}-{1}]",
                result.StartTest, result.EndTest);

            return result;
        }

        static bool CheckValidRangeValues(string rangeText, string[] limits)
        {
            if (rangeText.IndexOf("-") < 0)
            {
                Console.WriteLine("Test range incorrectly specified, it must be something like 0-10");
                return false;
            }

            if (limits.Length != 2)
            {
                Console.WriteLine("Test range incorrectly specified, it must be something like 0-10");
                return false;
            }
            return true;
        }

        static void LaunchAPattern(string arg, TestGroup group)
        {
            string pattern = arg.Substring("--pattern=".Length).ToLower();

            List<ParallelTest> originalList = new List<ParallelTest>(group.ParallelTests);

            foreach (ParallelTest test in originalList)
            {
                if (test.Name.ToLower().IndexOf(pattern) < 0)
                {
                    group.ParallelTests.Remove(test);
                }
            }
        }

        static TestRange CalculateRange(string[] ranges, TestGroup group)
        {
            //--range=0-LAST == --range=0-group.ParallelTests.Length -1,
            //whatever the number of tests is
            if (ranges[1] == "LAST")
            {
                return new TestRange(int.Parse(ranges[0]), group.ParallelTests.Count - 1);
            }
            else
            {
                return new TestRange(int.Parse(ranges[0]), int.Parse(ranges[1]));
            }
        }

        static bool CheckValidInterval(TestRange testRange, TestGroup group)
        {
            if ((testRange.StartTest > testRange.EndTest) ||
                (testRange.StartTest < 0) ||
                (testRange.StartTest > group.ParallelTests.Count - 1))
            {
                Console.WriteLine("Start test must be in a correct test range");
                return false;
            }

            if ((testRange.EndTest < testRange.StartTest) ||
                (testRange.EndTest < 0) ||
                (testRange.EndTest > group.ParallelTests.Count - 1))
            {
                Console.WriteLine("End test must be in a correct test range");
                return false;
            }
            return true;
        }

        const string SMOKE_RESULT_FILE = "smoke-result.log";
        const string SMOKE_ERRORS_FILE = "smoke-errors.log";

        static readonly ILog mLog = LogManager.GetLogger("launcher");
    }
}