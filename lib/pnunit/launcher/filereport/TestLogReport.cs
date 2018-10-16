using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PNUnit.Launcher.FileReport
{
    internal static class TestLogReport
    {
        internal static void LogTest(
            string file,
            TestSuiteEntry.Data entry,
            string testName,
            bool isRepeated)
        {
            string contents = string.Join(FileReportConstants.TEST_FIELD_SEPARATOR, new string[] {
                testName,
                entry.BackendType,
                entry.ClientConfig,
                entry.ServerConfig,
                entry.ExecTime.ToString(),
                entry.Status,
                entry.Log,
                isRepeated.ToString()});

            File.AppendAllText(file, contents + FileReportConstants.TEST_SEPARATOR);
        }
    }
}
