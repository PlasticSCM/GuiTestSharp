using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Threading;
using Codice.Examples.GuiTesting.Windows.Testing;
using Codice.Examples.GuiTesting.Windows.Threading;

namespace Codice.Examples.GuiTesting.Windows
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

                ThreadWaiterBuilder.Initialize(new WinPlasticTimerBuilder());

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (appArgs.IsTestingMode)
                    InstallTestAssembliesResolver(appArgs.PathToAssemblies);

                // Tip: you could launch different windows depending on the
                // argument flags.
                WindowHandler.LaunchApplicationWindow();

                if (appArgs.IsTestingMode)
                    WindowHandler.LaunchTest(appArgs.TestInfoFile, appArgs.PathToAssemblies);

                Application.Run();
            }
            catch
            {
                // You would track the exception here.
                ExitCode = 1;
                Application.Exit();
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
            assemblies.Add("guitests.dll");

            GuiTestRunner.GuiTestAssemblyResolver.InstallAssemblyResolver(
                pathToAssemblies, assemblies);
        }

        static int ExitCode = 0;
    }
}
