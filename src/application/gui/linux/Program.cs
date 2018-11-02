using System;
using System.Collections.Generic;

using Gtk;

using GuiTest;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.Lib.Threading;
using Codice.Examples.GuiTesting.Linux.Threading;

namespace Codice.Examples.GuiTesting.Linux
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                ApplicationArgs appArgs = ApplicationArgs.Parse(args);

                ExceptionsHandler.SetExceptionHandlers(appArgs.IsTestingMode);

                ThreadWaiterBuilder.Initialize(new GtkApplicationTimerBuilder());

                GuiMessage.Initialize(new GtkGuiMessage());

                if (appArgs.IsTestingMode)
                    InstallTestAssembliesResolver(appArgs.PathToAssemblies);

                // Tip: you could launch different windows depending on the
                // argument flags.
                Application.Init();
                WindowHandler.LaunchApplicationWindow();

                if (appArgs.IsTestingMode)
                    WindowHandler.LaunchTest(appArgs.TestInfoFile, appArgs.PathToAssemblies);

                Application.Run();
            }
            catch (Exception ex)
            {
                // You would track the exception here
                Console.Error.WriteLine($"{ex.GetType()}: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);

                ExitCode = 1;
                Application.Quit();
            }
            finally
            {
                // You would dispose everything you need here.
                Environment.Exit(ExitCode);
            }
        }

        static void InstallTestAssembliesResolver(string pathToAssemblies)
        {
            List<string> assemblies = new List<string>();
            assemblies.Add("nunit.core.dll");
            assemblies.Add("nunit.core.interfaces.dll");
            assemblies.Add("nunit.util.dll");
            assemblies.Add("pnunit.framework.dll");

            assemblies.Add("guitestinterfaces.dll");
            assemblies.Add("guitest.dll");

            GuiTestRunner.GuiTestAssemblyResolver.InstallAssemblyResolver(
                pathToAssemblies, assemblies);
        }

        static int ExitCode = 0;
    }
}
