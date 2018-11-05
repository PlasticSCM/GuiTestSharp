using System;
using System.IO;

using PNUnit.Framework;

namespace PNUnit.Agent
{
    public class TestLogInfo : MarshalByRefObject, ITestLogInfo
    {
        public string OSVersion = string.Empty;
        public string BackendType = string.Empty;

        public void SetOSVersion(string s)
        {
            OSVersion = s;
        }

        public void SetBackendType(string s)
        {
            BackendType = s;
        }
    }

    public class TestConsoleAccess : MarshalByRefObject, ITestConsoleAccess
    {
        public TestConsoleAccess(string outputFile)
        {
            mOutputFilePath = outputFile;
        }

        public void WriteLine(string s)
        {
            if (mStoreOutputEnabled)
                AppendOutputTest(s);

            if(mConsoleEnabled)
                Console.WriteLine(s);
        }

        public void Write(char[] buf)
        {
            if (mStoreOutputEnabled)
                AppendOutputTest(buf);

            if (mConsoleEnabled)
                Console.Write(buf);
        }

        public void Clear()
        {
            using (StreamWriter sw = new StreamWriter(mOutputFilePath, false))
            {
                sw.WriteLine();
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        internal static void EnableConsole()
        {
            mConsoleEnabled = true;
        }

        internal static void DisableConsole()
        {
            mConsoleEnabled = false;
        }

        internal static void EnableStoreOutput()
        {
            mStoreOutputEnabled = true;
        }

        internal static void DisableStoreOutput()
        {
            mStoreOutputEnabled = false;
        }

        private void AppendOutputTest(string text)
        {
            using (StreamWriter sw = new StreamWriter(mOutputFilePath, true))
            {
                sw.WriteLine(text);
            }
        }

        private void AppendOutputTest(char[] buf)
        {
            using (StreamWriter sw = new StreamWriter(mOutputFilePath, true))
            {
                sw.WriteLine(buf);
            }
        }

        string mOutputFilePath;
        static bool mConsoleEnabled = true;
        static bool mStoreOutputEnabled = true;
    }
}