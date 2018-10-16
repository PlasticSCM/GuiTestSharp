using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;

using log4net;
using PNUnit.Framework;
using NUnit.Util;

namespace GuiTest
{
    public class GuiTestRunner
    {
        public interface IGuiFinalizer
        {
            void Finalize(int exitCode);
        }

        public GuiTestRunner(
            string testInfoFile,
            string pathToAssemblies,
            IGuiFinalizer guiFinalizer,
            ITestConsoleAccess guiConsole)
        {
            mTestInfoFile = testInfoFile;
            mPathToAssemblies = pathToAssemblies;
            mGuiFinalizer = guiFinalizer;
            mGuiConsole = guiConsole;
        }

        public void Run(object state)
        {
            InitServices.InitNUnitServices();

            PNUnitTestInfo testInfo = TestInfoReader.ReadTestInfo(mTestInfoFile);

            if (testInfo == null)
            {
                Console.Error.WriteLine("Cannot execute tests without information. Exiting...");
                mGuiFinalizer.Finalize(1);
            }

            IPNUnitServices services = GetPNUnitServices(testInfo.PNUnitServicesServer);
            PNUnitTestRunner.PNUnitTestRunner runner =
                new PNUnitTestRunner.PNUnitTestRunner(mPathToAssemblies);

            runner.Run(testInfo, services, mGuiConsole);

            GuiTestAssemblyResolver.UninstallAssemblyResolver();

            mGuiFinalizer.Finalize(1);
        }

        static IPNUnitServices GetPNUnitServices(string server)
        {
            return string.IsNullOrEmpty(server)
                ? new DummyPnunitServices()
                : PNUnitServices.GetPNunitServicesProxy(server);
        }

        readonly string mTestInfoFile;
        readonly string mPathToAssemblies;
        readonly IGuiFinalizer mGuiFinalizer;
        readonly ITestConsoleAccess mGuiConsole;

        internal static class GuiTestAssemblyResolver
        {
            internal static void InstallAssemblyResolver(
                string pathToAssemblies, List<string> assemblies)
            {
                mPathToAssemblies = pathToAssemblies;
                mAssemblies = assemblies;

                AppDomain.CurrentDomain.AssemblyResolve += Resolve;
            }

            internal static void UninstallAssemblyResolver()
            {
                AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
            }

            static Assembly Resolve(object sender, ResolveEventArgs e)
            {
                string[] asm = e.Name.Split(',');

                string assemblyName = asm[0];

                if (!assemblyName.EndsWith(".dll"))
                    assemblyName += ".dll";

                if (mAssemblies == null || !mAssemblies.Contains(assemblyName))
                    return null;

                string assemblyFullPath = Path.Combine(mPathToAssemblies, assemblyName);

                return Assembly.LoadFrom(assemblyFullPath);
            }

            static string mPathToAssemblies;
            static List<string> mAssemblies;
        }

        class InitServices
        {
            internal static void InitNUnitServices()
            {
                ServiceManager.Services.AddService(new SettingsService());
                ServiceManager.Services.AddService(new DomainManager());
                ServiceManager.Services.AddService(new ProjectService());

                ServiceManager.Services.AddService(new SettingsService());
                ServiceManager.Services.AddService(new DomainManager());
                ServiceManager.Services.AddService(new ProjectService());

                NUnit.Core.CoreExtensions.Host.InitializeService();
                ServiceManager.Services.InitializeServices();
            }
        }

        class TestInfoReader
        {
            internal static PNUnitTestInfo ReadTestInfo(string testInfoPath)
            {
                int init = Environment.TickCount;
                try
                {
                    using (FileStream fs = new FileStream(testInfoPath, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                        return (PNUnitTestInfo)bf.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    mLog.ErrorFormat(
                        "Something went wrong while reading the agent info: {0}",
                        ex.Message);
                    return null;
                }
                finally
                {
                    TryDelete(testInfoPath);
                    mLog.DebugFormat(
                        "Read test info file {0} in {1} ms.",
                        testInfoPath, Environment.TickCount - init);
                }
            }

            static void TryDelete(string testInfoPath)
            {
                try
                {
                    File.Delete(testInfoPath);
                }
                catch (Exception ex)
                {
                    mLog.WarnFormat(
                        "Could not delete test info file {0}: {1}",
                        testInfoPath, ex.Message);

                    mLog.DebugFormat(
                        "StackTrace:{0}{1}",
                        Environment.NewLine, ex.StackTrace);
                }
            }

            static readonly ILog mLog = LogManager.GetLogger("TestInfoReader");
        }

        class DummyPnunitServices : IPNUnitServices
        {
            void IPNUnitServices.NotifyResult(string testName, PNUnitTestResult result) { }
            void IPNUnitServices.InitBarriers(string testName) { }
            void IPNUnitServices.InitBarrier(string testName, string barrierName, int max) { }
            void IPNUnitServices.EnterBarrier(string testName, string barrier) { }
            void IPNUnitServices.SendMessage(string tag, int receivers, object message) { }
            object IPNUnitServices.ReceiveMessage(string tag) { return null; }
            void IPNUnitServices.ISendMessage(string tag, int receivers, object message) { }
            object IPNUnitServices.IReceiveMessage(string tag) { return null; }
            void IPNUnitServices.NotifyNUnitProgress(
                int runTestsCount,
                int totalTestCount,
                List<string> failed,
                List<string> ignored, bool bFinished) { }
        }
    }
}
