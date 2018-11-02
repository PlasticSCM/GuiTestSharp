using Codice.Examples.GuiTesting.Lib;
using System;

namespace Codice.Examples.GuiTesting.Linux
{
    internal static class WindowHandler
    {
        internal static void LaunchApplicationWindow()
        {
            ApplicationOperations operations = new ApplicationOperations();
            mApplicationWindow = new ApplicationWindow(operations);
            mApplicationWindow.Show();
            mApplicationWindow.Present();
        }

        internal static void LaunchTest(string testInfoFile, string pathToAssemblies)
        {
            throw new NotImplementedException();
        }

        static ApplicationWindow mApplicationWindow;
    }
}
