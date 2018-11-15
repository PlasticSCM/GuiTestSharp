using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using log4net;

using PNUnit.Framework;

namespace PNUnit.Agent
{
    class ProcessPool
    {
        internal ProcessPool(
            string preloadTestRunners,
            string pathToAssemblies,
            bool bNoTimeout)
        {
            if (string.IsNullOrEmpty(preloadTestRunners))
                mbUsePreload = false;
            else
            {
                mbUsePreload = true;
                if (preloadTestRunners != string.Empty)
                {
                    int.TryParse(preloadTestRunners, out mPreloadCount);
                    mLog.DebugFormat("Preload count {0}", mPreloadCount);
                }
            }

            mPathToAssemblies = pathToAssemblies;
            mNoTimeout = bNoTimeout;
        }

        internal void RunProcess(
            PNUnitTestInfo testInfo,
            string testInfoPath)
        {
            Process pnunitTestRunnerProc = Run(testInfo, testInfoPath);

            Add(pnunitTestRunnerProc, testInfo);
        }

        internal bool IsTestRunning(string testName)
        {
            lock (mRunningTests)
            {
                foreach (RunningTest test in mRunningTests)
                {
                    if (test.TestInfo.TestName != testName)
                        continue;

                    if (test.TestProcess.HasExited)
                        return false;

                    return true;
                }
            }

            return false;
        }

        internal void Start()
        {
            Thread t = new Thread(new ThreadStart(BackgroundThread));
            t.Start();
        }

        internal void SetTimeout(int val)
        {
            mTestsTimeoutSecs = val;
        }

        internal void CleanPreviousRunningTests()
        {
            lock (mRunningTests)
            {
                if (mRunningTests.Count < 1)
                    return;

                foreach (RunningTest runningTest in mRunningTests)
                    SafeKillProcessTree(runningTest);

                mRunningTests.Clear();
            }
        }

        void SafeKillProcessTree(RunningTest runningTest)
        {
            if (runningTest == null)
                return;

            if (runningTest.TestProcess.HasExited)
                return;

            try
            {
                int ini = Environment.TickCount;

                string processPid = runningTest.TestProcess.Id.ToString();
                mLog.DebugFormat(
                    "SafeKillProcessTree: About to kill test with process tree PID [{0}] (Test name [{1}])",
                    processPid, runningTest.TestInfo.TestName);

                Codice.Test.OSProcessKillerCmd.KillProcessTree(processPid);

                PNUnitServices.Get().WriteLine(string.Format(
                    "Kill [{0}] took [{1} ms]",
                    runningTest.TestInfo.TestName,
                    Environment.TickCount - ini));
            }
            catch (Exception e)
            {
                mLog.ErrorFormat(
                    "SafeKillProcessTree: An error occurred trying " +
                    " to kill test process tree: {0}", e.Message);

                mLog.Debug(e.StackTrace);
            }
        }

        void Add(Process pnunitTestRunnerProc, PNUnitTestInfo testInfo)
        {
            lock (mRunningTests)
            {
                mRunningTests.Add(new RunningTest(pnunitTestRunnerProc, testInfo));
            }
        }

        Process Run(PNUnitTestInfo testInfo, string testInfoPath)
        {
            if (!mbUsePreload || !AssemblyPreload.CanUsePreload(testInfo.AssemblyName))
                return LaunchTestRunner(testInfo, testInfoPath);

            Process result = LaunchPreloaded(testInfoPath);

            if (result != null)
                return result;

            return LaunchTestRunner(testInfo, testInfoPath);
        }

        Process LaunchPreloaded(string testInfoPath)
        {
            Process result;

            lock (mPreloadedQueue)
            {
                if (mPreloadedQueue.Count == 0)
                {
                    return null;
                }

                result = mPreloadedQueue.Dequeue();
            }

            string testInfoFileForRunnerPid = Path.Combine(
                    Path.GetTempPath(),
                    AssemblyPreload.PRELOADED_PROCESS_FILE_PREFIX +
                    result.Id.ToString());

            File.Move(testInfoPath, testInfoFileForRunnerPid);

            return result;
        }

        Process LaunchTestRunner(PNUnitTestInfo testInfo, string testInfoPath)
        {
            string testingArgs = string.Format(
                "\"{0}\" \"{1}\"", testInfoPath, mPathToAssemblies);

            // You should customize this section depending on the applications
            // you've got and the platform they run on.
            Process result = null;

            if (testInfo.TestName.StartsWith("windows:"))
            {
                result = ProcessCreator.CreateGuiApplicationRunnerProcess(
                    testInfo, "windows.exe", testingArgs, mPathToAssemblies);
            }

            if (testInfo.TestName.StartsWith("linux:"))
            {
                result = ProcessCreator.CreateGuiApplicationRunnerProcess(
                    testInfo, "linux", testingArgs, mPathToAssemblies);
            }

            if (result == null)
            {
                result = ProcessCreator.CreateTestRunnerProc(testingArgs, mPathToAssemblies);
            }

            result.Start();
            return result;
        }

        Process PreloadTestRunner()
        {
            Process preload = ProcessCreator.CreateTestRunnerProc(
                string.Format("preload \"{0}\"", mPathToAssemblies),
                mPathToAssemblies);

            preload.Start();

            return preload;
        }

        void EnqueuePreloadedTestRunner()
        {
            Process p = PreloadTestRunner();

            lock (mPreloadedQueue)
            {
                mPreloadedQueue.Enqueue(p);
            }
        }

        void BackgroundThread()
        {
            const int MAX_ERRORS = 4;
            int errorCount = 0;

            while (true)
            {
                try
                {
                    PreloadedTestRunners();

                    CheckRunningTests();
                }
                catch (Exception e)
                {
                    mLog.ErrorFormat(
                        "An error occurred in the process pool background loop thread:{0}",
                        e.Message);

                    if (++errorCount == MAX_ERRORS)
                    {
                        mLog.Error(
                            "Process pool background loop thread: " +
                            "Max errors reached. Finishing thread...");

                        return;
                    }
                }

                Thread.Sleep(10000);
            }
        }

        void PreloadedTestRunners()
        {
            if (!mbUsePreload)
                return;

            int currentQueue;

            lock (mPreloadedQueue)
            {
                currentQueue = mPreloadedQueue.Count;
            }

            if (currentQueue >= mPreloadCount)
                return;

            int count = 0;
            int ini = Environment.TickCount;

            for (int i = currentQueue; i < mPreloadCount; ++i)
            {
                EnqueuePreloadedTestRunner();
                ++count;
            }

            mLog.InfoFormat("Preloaded {0} testrunners {1} ms",
                count, Environment.TickCount - ini);
        }

        void CheckRunningTests()
        {
            RunningTest timedOutTest = null;

            lock (mRunningTests)
            {
                for (int i = mRunningTests.Count - 1; i >= 0; --i)
                {
                    RunningTest current = mRunningTests[i];

                    int testRunningTimeSecs =
                        (int)(DateTime.Now - current.TestProcess.StartTime).TotalSeconds;

                    if (current.TestProcess.HasExited)
                    {
                        mRunningTests.RemoveAt(i);
                        continue;
                    }

                    if ((testRunningTimeSecs > mTestsTimeoutSecs) && !mNoTimeout)
                    {
                        timedOutTest = current;
                        break;
                    }
                }

                if (timedOutTest == null)
                    return;

                try
                {
                    SafeKillProcessTree(timedOutTest);

                    PNUnitAgent.NotifyError(
                        new Exception("Test killed due to timeout!"),
                        timedOutTest.TestInfo);

                    mLog.WarnFormat(
                        "Test killed due to timeout! [{0}]",
                        timedOutTest.TestInfo.TestName);
                }
                catch (Exception e)
                {
                    mLog.ErrorFormat(
                        "An error occurred killing processes. Error:[{0} - {1}]", e.Message, e.StackTrace);
                }
            }
        }

        void CleanGuiTestsBaseWkPath(string path)
        {
            if (path.StartsWith(".")) //if it's relative, get full-based-pnunit folder path
                path = Path.GetFullPath(Path.Combine(mPathToAssemblies, path));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }

            try
            {
                mLog.DebugFormat(
                    "CleanGuiTestsBaseWkPath: Deleting path:[{0}]", path);

                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                mLog.WarnFormat(
                    "CleanGuiTestsBaseWkPath: Unable to clean path:[{0}]. Reason:{1}",
                    path,
                    e.Message);
            }
        }

        internal class RunningTest
        {
            internal Process TestProcess;
            internal PNUnitTestInfo TestInfo;

            internal RunningTest(Process p, PNUnitTestInfo info)
            {
                TestProcess = p;
                TestInfo = info;
            }
        }

        static class ProcessCreator
        {
            internal static Process CreateGuiApplicationRunnerProcess(
                PNUnitTestInfo testInfo,
                string executableName,
                string testingArgs,
                string pathToAssemblies)
            {
                if (testInfo.TestParams.Length == 0)
                    throw new Exception("The test is wrongly defined!");

                string executablePath = Path.Combine(
                    pathToAssemblies, testInfo.TestParams[0]);

                executablePath = Path.GetFullPath(executablePath);

                string processName = Path.Combine(executablePath, executableName);

                string arguments = string.Concat(testInfo.TestParams[1], " ", testingArgs);

                mLog.DebugFormat(
                    "Running process [{0} {1}]", processName, arguments);

                Console.WriteLine(processName);
                Console.WriteLine(arguments);

                Process testRunnerProc = new Process();
                testRunnerProc.StartInfo.UseShellExecute = false;
                testRunnerProc.StartInfo.FileName = processName;
                testRunnerProc.StartInfo.CreateNoWindow = false;
                testRunnerProc.StartInfo.WorkingDirectory = pathToAssemblies;
                testRunnerProc.StartInfo.Arguments = arguments;

                return testRunnerProc;
            }

            internal static Process CreateTestRunnerProc(
                string args, string pathToAssemblies)
            {
                string processName = Path.Combine(
                    pathToAssemblies, PNUNIT_TEST_RUNNER_PROC_NAME);

                Process testRunnerProc = new Process();
                testRunnerProc.StartInfo.UseShellExecute = false;
                testRunnerProc.StartInfo.FileName = processName;
                testRunnerProc.StartInfo.CreateNoWindow = false;
                testRunnerProc.StartInfo.WorkingDirectory = pathToAssemblies;

                testRunnerProc.StartInfo.Arguments = args;

                return testRunnerProc;
            }
        }

        int mTestsTimeoutSecs = 5 * 60;
        bool mNoTimeout;
        string mPathToAssemblies;
        bool mbUsePreload = false;
        int mPreloadCount = 4;

        List<RunningTest> mRunningTests = new List<RunningTest>();
        Queue<Process> mPreloadedQueue = new Queue<Process>();

        static readonly ILog mLog = LogManager.GetLogger("ProcessPool");

        const string PNUNIT_TEST_RUNNER_PROC_NAME = "pnunittestrunner";
    }
}
