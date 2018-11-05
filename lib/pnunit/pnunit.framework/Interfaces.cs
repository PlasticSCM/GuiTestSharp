using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using NUnit.Core;

namespace PNUnit.Framework
{
    public class Names
    {
        public const string PNUnitAgentServiceName = "IPNUnitAgent";
        public const string ServerBarrier = "SERVERSTART";
        public const string EndBarrier = "ENDBARRIER";
        public const string RestartBarrier = "SERVERRESTART";
        public const string RestartedOkBarrier = "SERVERRESTARTEDOK";
        public const string DownloadRelease = "DOWNLOADRELEASE";
    }

    public interface ITestConsoleAccess
    {
        void WriteLine(string s);
        void Write(char[] buf);
        void Clear();
    }

    public interface ITestLogInfo
    {
        void SetOSVersion(string s);
        void SetBackendType(string s);
    }

    public interface IPNUnitServices
    {
        void NotifyResult(string testName, PNUnitTestResult result);

        void InitBarriers(string testName);

        void InitBarrier(string testName, string barrierName, int max);

        void EnterBarrier(string testName, string barrier);

        void SendMessage(string tag, int receivers, object message);

        object ReceiveMessage(string tag);

        void ISendMessage(string tag, int receivers, object message);

        object IReceiveMessage(string tag);

        void NotifyNUnitProgress(
            int runTestsCount, int totalTestCount,
            List<string> failed, List<string> ignored, bool bFinished);
    }

    [Serializable]
    public class PNUnitTestInfo
    {
        public string TestName;
        public string AssemblyName;
        public string TestToRun;
        public string[] TestParams;
        public string StartBarrier;
        public string EndBarrier;
        public string[] WaitBarriers;
        public Hashtable UserValues = new Hashtable();
        public string OutputFile;
        public string PNUnitServicesServer;

        public PNUnitTestInfo(
            string TestName, string AssemblyName,
            string TestToRun, string[] TestParams,
            string StartBarrier, string EndBarrier, string[] WaitBarriers,
            string PnunitServicesServer)
        {
            this.TestName = TestName;
            this.AssemblyName = AssemblyName;
            this.TestToRun = TestToRun;
            this.TestParams = TestParams;
            this.StartBarrier = StartBarrier;
            this.EndBarrier = EndBarrier;
            this.WaitBarriers = WaitBarriers;
            this.PNUnitServicesServer = PnunitServicesServer;
        }

        public string GetTestOutput()
        {
            string result = string.Empty;

            if (!File.Exists(OutputFile))
                return result;

            using (StreamReader sr = new StreamReader(OutputFile))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        public void DeleteTestOutput()
        {
            if (!File.Exists(OutputFile))
                return;

            File.Delete(OutputFile);
        }
    }

    public interface IPNUnitAgent
    {
        void SetTestsTimeout(int value);
        void RunTest(PNUnitTestInfo info);
        bool IsTestRunning(string testName);
        bool IsUp();
        void CleanPreviousRunningTests();
    }

    [Serializable]
    public class PNUnitRetryException : Exception
    {
        public static string RETRY_EXCEPTION = "RETRY_EXCEPTION:";
        #region "constructors"
        public PNUnitRetryException(string message)
            : base(RETRY_EXCEPTION + message)
        {
        }

        public PNUnitRetryException(string message, Exception innerException)
            : base(RETRY_EXCEPTION + message, innerException)
        {
        }

        public PNUnitRetryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }

    [Serializable]
    public class PNUnitTestResult : TestResult
    {
        private string mOutput = string.Empty;
        private bool mRetryTest = false;

        private string mOSVersion = string.Empty;
        private string mBackendType = string.Empty;

        private bool bKilled = false;

        public PNUnitTestResult(
            TestName testName,
            string output,
            string osversion,
            string backendtype,
            bool killed)
            : this(testName, output, osversion, backendtype)
        {
            bKilled = killed;
        }

        public PNUnitTestResult(
            TestName testName,
            string output,
            string osversion,
            string backendtype)
            : base(testName)
        {
            mOutput = output;
            mOSVersion = osversion;
            mBackendType = backendtype;
        }

        public PNUnitTestResult(
            TestResult testResult,
            string output,
            string osversion,
            string backendtype) : base(testResult.Test)
        {
            mOSVersion = osversion;
            mBackendType = backendtype;

            mOutput = output;
            if (testResult.Message != null &&
                (testResult.Message.IndexOf(PNUnitRetryException.RETRY_EXCEPTION) >= 0))
                this.mRetryTest = true;

            if (testResult.Executed)
            {
                if (testResult.IsSuccess)
                    this.Success(testResult.Message);
                else
                    this.Failure(testResult.Message, testResult.StackTrace);
            }
            else
            {
                //ignored test
                this.Ignore(testResult.Message);
            }

            if (testResult.Results != null)
            {
                foreach (TestResult child in testResult.Results)
                {
                    this.AddResult(child);
                }
            }

            this.Time = testResult.Time;
        }

        public bool Killed { get { return bKilled; } }
        public string Output { get { return mOutput; } }
        public bool RetryTest { get { return mRetryTest; } }
        public string OSVersion { get { return mOSVersion; } }
        public string BackendType { get { return mBackendType; } }
    }


}

