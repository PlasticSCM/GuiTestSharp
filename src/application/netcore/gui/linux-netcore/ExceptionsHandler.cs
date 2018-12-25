using System;

namespace Codice.Examples.GuiTesting.Linux
{
    internal static class ExceptionsHandler
    {
        internal static void SetExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            GLib.ExceptionManager.UnhandledException += HandleUnhandledGlibException;
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex == null)
                return;

            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }

        static void HandleUnhandledGlibException(GLib.UnhandledExceptionArgs e)
        {
            e.ExitApplication = false;

            Exception ex = e.ExceptionObject as Exception;
            if (ex == null)
                return;

            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }
    }
}
