using System;
using System.IO;

using NUnit.Core;
using NUnit.Util;

using PNUnit.Agent;
using PNUnit.Framework;

using log4net;

namespace PNUnitTestRunner
{
    public class PNUnitTestRunner
    {
        public PNUnitTestRunner(string pathToAssemblies)
        {
            mPathToAssemblies = pathToAssemblies;
        }

        public void Run(
            PNUnitTestInfo testInfo,
            IPNUnitServices services,
            ITestConsoleAccess extraConsoleAccess)
        {
            try
            {
                TestResult result = null;

                TestRunner testRunner = null;

                TestConsoleAccess consoleAccess = new TestConsoleAccess(testInfo.OutputFile);

                TestLogInfo testLogInfo = new TestLogInfo();

                try
                {
                    mLog.DebugFormat("Running test {0}:{1} Assembly {2}",
                        testInfo.TestName,
                        testInfo.TestToRun,
                        testInfo.AssemblyName);

                    ConsoleWriter outStream = new ConsoleWriter(Console.Out);
                    ConsoleWriter errorStream = new ConsoleWriter(Console.Error);

                    testRunner = SetupTest(testInfo, services, consoleAccess,
                        testLogInfo, extraConsoleAccess);

                    if (testRunner == null)
                        return;

                    mLog.DebugFormat("RunTest {0}", testInfo.TestName);

                    PrintSeparator();

                    try
                    {
                        result = RunTest(testInfo, outStream, testRunner);
                    }
                    catch (Exception e)
                    {
                        mLog.ErrorFormat("Error running test {0}. {1}", testInfo.TestName, e.Message);
                        mLog.Debug(e.StackTrace);

                        result = BuildError(testInfo, e, consoleAccess, testLogInfo);
                    }
                }
                catch (Exception e)
                {
                    mLog.ErrorFormat("Error launching tests: {0}", e.Message);
                    mLog.Debug(e.StackTrace);

                    result = BuildError(testInfo,
                        e, consoleAccess, testLogInfo);
                }
                finally
                {
                    mLog.InfoFormat("Notifying the results {0}",
                        testInfo.TestName);

                    PNUnitTestResult pnunitResult = BuildResult(
                        testInfo,
                        result, consoleAccess, testLogInfo);

                    mLog.DebugFormat("Result built!! Now, notify the launcher ... {0}",
                        testInfo.TestName);

                    try
                    {
                        services.NotifyResult(
                            testInfo.TestName, pnunitResult);
                        mLog.DebugFormat("<-Results NOTIFIED - {0}",
                            testInfo.TestName);
                    }
                    catch (Exception e)
                    {
                        mLog.ErrorFormat(
                            "Error notifying back the result of test {0}. {1}",
                            testInfo.TestName, e.Message);
                        mLog.Error(
                            "If you're using a custom launcher.remoting.conf" +
                            " check that you've defined the binaryFormatter");
                    }

                    result = null;

                    UnloadTests(testRunner);
                }
            }
            catch (Exception ex)
            {
                mLog.ErrorFormat("Error running test {0} {1}",
                    testInfo.TestName, ex.Message);
                mLog.Debug(ex.StackTrace);
            }
        }

        void PrintSeparator()
        {
            try
            {
                int repeat = Console.WindowWidth > 10 ? Console.WindowWidth - 2 : 80;
                PNUnitServices.Get().WriteLine(new string('-', repeat));
            }
            catch (IOException e)
            {
                mLog.WarnFormat("Cannot access to Console:[{0}]", e.Message);
            }
        }

        internal void Preload()
        {
            mPreloader = new PreloadedTestRunner();

            mPreloader.TestRunner = new SimpleTestRunner();

            int ini = Environment.TickCount;

            string fullAssemblyPath = Path.GetFullPath(
                Path.Combine(mPathToAssemblies, AssemblyPreload.PRELOADED_ASSEMBLY));

            mPreloader.TestAssemblyLoaded = MakeTest(
                mPreloader.TestRunner, fullAssemblyPath);

            mLog.DebugFormat("Preloader load test assembly {0} ms", Environment.TickCount - ini);
        }

        TestResult RunTest(
            PNUnitTestInfo testInfo,
            ConsoleWriter outStream,
            TestRunner testRunner)
        {
            EventListener collector = new EventCollector(outStream);

            ITestFilter filter = new NUnit.Core.Filters.SimpleNameFilter(
                testInfo.TestToRun);

            TestResult result = testRunner.Run(
                collector, filter, true, LoggingThreshold.All);

            result = FindResult(testInfo.TestToRun, result);

            return result;
        }

        void UnloadTests(TestRunner runner)
        {
            lock (obj)
            {
                runner.Unload();
            }
        }

        TestRunner SetupTest(
            PNUnitTestInfo testInfo,
            IPNUnitServices services,
            TestConsoleAccess consoleAccess,
            TestLogInfo testLogInfo,
            ITestConsoleAccess extraConsoleAccess)
        {
            TestRunner result;
            bool testAssemblyLoaded;

            if (mPreloader == null)
            {
                result = new SimpleTestRunner();

                int ini = Environment.TickCount;

                string fullAssemblyPath = Path.GetFullPath(
                    Path.Combine(mPathToAssemblies, testInfo.AssemblyName));

                testAssemblyLoaded = MakeTest(result, fullAssemblyPath);

                mLog.DebugFormat("Load test assembly {0} ms", Environment.TickCount - ini);
            }
            else
            {
                //if (!AssemblyPreload.CanUsePreload(testInfo.AssemblyName))
                //{
                //    throw new Exception(
                //        "The preloaded and the target assembly don't match!!");
                //}

                result = mPreloader.TestRunner;
                testAssemblyLoaded = mPreloader.TestAssemblyLoaded;
            }

            if (!testAssemblyLoaded)
            {
                mLog.InfoFormat("Unable to load test assembly {0} for test {1}",
                    testInfo.AssemblyName,
                    testInfo.TestName);

                PNUnitTestResult testResult = BuildError(
                    testInfo, new Exception("Unable to locate tests"),
                    consoleAccess, testLogInfo);

                services.NotifyResult(
                    testInfo.TestName, testResult);

                return null;
            }

            mLog.Debug("Test loaded, going to set CurrentDirectory");

            Directory.SetCurrentDirectory(mPathToAssemblies);

            mLog.Debug("Creating PNUnit services");

            CreatePNUnitServices(testInfo, result, services, consoleAccess,
                testLogInfo, extraConsoleAccess);

            return result;
        }

        internal static PNUnitTestResult BuildResult(
            PNUnitTestInfo pnunittestinfo,
            TestResult result,
            TestConsoleAccess consoleAccess,
            TestLogInfo testLogInfo)
        {
            string output = pnunittestinfo.GetTestOutput();

            pnunittestinfo.DeleteTestOutput();

            mLog.Debug("Going to build the result ...");

            if (result == null)
            {
                mLog.Debug("Going to build an error result ...");
                TestName testName = new TestName();
                testName.Name = pnunittestinfo.TestName;
                testName.FullName = pnunittestinfo.TestName;
                testName.TestID = new TestID();

                string errorMsg = string.Format(
                    "The test {0} couldn't be found in the assembly {1}",
                     pnunittestinfo.TestToRun,
                     pnunittestinfo.AssemblyName);

                PNUnitTestResult testResult = new PNUnitTestResult(
                    testName,
                    output,
                    testLogInfo.OSVersion,
                    testLogInfo.BackendType);

                testResult.Failure(errorMsg, string.Empty);
                return testResult;
            }

            PNUnitTestResult myResult = new PNUnitTestResult(result, output,
                testLogInfo.OSVersion, testLogInfo.BackendType);

            return myResult;
        }

        internal static PNUnitTestResult BuildError(
            PNUnitTestInfo pnunitTestInfo,
            Exception e,
            TestConsoleAccess consoleAccess,
            TestLogInfo testLogInfo)
        {
            TestName testName = new TestName();
            testName.Name = pnunitTestInfo.TestName;
            testName.FullName = pnunitTestInfo.TestName;
            testName.TestID = new TestID();

            PNUnitTestResult result = new PNUnitTestResult(
                testName,
                pnunitTestInfo.GetTestOutput(),
                testLogInfo.OSVersion,
                testLogInfo.BackendType);

            string fullMessage = e.Message + "; STACK TRACE: " + e.StackTrace;
            result.Failure(fullMessage, string.Empty);

            pnunitTestInfo.DeleteTestOutput();
            return result;
        }

        static void CreatePNUnitServices(
            PNUnitTestInfo testInfo,
            TestRunner testRunner,
            IPNUnitServices services,
            TestConsoleAccess consoleAccess,
            TestLogInfo testLogInfo,
            ITestConsoleAccess extraConsoleAccess)
        {
            // Intialize the singleton
            new PNUnitServices(testInfo, services, consoleAccess, testLogInfo);

            if (extraConsoleAccess != null)
                PNUnitServices.Get().RegisterConsole(extraConsoleAccess);
        }

        static TestResult FindResult(string name, TestResult result)
        {
            if (result.Test.TestName.FullName == name)
                return result;

            if (result.HasResults)
            {
                foreach (TestResult r in result.Results)
                {
                    TestResult myResult = FindResult(name, r);
                    if (myResult != null)
                        return myResult;
                }
            }

            return null;
        }

        bool MakeTest(TestRunner runner, string fullAssemblyPath)
        {
            return runner.Load(new TestPackage(fullAssemblyPath));
        }

        #region Nested Class to Handle Events

        [Serializable]
        private class EventCollector : EventListener
        {
            private int testRunCount;
            private int testIgnoreCount;
            private int failureCount;

            private ConsoleWriter mWriter;

            private string mCurrentTestName;

            public EventCollector(ConsoleWriter writer)
            {
                mWriter = writer;
                mCurrentTestName = string.Empty;
            }

            public void RunStarted(Test[] tests)
            {
            }

            public void RunStarted(string a, int b)
            {
            }

            public void RunFinished(TestResult[] results)
            {
            }

            public void RunFinished(Exception exception)
            {
            }

            public void RunFinished(TestResult result)
            {
            }

            public void TestFinished(TestResult testResult)
            {
                if (testResult.Executed)
                {
                    testRunCount++;

                    if (testResult.IsFailure)
                    {
                        failureCount++;
                    }
                }
                else
                {
                    testIgnoreCount++;
                }

                mCurrentTestName = string.Empty;
            }

            public void TestStarted(TestMethod testCase)
            {
                mCurrentTestName = testCase.TestName.FullName;
            }

            public void TestStarted(TestName testName)
            {
                mCurrentTestName = testName.FullName;
            }

            public void SuiteStarted(TestName name)
            {
            }

            public void SuiteFinished(TestResult suiteResult)
            {
            }

            public void UnhandledException(Exception exception)
            {
                string msg = string.Format("##### Unhandled Exception while running {0}", mCurrentTestName);

                // If we do labels, we already have a newline
                //if ( !options.labels ) writer.WriteLine();
                mWriter.WriteLine(msg);
                mWriter.WriteLine(exception.ToString());
            }

            public void TestOutput(TestOutput output)
            {
            }
        }

        #endregion
        class PreloadedTestRunner
        {
            internal TestRunner TestRunner;
            internal bool TestAssemblyLoaded;
        }

        static readonly ILog mLog = LogManager.GetLogger("TestRunner");
        string mPathToAssemblies;
        static object obj = new object();
        PreloadedTestRunner mPreloader = null;
    }
}