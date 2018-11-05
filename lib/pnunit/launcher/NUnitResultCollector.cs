using System;
using System.Collections;

using NUnit.Core;
using NUnit.Util;


namespace PNUnit.Launcher
{
    class NUnitResultCollector
    {
        private ArrayList mResultList = new ArrayList();

        internal void AddResults(TestResult[] results)
        {
            mResultList.AddRange(results);
        }

        internal void SaveResults(string fileName)
        {
            TestResult all = new TestResult(new TestName());

            foreach (TestResult r in mResultList) all.AddResult(r);

            new XmlResultWriter(fileName).SaveTestResult(all);
        }
    }
}
