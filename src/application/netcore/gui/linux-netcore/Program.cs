using System;

using Gtk;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.Lib.Threading;
using Codice.Examples.GuiTesting.Linux.Threading;

namespace Codice.Examples.GuiTesting.Linux
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ApplicationArgs appArgs = ApplicationArgs.Parse(args);

                ExceptionsHandler.SetExceptionHandlers();

                ProcessNameSetter.SetProcessName("linux-netcore");

                ThreadWaiterBuilder.Initialize(new GtkApplicationTimerBuilder());

                GuiMessage.Initialize(new GtkGuiMessage());

                Application.Init();
                WindowHandler.LaunchApplicationWindow();

                Application.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex.GetType()}: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);

                ExitCode = 1;
                Application.Quit();
            }
            finally
            {
                Environment.Exit(ExitCode);
            }
        }

        static int ExitCode = 0;
    }
}
