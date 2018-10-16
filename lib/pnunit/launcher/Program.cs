using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.IO;

using PNUnit.Launcher.Automation;

using log4net;

namespace PNUnit.Launcher
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ProcessNameSetter.SetProcessName("launcher");

            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Usage: launcher configfile [--result=filename] [--failed=filename] " + Environment.NewLine +
                    "\t[--max_barrier_wait_time=number_of_seconds] [--timeout=seconds]" + Environment.NewLine +
                    "\t[-D:var=value] [-val:variable=value] [--retry=number] [--range=from-to] " + Environment.NewLine +
                    "\t[--test=testname] [--shell] [[--buildtype=release|task|private|testrun|...]" + Environment.NewLine +
                    "\t[--buildname=4.1.2.411|SCM12547|...][--cset=changeset][--buildcomment=comment]" + Environment.NewLine +
                    "\t[--suitetype=debug-smoke|debug-gui|release-smoke|...][--suitename=oracle...]" + Environment.NewLine +
                    "\t[--logsuccess=true|false]] [--skipsummarylog] " + Environment.NewLine +
                    "\t[--usefilereport=file] " + Environment.NewLine +
                    "\t[--automated] [--logfolder=<output_log_path_for_launchers>]" + Environment.NewLine +
                    "\t[--port=port_number] " + Environment.NewLine +
                    "\t[--iptobind=launcher_ip_used_by_test_runners] " + Environment.NewLine +
                    "\t[--testslist=<file_with_testname_per_line>]");
                Console.WriteLine();
                Console.WriteLine("Example:");
                Console.WriteLine("launcher smokeldap.conf --iptobind=127.0.0.1 -val:show_cmd_no=true -D:$authmode=LDAPWorkingMode --range=0-0");
                return;
            }

            bool bIsAutomatedLauncher = args[0] == "--automated";
            string customLogFolder = GetCustomLogOutputFolder(ref args);

            Configurator.ConfigureLogging(bIsAutomatedLauncher, customLogFolder);

            if (bIsAutomatedLauncher)
            {
                RunAutomatedLauncher(args);
                return;
            }

            RunLauncher(customLogFolder, args);
        }

        static string GetCustomLogOutputFolder(ref string[] args)
        {
            string resultPath = string.Empty;
            List<string> filteredArgs = new List<string>();

            foreach (string arg in args)
            {
                if (!arg.StartsWith("--logfolder="))
                {
                    filteredArgs.Add(arg);
                    continue;
                }

                resultPath = arg.Substring("--logfolder=".Length).Trim();
            }

            args = filteredArgs.ToArray();

            if (string.IsNullOrEmpty(resultPath))
                return string.Empty;

            if (resultPath[resultPath.Length - 1] == Path.DirectorySeparatorChar)
                return resultPath;

            return resultPath + Path.DirectorySeparatorChar;
        }

        static void RunLauncher(string customLogFolder, string[] args)
        {
            TestGroup group = TestConfLoader.LoadFromFile(args[0], args);

            LauncherArgs launcherArgs = CliArgsReader.ProcessArgs(args, group);

            if ((group == null) || (group.ParallelTests.Count == 0))
            {
                Console.WriteLine("No tests to run");
                return;
            }

            TestSuiteLoggerParams loggerParams = CliArgsReader.ProcessTestSuiteLoggerArgs(args);

            NUnitResultCollector nunitReport = new NUnitResultCollector();
            LogWriter logWriter = new LogWriter(launcherArgs.ResultLogFile, launcherArgs.ErrorLogFile);

            try
            {
                string portValue = CliArgsReader.GetArgumentValue("--port=", args);
                int port = portValue == null ? DEFAULT_LAUNCHER_PORT : int.Parse(portValue);

                string ipToBind = CliArgsReader.GetArgumentValue("--iptobind=", args);

                Configurator.ConfigureRemoting(port, ipToBind ?? string.Empty);

                DateTime beginTimeStamp = DateTime.Now;

                TestSuiteLogger testSuiteLogger = null;

                if (loggerParams.IsInitialized())
                {
                    testSuiteLogger = new TestSuiteLogger(loggerParams);
                    testSuiteLogger.SaveBuild();
                    testSuiteLogger.CreateSuite();
                }

                Hashtable userValues = CliArgsReader.GetUserValues(args);

                Launcher launcher = new Launcher();

                string listenAddress = string.Format("{0}:{1}",
                    ipToBind ?? Environment.MachineName, port);

                List<string> testList = string.IsNullOrEmpty(launcherArgs.ListTestsFile) ?
                    null :
                    LoadTestsToRunFromFile(launcherArgs.ListTestsFile);

                Runner[] runners = launcher.RunTests(
                    group,
                    testList,
                    launcherArgs.MaxRetry,
                    launcherArgs.ShellMode,
                    launcherArgs.RetryOnFailure,
                    launcherArgs.FailedConfigFile,
                    testSuiteLogger,
                    launcherArgs.TestsTimeout,
                    launcherArgs.TestRange,
                    userValues,
                    logWriter,
                    listenAddress,
                    launcherArgs.UseFileReport);

                DateTime endTimeStamp = DateTime.Now;

                FillNunitReport(nunitReport, runners);

                if (CliArgsReader.GetArgumentValue("--skipsummarylog", args) != null)
                    return;

                LogWriter.PrintResults(
                    runners,
                    beginTimeStamp, endTimeStamp, logWriter);
            }
            finally
            {
                logWriter.WriteFullLog(launcherArgs.ResultFile);
                nunitReport.SaveResults(Path.Combine(customLogFolder, "pnunit-results.xml"));
            }
        }

        static void FillNunitReport(NUnitResultCollector nunitReport, Runner[] runners)
        {
            foreach (Runner runner in runners)
            {
                if (runner == null) continue;

                NUnit.Core.TestResult[] results = runner.GetTestResults();
                nunitReport.AddResults(results);
            }
        }

        static void RunAutomatedLauncher(string[] args)
        {
            mLog.DebugFormat("Starting automated launcher with args {0}", PrintArgs(args));

            int port;

            if (args.Length < 2)
            {
                LogError("Port wasn't specified. Launcher can't start");
                return;
            }

            if (!int.TryParse(args[1], out port))
            {
                LogError(string.Format(
                    "Invalid port '{0}' specified. Launcher can't start", args[1]));
                return;
            }

            string ipToBind = args.Length == 3 ? args[2] : string.Empty;

            if (!Configurator.ConfigureRemoting(port, ipToBind))
            {
                LogError(string.Format(
                    "Error configuring remoting in port '{0}'. Launcher can't start",args[1]));
                return;
            }

            LauncherService automated = new LauncherService(ipToBind, port);

            ObjRef objRef = RemotingServices.Marshal(automated, "AutomatedLauncher");

            Console.WriteLine(START_MESSAGE_PATTERN, "started ok");
            while (!automated.CanExit)
            {
                System.Threading.Thread.Sleep(200);
            }
        }

        static List<string> LoadTestsToRunFromFile(string listTestsFile)
        {
            if (!File.Exists(listTestsFile))
                return null;

            List<string> testsList = new List<string>();
            string line = null;

            using (StreamReader sr = new StreamReader(listTestsFile))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length < 1 || line[0] == '#')
                        continue;

                    testsList.Add(line.Trim());
                }
            }

            return testsList;
        }

        static void LogError(string errorMessage)
        {
            mLog.Fatal(errorMessage);
            Console.WriteLine(START_MESSAGE_PATTERN, errorMessage);
        }

        static string PrintArgs(string[] args)
        {
            string result = string.Empty;

            foreach (string arg in args)
                result += " " + arg;

            return result;
        }

        const string START_MESSAGE_PATTERN = "Automated launched start: {0}";
        const int DEFAULT_LAUNCHER_PORT = 8079;

        static readonly ILog mLog = LogManager.GetLogger("launcher");
    }

}