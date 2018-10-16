using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Core;
using NUnit.Core.Filters;
using NUnit.Util;

using PNUnit.Agent;
using PNUnit.Framework;

using log4net;

namespace PNUnitTestRunner
{
    public class NUnitTestRunner
    {
        public NUnitTestRunner(
            PNUnitTestInfo info,
            string pathToAssemblies)
        {
            mNUnitAssemblyPath = Path.Combine(pathToAssemblies, info.AssemblyName);
            mPNUnitTestInfo = info;
        }

        public void Run(IPNUnitServices services)
        {
            try
            {
                TestResult result = null;

                TestRunner testRunner = null;

                TestConsoleAccess consoleAccess = new TestConsoleAccess(mPNUnitTestInfo.OutputFile);

                TestLogInfo testLogInfo = new TestLogInfo();

                try
                {
                    mLog.DebugFormat("Running test assembly {0}",
                        mNUnitAssemblyPath);

                    ConsoleWriter outStream = new ConsoleWriter(Console.Out);
                    ConsoleWriter errorStream = new ConsoleWriter(Console.Error);

                    testRunner = SetupTest(services, consoleAccess, testLogInfo);

                    if (testRunner == null)
                        return;

                    mLog.Debug("Running tests");

                    try
                    {
                        PNUnitServices.Get().InitBarriers();
                        PNUnitServices.Get().EnterBarrier(PNUnitServices.Get().GetTestStartBarrier());

                        result = RunNUnitAssembly(
                            services,
                            outStream,
                            testRunner,
                            Path.GetFileNameWithoutExtension(mPNUnitTestInfo.AssemblyName));

                        PNUnitServices.Get().EnterBarrier(PNUnitServices.Get().GetTestEndBarrier());
                    }
                    catch (Exception e)
                    {
                        mLog.ErrorFormat("Error running test {0}. {1}", mPNUnitTestInfo.TestName, e.Message);
                        mLog.Debug(e.StackTrace);

                        result = PNUnitTestRunner.BuildError(
                            mPNUnitTestInfo, e, consoleAccess, testLogInfo);
                    }
                }
                finally
                {
                    mLog.Info("Notifying the results");

                    PNUnitTestResult pnunitResult = PNUnitTestRunner.BuildResult(
                        mPNUnitTestInfo,
                        result, consoleAccess, testLogInfo);

                    mLog.Debug("Result built!! Now, notify the launcher ...");

                    try
                    {
                        services.NotifyResult(
                            mPNUnitTestInfo.TestName, pnunitResult);
                        mLog.Debug("<-Results NOTIFIED");
                    }
                    catch (Exception e)
                    {
                        mLog.ErrorFormat(
                            "Error notifying back the result of test {0}. {1}",
                            mPNUnitTestInfo.TestName, e.Message);
                        mLog.Error(
                            "If you're using a custom launcher.remoting.conf"+
                            " check that you've defined the binaryFormatter");
                    }

                    result = null;

                    UnloadTests(testRunner);
                }
            }
            catch (Exception ex)
            {
                mLog.ErrorFormat("Error running test {0} {1}",
                    mPNUnitTestInfo.TestName, ex.Message);
                mLog.Debug(ex.StackTrace);
            }
        }

        TestResult RunNUnitAssembly(
            IPNUnitServices services,
            ConsoleWriter outStream,
            TestRunner testRunner,
            string assemblyName)
        {
            ITestFilter testFilter = TestFilter.Empty;

            string excludeFilterString = GetExcludeFilter();

            if (!string.IsNullOrEmpty(excludeFilterString))
            {
                testFilter = new NotFilter(
                    new CategoryExpression(excludeFilterString).Filter);
            }

            int testCount = testRunner.CountTestCases(testFilter);

            EventCollector collector =
                new EventCollector(outStream, testCount, services, assemblyName);

            TestResult result = testRunner.Run(
                collector,
                testFilter,
                true,
                LoggingThreshold.All);

            collector.NotifyProgress();

            return result;
        }
        string GetExcludeFilter()
        {
            if (mPNUnitTestInfo.TestParams == null)
                return null;

            if (mPNUnitTestInfo.TestParams.Length == 0)
                return null;

            return mPNUnitTestInfo.TestParams[0];
        }

        void UnloadTests(TestRunner runner)
        {
            lock(obj)
            {
                runner.Unload();
            }
        }

        TestRunner SetupTest(IPNUnitServices services, TestConsoleAccess consoleAccess, TestLogInfo testLogInfo)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(mNUnitAssemblyPath));

            TestPackage package = new TestPackage(Path.GetFileName(mNUnitAssemblyPath));

            TestRunner result = new SimpleTestRunner();

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            bool testLoaded = result.Load(package);

            if( !testLoaded )
            {
                mLog.InfoFormat("Unable to locate test {0}", mPNUnitTestInfo.TestName);
                PNUnitTestResult testResult = PNUnitTestRunner.BuildError(
                    mPNUnitTestInfo, new Exception("Unable to locate tests"), consoleAccess,
                    testLogInfo);

                services.NotifyResult(
                    mPNUnitTestInfo.TestName, testResult);

                return null;
            }

            InitPNUnitServices(
                mPNUnitTestInfo, result, services, consoleAccess, testLogInfo);

            return result;
        }

        static void InitPNUnitServices(
             PNUnitTestInfo testInfo,
             TestRunner testRunner,
             IPNUnitServices services,
             TestConsoleAccess consoleAccess,
             TestLogInfo testLogInfo)
        {
            // Intialize the singleton
            new PNUnitServices(testInfo, services, consoleAccess, testLogInfo);
        }

        Assembly CurrentDomain_AssemblyResolve(
            object sender, ResolveEventArgs args)
        {
            mLog.DebugFormat("Required assembly {0}.", args.Name);

            string[] asm = args.Name.Split(',');

            try
            {
                // we need a class loader to find nunitclient and nunitserver
                // it is not clean and nunit-console doesn't need it, so
                // there must be a way to fix this and don't require a
                // crappy Assembly Resolver :-(
                if (!IsPlasticNUnitTestDll(asm[0]))
                {
                    return null;
                }

                string appPath = Path.GetDirectoryName(mNUnitAssemblyPath);

                string asmname = Path.Combine(appPath, asm[0]);

                if (!asmname.EndsWith(".dll"))
                    asmname += ".dll";

                //Load the existing assembly
                Assembly result = Assembly.LoadFrom(asmname);

                if (result == null)
                    return result;

                string requestedVersion = GetVersion(asm);
                string loadedVersion = GetVersion(result.FullName.Split(','));

                if (!IsValidVersion(requestedVersion, loadedVersion))
                {
                    mLog.InfoFormat("The loaded version {0} of the assembly {1} is not the " +
                        "requested version {2}.", loadedVersion, asm[0], requestedVersion);
                    //Return null, so another resolve handler could load the right one
                    return null;
                }

                mLog.DebugFormat("Resolved assembly {0}", result.FullName);
                return result;
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("Error trying to load assembly {0}", e.Message);
                mLog.DebugFormat(e.StackTrace);
                throw;
            }
        }

        static bool IsValidVersion(string requestedVersion, string loadedVersion)
        {
            return string.IsNullOrEmpty(requestedVersion)
                || requestedVersion.Equals(loadedVersion);
        }

        static string GetVersion(string[] asmNameFields)
        {
            for (int i = 1; i < asmNameFields.Length; i++)
            {
                string field = asmNameFields[i].Trim();
                if (!field.StartsWith("Version=", StringComparison.OrdinalIgnoreCase))
                    continue;

                return field.Substring("Version=".Length);
            }

            return string.Empty;
        }

        bool IsPlasticNUnitTestDll(string asm)
        {
            string probePath = Path.Combine(
                Path.GetDirectoryName(mNUnitAssemblyPath), asm + ".dll");

            return File.Exists(probePath);
        }

        #region Nested Class to Handle Events

        [Serializable]
        private class EventCollector : EventListener
        {
            public EventCollector(
                ConsoleWriter writer,
                int testCount,
                IPNUnitServices services,
                string assemblyName)
            {
                mWriter = writer;
                mCurrentTestName = string.Empty;
                mTotalTestCount = testCount;
                mServices = services;
                mAssemblyName = assemblyName;
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
                mTestRunCount++;

                if (testResult.Executed)
                {
                    if (testResult.IsFailure || testResult.IsError)
                    {
                        mFailed.Add(mCurrentTestName);
                    }
                }
                else
                {
                    mIgnored.Add(mCurrentTestName);
                }

                string msg = string.Format(
                    "{0, -20} {1}/{2} tests (time:{3}s) [{4}]",
                    mAssemblyName,
                    mTestRunCount,
                    mTotalTestCount,
                    Math.Round(testResult.Time, 3),
                    testResult.Name);

                if (mFailed.Count > 0)
                {
                    msg += string.Format(" Failed {0}.", mFailed.Count);
                }

                if (mIgnored.Count > 0)
                {
                    msg += string.Format(" Ignored {0}", mIgnored.Count);
                }

                Console.WriteLine(msg);

                if (mTestRunCount % 10 == 0)
                {
                    mLog.DebugFormat("Notifying progress back to launcher {0}", msg);

                    NotifyProgress();
                }

                mCurrentTestName = string.Empty;
            }

            internal void NotifyProgress()
            {
                try
                {
                    mServices.NotifyNUnitProgress(
                        mTestRunCount, mTotalTestCount,
                        mFailed, mIgnored,
                        false);
                }
                catch (Exception e)
                {
                    mLog.ErrorFormat(
                        "Unable to notify back to the launcher {0}",
                        e.Message);
                    mLog.Debug(e.StackTrace);
                }
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
                string msg = string.Format("Unhandled Exception running {0}", mCurrentTestName);

                // If we do labels, we already have a newline
                //if ( !options.labels ) writer.WriteLine();
                mWriter.WriteLine(msg);
                mWriter.WriteLine(exception.ToString());
            }

            public void TestOutput(TestOutput output)
            {
            }

            int mTestRunCount;
            List<string> mIgnored = new List<string>();
            List<string> mFailed = new List<string>();
            int mTotalTestCount;
            string mAssemblyName;
            IPNUnitServices mServices;
            ConsoleWriter mWriter;
            string mCurrentTestName;
        }

        #endregion

        PNUnitTestInfo mPNUnitTestInfo;
        static readonly ILog mLog = LogManager.GetLogger("NUnitTestRunner");
        string mNUnitAssemblyPath;
        static object obj = new object();
    }
}