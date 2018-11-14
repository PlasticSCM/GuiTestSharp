using System;

using log4net;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.Linux
{
    internal static class ExceptionsHandler
    {
        internal static void SetExceptionHandlers(bool isTestingMode)
        {
            if (isTestingMode)
            {
                SetTestingExceptionHandlers();
                return;
            }

            SetStandardExceptionHandlers();
        }

        static void SetStandardExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleTestingUnhandledException;
            GLib.ExceptionManager.UnhandledException += HandleUnhandledGlibTestingException;
        }

        static void SetTestingExceptionHandlers()
        {
            // We don't want to eat up the exception during testing like nothing
            // happens, but we still want to save it in order to display it
            // as the cause of the failed test.
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            GLib.ExceptionManager.UnhandledException += HandleUnhandledGlibException;
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Here you'd usually log the exception, send telemetry events...
            Exception ex = e.ExceptionObject as Exception;
            if (ex == null)
                return;

            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }

        static void HandleUnhandledGlibException(GLib.UnhandledExceptionArgs e)
        {
            // Here you'd usually log the exception, send telemetry events...
            mLog.ErrorFormat("The runtime is terminating [{0}]", e.IsTerminating);
            e.ExitApplication = false;

            Exception ex = e.ExceptionObject as Exception;
            if (ex == null)
                return;

            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }

        static void HandleTestingUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleTestingException(e.ExceptionObject as Exception);
        }

        static void HandleUnhandledGlibTestingException(GLib.UnhandledExceptionArgs e)
        {
            HandleTestingException(e.ExceptionObject as Exception);

            mLog.ErrorFormat("The runtime is terminating [{0}]", e.IsTerminating);
            e.ExitApplication = false;
        }

        static void HandleTestingException(Exception ex)
        {
            GuiTesteableServices.UnhandledException = ex;
        }

        static readonly ILog mLog = LogManager.GetLogger("ExceptionsHandler");
    }
}
