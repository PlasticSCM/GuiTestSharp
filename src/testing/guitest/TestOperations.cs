using System;
using System.IO;
using System.Reflection;

using log4net;

using NUnit.Framework;
using PNUnit.Framework;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace GuiTest
{
    // This delegate's parameters will be the way to pass down parameters to
    // your test.
    // For example, if your application has to connect to a remote server
    // unknown until testing, the server's address would be a parameter of this
    // delegate.
    public delegate void ExecuteTestDelegate(string testName);

    internal static class TestOperations
    {
        internal static void RunTest(
            string methodTestname, ExecuteTestDelegate testDelegate)
        {
            RunGuitest(methodTestname, testDelegate);

            // In this example we do not throw any unhandled exception, but in
            // the case the application does, it would end up being handled here.
            Exception unhandledException = GuiTesteableServices.UnhandledException;

            if (unhandledException != null)
            {
                Assert.Fail(
                    "The test finished with an unhandled exception: {1}{0}{2}",
                    Environment.NewLine,
                    unhandledException.Message,
                    unhandledException.StackTrace);
            }

            CheckUnexpectedMessages();
        }

        internal static void RunGuitest(
            string methodTestName, ExecuteTestDelegate testDelegate)
        {
            mRunTestLog.Info("0 stage");

            // Here you can prepare some options for the test,
            // setup configuration files or clean old ones to make sure the
            // test runs in a reproducible scenario.

            RunTest(methodTestName, testDelegate, GetGuiTestExecutablePath());
        }

        static void CheckUnexpectedMessages()
        {
            ITesteableErrorDialog errorDialog = GuiTesteableServices.GetErrorDialog();

            if (errorDialog != null)
            {
                Assert.Fail(
                    "The test finished with an unexpected error dialog still showing up: {0}",
                    errorDialog.GetText());
            }

            // Here you would check for the rest of possible dialogs your
            // application can show.
        }

        static string GetGuiTestExecutablePath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        static void RunTest(
            string methodTestname,
            ExecuteTestDelegate testDelegate,
            string binariesPath)
        {
            mRunTestLog.Info("1 stage");
            InitLog(methodTestname);

            string testName = PNUnitServices.Get().GetTestName();
            try
            {
                mRunTestLog.Info("2 stage");
                PNUnitServices.Get().SetInfoOSVersion(GetOSVersion());

                mRunTestLog.Info("Before GetTestParams");
                string[] testParams = PNUnitServices.Get().GetTestParams();

                mRunTestLog.Info("Before InitBarriers");
                PNUnitServices.Get().InitBarriers();

                mRunTestLog.Info("3 stage");
                EnterBarrier(PNUnitServices.Get().GetTestStartBarrier());

                mRunTestLog.Info("4 stage");

                if (PlatformUtils.CurrentPlatform == PlatformUtils.Platform.Linux)
                {
                    CheckMonoLibraries(binariesPath);
                    CheckMonoLibraries(Assembly.GetExecutingAssembly().Location);
                }

                mRunTestLog.Info("5 stage");

                string actualTestName = GetCleanTestName(testName);

                testDelegate?.Invoke(GetCleanTestName(testName));
            }
            catch (Exception ex)
            {
                LogTestException(methodTestname, testName, ex);
                throw;
            }
            finally
            {
                EnterBarrier(PNUnitServices.Get().GetTestEndBarrier());
            }
        }

        static void InitLog(string testName)
        {
            ThreadContext.Properties["TestName"] = testName;
        }

        static void EnterBarrier(string barrierName)
        {
            mBarrierLog.DebugFormat(
                ">>>Test {0} entering barrier {1}",
            PNUnitServices.Get().GetTestName(), barrierName);

            PNUnitServices.Get().EnterBarrier(barrierName);

            mBarrierLog.DebugFormat(
                "<<<Test {0} leavinf barrier {1}",
                PNUnitServices.Get().GetTestName(), barrierName);
        }

        static void LogTestException(
            string methodTestName, string testName, Exception ex)
        {
            mRunTestLog.ErrorFormat(
                "{1} {2} FAILED with exception '{3}':{0}{4}",
                Environment.NewLine,
                methodTestName,
                testName,
                ex.Message,
                ex.StackTrace);

            PNUnitServices.Get().WriteLine("Test {0} failed", methodTestName);
            PNUnitServices.Get().WriteLine("Message: {0}", ex.Message);
            PNUnitServices.Get().WriteLine("Stack trace: {0}", ex.StackTrace);
        }

        static string GetOSVersion()
        {
            return string.Format("{0} ({1})",
                OSVersionName.GetOSVersionName(),
                OSVersionName.GetOSArchitectureType());
        }

        static void CheckMonoLibraries(string binariesPath)
        {
            string monoPosixPath = Path.Combine(binariesPath, "Mono.Posix.dll");

            if (!File.Exists(monoPosixPath))
                return;

            throw new Exception(string.Format(
                "No Mono.*.dll should be inside the {0} directory.",
                binariesPath));
        }

        static string GetCleanTestName(string testName)
        {
            int separatorIndex = testName.IndexOf(':');
            if (separatorIndex < 0)
                return testName;

            return testName.Substring(separatorIndex + 1);
        }

        static readonly ILog mRunTestLog = LogManager.GetLogger("RunTest");
        static readonly ILog mBarrierLog = LogManager.GetLogger("Barriers");
    }
}
