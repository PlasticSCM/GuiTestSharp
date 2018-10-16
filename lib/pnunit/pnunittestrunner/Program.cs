using System;
using System.IO;
using System.Reflection;

using log4net;
using log4net.Config;

using PNUnit.Framework;

namespace PNUnitTestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogging();

            // usage: pnunitinfofile agentconfigfile path to assemblies
            if (args.Length != 2)
            {
                Console.WriteLine("Wrong number of parameters; exiting ...");
                Environment.Exit(1);
            }

            if (args[0] == "preload")
            {
                RunPreload(args[1]);
                return;
            }

            InitServices.InitNUnitServices();

            RunOnce(args[0], args[1]);
        }

        static void RunOnce(string testInfoFile, string pathToAssemblies)
        {
            PNUnitTestInfo info = TestInfoReader.ReadTestInfo(testInfoFile);

            if (info == null)
            {
                Console.WriteLine("Cannot execute tests without information; exiting ...");
                Environment.Exit(1);
            }

            ProcessNameSetter.SetProcessName(info.TestName);

            IPNUnitServices services =
                PNUnitServices.GetPNunitServicesProxy(info.PNUnitServicesServer);

            if (info.TestName.StartsWith("run-nunit"))
            {
                new NUnitTestRunner(info, pathToAssemblies).Run(services);
                return;
            }

            new PNUnitTestRunner(pathToAssemblies).Run(info, services, null);
        }

        static void RunPreload(string pathToAssemblies)
        {
            mLog.Debug("Preload started. Path To assemblies:" + pathToAssemblies);
            InitServices.InitNUnitServices();

            PNUnitTestRunner runner = new PNUnitTestRunner(pathToAssemblies);
            runner.Preload();

            int pidOfThisExpectedByAgent = Codice.Test.PlatformIdentifier.IsWindows() ?
                System.Diagnostics.Process.GetCurrentProcess().Id :
                Mono.Unix.UnixEnvironment.GetParentProcessId();

            string testInfoFile = Path.Combine(
                Path.GetTempPath(),
                PNUnit.Agent.AssemblyPreload.PRELOADED_PROCESS_FILE_PREFIX + pidOfThisExpectedByAgent.ToString());

            int count = 0;
            while(!File.Exists(testInfoFile))
            {
                System.Threading.Thread.Sleep(150);
                mLog.DebugFormat("Waiting for testinfo file to be created...: {0}", testInfoFile);

                count++;

                if (count >= 6000) //wait 1,5 minutes for test arrival
                {
                    mLog.Fatal("Tired of waiting: Cannot execute tests without information; exiting ...");
                    Environment.Exit(1);
                }

            }

            mLog.DebugFormat("Preload read {0} from file", testInfoFile);

            PNUnitTestInfo info = TestInfoReader.ReadTestInfo(testInfoFile);

            if (info == null)
            {
                mLog.Fatal("Cannot execute tests without information; exiting ...");
                Environment.Exit(1);
            }

            ProcessNameSetter.SetProcessName(info.TestName);

            IPNUnitServices services =
                PNUnitServices.GetPNunitServicesProxy(info.PNUnitServicesServer);

            runner.Run(info, services, null);
        }

        static void ConfigureLogging()
        {
            string log4netpath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "pnunittestrunner.log.conf");

            XmlConfigurator.Configure(new FileInfo(log4netpath));
        }

        static readonly ILog mLog = LogManager.GetLogger("Program");
    }
}
