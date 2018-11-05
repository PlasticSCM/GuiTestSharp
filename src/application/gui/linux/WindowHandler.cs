using System;

using Gtk;

using Codice.Examples.GuiTesting.Lib;

namespace Codice.Examples.GuiTesting.Linux
{
    internal static class WindowHandler
    {
        internal static void LaunchApplicationWindow()
        {
            ApplicationOperations operations = new ApplicationOperations();
            mApplicationWindow = new ApplicationWindow(operations);
            mApplicationWindow.ShowAll();
        }

        internal static void UnregisterApplicationWindow()
        {
            mApplicationWindow.Dispose();
            mApplicationWindow = null;

            TerminateApplication();
        }

        internal static void LaunchTest(string testInfoFile, string pathToAssemblies)
        {
            throw new NotImplementedException();
        }

        static void TerminateApplication()
        {
            if (mbIsTestRun)
                return;

            Application.Quit();
        }

        static ApplicationWindow mApplicationWindow;
        static bool mbIsTestRun = false;
    }
}
