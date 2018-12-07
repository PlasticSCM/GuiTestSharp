using System;

using AppKit;
using Foundation;

using Codice.Examples.GuiTesting.Lib;

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
            throw new NotImplementedException();
        }

        internal static void SetActiveDialogForTesting(NSObject dialog)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveDialogForTesting(NSObject dialog)
        {
            throw new NotImplementedException();
        }

        internal static NSObject GetActiveDialog()
        {
            throw new NotImplementedException();
        }

        internal static void LaunchTest(string testInfoFile, string pathToAssemblies)
        {
            throw new NotImplementedException();
        }

        static void ConfigureTestLogging()
        {
            throw new NotImplementedException();
        }

        static void InitTesteableServices()
        {
            throw new NotImplementedException();
        }

        static NSObject mApplication;

        static ApplicationWindow mApplicationWindow;
        static NSObject mActiveDialog;
        static bool mbIsTestRun = false;
    }
}
