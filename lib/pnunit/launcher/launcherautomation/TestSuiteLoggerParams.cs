using System;
using System.Collections.Generic;
using System.Text;

namespace PNUnit.Launcher
{
    [Serializable]
    public class TestSuiteLoggerParams
    {
        public TestSuiteLoggerParams(
            string buildType,
            string buildName,
            int cset,
            string buildComment,
            string suiteType,
            string suiteName,
            string host,
            string vmachine,
            bool bLogSuccess)
        {
            mbInitialized = true;

            mBuildType = buildType;
            mBuildName = buildName;
            mCset = cset;
            mBuildComment = buildComment;
            mSuiteType = suiteType;
            mSuiteName = suiteName;
            mHost = host;
            mVMachine = vmachine;
            mbLogSuccess = bLogSuccess;
        }

        public TestSuiteLoggerParams()
        {
            mbInitialized = false;
        }

        public bool IsInitialized()
        {
            return mbInitialized;
        }

        public string BuildType { get { return mBuildType; } }
        public string BuildName { get { return mBuildName; } }
        public int Cset { get { return mCset; } }
        public string Comment { get { return mBuildComment; } }
        public string SuiteType { get { return mSuiteType; } }
        public string SuiteName { get { return mSuiteName; } }
        public string Host { get { return mHost; } }
        public string VMachine { get { return mVMachine; } }
        public bool LogSuccessfulTests { get { return mbLogSuccess; } }

        string mBuildType;
        string mBuildName;
        int mCset;
        string mBuildComment;
        string mSuiteType;
        string mSuiteName;
        string mHost;
        string mVMachine;
        bool mbLogSuccess;
        bool mbInitialized;
    }
}
