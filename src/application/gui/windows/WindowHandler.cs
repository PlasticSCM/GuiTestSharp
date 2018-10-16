using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using log4net;
using log4net.Config;
using log4net.Repository;

using GuiTest;

using Codice.Examples.GuiTesting.GuiTestInterfaces;
using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Windows.Testing;

namespace Codice.Examples.GuiTesting.Windows
{
    internal static class WindowHandler
    {
        internal static void LaunchApplicationWindow()
        {
            ApplicationOperations operations = new ApplicationOperations();
            mApplicationWindow = new ApplicationWindow(operations);
            mApplicationWindow.Show();
        }

        internal static void UnregisterApplicationWindow()
        {
            mApplicationWindow.Dispose();
            mApplicationWindow = null;

            TerminateApplication();
        }

        internal static void LaunchTest(string testInfoFile, string pathToAssemblies)
        {
            mbIsTestRun = true;

            ConfigureTestLogging();
            InitTesteableServices();

            GuiTestRunner testRunner = new GuiTestRunner(
                testInfoFile, pathToAssemblies, new GuiFinalizer(), null);

            ThreadPool.QueueUserWorkItem(new WaitCallback(testRunner.Run));
        }

        static void ConfigureTestLogging()
        {
            try
            {
                string log4netPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "pnunittestrunner.log.conf");

                XmlConfigurator.Configure(mLogRepository, new FileInfo(log4netPath));
            }
            catch { } // Log config failed, nothing else to do!
        }

        static void InitTesteableServices()
        {
            TesteableApplicationWindow testeableWindow =
                new TesteableApplicationWindow(mApplicationWindow);

            GuiTesteableServices.Init(testeableWindow);
        }

        static void TerminateApplication()
        {
            if (mbIsTestRun)
                return;

            Application.Exit();
        }

        static ApplicationWindow mApplicationWindow;
        static bool mbIsTestRun = false;

        static readonly ILoggerRepository mLogRepository =
            LogManager.CreateRepository("log4net-rep");

        class GuiFinalizer : GuiTestRunner.IGuiFinalizer
        {
            void GuiTestRunner.IGuiFinalizer.Finalize(int exitCode)
            {
                Environment.Exit(exitCode);
            }
        }
    }
}
