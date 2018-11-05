using System;
using System.Threading;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.Windows
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
            Application.ThreadException += HandleThreadException;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        }

        static void SetTestingExceptionHandlers()
        {
            // We don't want to eat up the exception during testing like nothing
            // happens, but we still want to save it in order to display it
            // as the cause of the failed test.
            Application.ThreadException += HandleTestingThreadException;
            AppDomain.CurrentDomain.UnhandledException += HandleTestingUnhandledException;
        }

        static void HandleThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // Here you'd usually log the exception, send telemetry events...
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Here you'd usually log the exception, send telemetry events...
        }

        static void HandleTestingThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleTestingException(e.Exception);
        }

        static void HandleTestingUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleTestingException((Exception)e.ExceptionObject);
        }

        static void HandleTestingException(Exception ex)
        {
            GuiTesteableServices.UnhandledException = ex;
        }
    }
}
