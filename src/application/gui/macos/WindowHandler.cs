using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using AppKit;
using Foundation;

using log4net;
using log4net.Config;
using log4net.Repository;

using GuiTest;

using Codice.Examples.GuiTesting.GuiTestInterfaces;
using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.MacOS.Testing;

namespace Codice.Examples.GuiTesting.MacOS
{
    internal static class WindowHandler
    {
        internal static NSWindow ApplicationWindow
        {
            get { return mApplicationWindow; }
        }

        internal static void Initialize(NSObject application)
        {
            mApplication = application;
        }

        internal static void LaunchApplicationWindow()
        {
            ApplicationOperations operations = new ApplicationOperations();
            mApplicationWindow = new ApplicationWindow(operations);
            mApplicationWindow.ReleasedWhenClosed = true;
            mApplicationWindow.MakeKeyAndOrderFront(mApplication);
        }

        internal static void UnregisterApplicationWindow()
        {
            mApplicationWindow = null;
        }

        internal static void SetActiveDialogForTesting(NSObject dialog)
        {
            if (!mbIsTestRun)
                return;

            mActiveDialog = dialog;
        }

        internal static void RemoveDialogForTesting(NSObject dialog)
        {
            if (!mbIsTestRun)
                return;

            if (mActiveDialog == dialog)
                mActiveDialog = null;
        }

        internal static NSObject GetActiveDialog()
        {
            return mActiveDialog;
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

                XmlConfigurator.Configure(new FileInfo(log4netPath));
            }
            catch { }
        }

        static void InitTesteableServices()
        {
            TesteableApplicationWindow testeableWindow =
                new TesteableApplicationWindow(mApplicationWindow);

            GuiTesteableServices.Init(testeableWindow);
        }

        static NSObject mApplication;

        static ApplicationWindow mApplicationWindow;
        static NSObject mActiveDialog;
        static bool mbIsTestRun = false;

        class GuiFinalizer : GuiTestRunner.IGuiFinalizer
        {
            void GuiTestRunner.IGuiFinalizer.Finalize(int exitCode)
            {
                // Environmnet.Exit is unable to terminate unmanaged threads
                // launched by the native Cocoa framework. We don't use any
                // in this example application, but you could face crashes
                // if you decide to rely on Environment.Exit to set an exit code.
                Exit(exitCode);
            }

            [DllImport("__Internal", EntryPoint = "exit")]
            static extern void Exit(int exitCode);
        }
    }
}
