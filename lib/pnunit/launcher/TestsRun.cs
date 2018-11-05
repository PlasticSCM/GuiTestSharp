using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Core;

using PNUnit.Framework;

namespace PNUnit.Launcher
{
    internal class TestsRun
    {
        internal bool PendingTestsToFinish()
        {
            lock (mResultLock)
            {
                return (mLaunchedTests > 0) && (mResults.Count < mLaunchedTests);
            }
        }

        internal bool IsTestExecuted(string name)
        {
            lock (mResultLock)
            {
                return mExecutedTests.Contains(name);
            }
        }

        internal TestResult[] GetTestResults()
        {
            lock (mResultLock)
            {
                TestResult[] result = new TestResult[mResults.Count];
                int i = 0;
                foreach (TestResult res in mResults)
                    result[i++] = res;

                return result;
            }
        }

        internal void IncrementTestsLaunched()
        {
            lock (mResultLock)
            {
                ++mLaunchedTests;
            }
        }

        internal void DecrementTestsLaunched()
        {
            lock (mResultLock)
            {
                --mLaunchedTests;
            }
        }

        internal int LaunchedCount
        {
            get
            {
                lock (mResultLock)
                {
                    return mLaunchedTests;
                }
            }
        }

        internal void AddExecutedTest(string testName)
        {
            lock (mResultLock)
            {
                mExecutedTests.Add(testName);
            }
        }

        internal void AddTestResult(PNUnitTestResult testResult)
        {
            lock (mResultLock)
            {
                mResults.Add(testResult);
            }
        }

        internal bool AllTestsFinished()
        {
            lock (mResultLock)
            {
                return mResults.Count == mLaunchedTests;
            }
        }

        internal int TestsResultsCount
        {
            get
            {
                lock (mResultLock)
                {
                    return mResults.Count;
                }
            }
        }

        Object mResultLock = new Object();
        int mLaunchedTests = 0;
        List<string> mExecutedTests = new List<string>();
        List<PNUnitTestResult> mResults = new List<PNUnitTestResult>();
    }
}