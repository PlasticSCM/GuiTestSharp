using System;
using System.IO;
using System.Reflection;
using System.Threading;

using Gtk;

using Codice.Examples.GuiTesting.Lib;

namespace Codice.Examples.GuiTesting.Linux
{
    internal static class WindowHandler
    {
        internal static Window ApplicationWindow
        {
            get { return mApplicationWindow; }
        }

        internal static void LaunchApplicationWindow()
        {
            ApplicationOperations operations = new ApplicationOperations();
            mApplicationWindow = new ApplicationWindow(operations);
            mApplicationWindow.WindowPosition = WindowPosition.Center;
            mApplicationWindow.ShowAll();
        }

        internal static void UnregisterApplicationWindow()
        {
            mApplicationWindow.Dispose();
            mApplicationWindow = null;

            TerminateApplication();
        }

        internal static void SetActiveDialogForTesting(Dialog dialog)
        {
            if (!mbIsTestRun)
                return;

            mActiveDialog = dialog;
        }

        internal static void RemoveDialogForTesting(Dialog dialog)
        {
            if (!mbIsTestRun)
                return;

            if (mActiveDialog == dialog)
                mActiveDialog = null;
        }

        internal static Dialog GetActiveDialog()
        {
            return mActiveDialog;
        }

        static void TerminateApplication()
        {
            if (mbIsTestRun)
                return;

            Application.Quit();
        }

        static ApplicationWindow mApplicationWindow;
        static Dialog mActiveDialog;
        static bool mbIsTestRun = false;
    }
}
