using System;
using System.Collections.Generic;
using System.IO;

using log4net;

using PNUnit.Launcher.Automation;

namespace PNUnit.Launcher.FileReport
{
    internal static class StatusReport
    {
        internal static void Write(
            string file, LauncherStatus status, bool bFinished)
        {
            string content = GetContent(status, bFinished);

            int retries = 0;
            while (true)
            {
                try
                {
                    File.WriteAllText(file, content);
                    return;
                }
                catch(Exception e)
                {
                    mLog.Info("The status file cannot be written. Error: " + e.Message);
                    if (retries++ > 10)
                        return;

                    System.Threading.Thread.Sleep(500);
                    continue;
                }
            }
        }

        static string GetContent(LauncherStatus status, bool bFinished)
        {
            return string.Join(FileReportConstants.FIELD_SEPARATOR, new string[] {
                status.TestCount.ToString(),
                status.TestToExecuteCount.ToString(),
                status.CurrentTestName,
                status.CurrentTest.ToString(),
                status.ExecutedTests.ToString(),
                string.Join(FileReportConstants.LIST_SEPARATOR, status.GetRepeatedTests().ToArray()),
                string.Join(FileReportConstants.LIST_SEPARATOR, status.GetFailedTests().ToArray()),
                string.Join(FileReportConstants.LIST_SEPARATOR, status.GetIgnoredTests().ToArray()),
                status.WarningMsg,
                bFinished.ToString()});
        }

        static readonly ILog mLog = LogManager.GetLogger("launcher");
    }
}
