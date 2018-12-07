using System;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.MacOS
{
    internal class ExceptionsHandler
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
            AppDomain.CurrentDomain.UnhandledException += HandleUndhandledException;
        }

        static void SetTestingExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleTestingUnhandledException;
        }

        static void HandleUndhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            // Here you'd usually log the exception, send telemetry events...
        }

        static void HandleTestingUnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            // We don't want to eat up the exception during testing like nothing
            // happens, but we still want to save it in order to display it
            // as the cause of the failed test.
            GuiTesteableServices.UnhandledException = (Exception)e.ExceptionObject;
        }
    }
}
