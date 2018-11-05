using System.Collections.Generic;
using System.Diagnostics;

using log4net;

namespace Codice.Test
{
    public static class OSProcessKillerCmd
    {
        public static void KillProcessTree(string pid)
        {
            if (PlatformIdentifier.IsWindows())
            {
                KillProcessForcedWindows(pid, true);
                return;
            }

            KillProcessTreeUnix(pid);
        }

        public static void KillProcess(string pid)
        {
            if (PlatformIdentifier.IsWindows())
            {
                KillProcessForcedWindows(pid, false);
                return;
            }

            KillProcessForcedUnix(pid);
        }

        static void KillProcessForcedWindows(string pid, bool bHasToKillProcessTree)
        {
            string processName = "taskkill";
            string killTreeArg = bHasToKillProcessTree ? "/T" : string.Empty;

            string args = string.Format("/F {0} /PID {1}", killTreeArg, pid);

            mLog.DebugFormat("executing [{0} {1}]", processName, args);

            using (Process taskKill = new Process())
            {
                taskKill.StartInfo.FileName = processName;
                taskKill.StartInfo.Arguments = args;
                taskKill.StartInfo.CreateNoWindow = true;
                taskKill.StartInfo.UseShellExecute = false;
                taskKill.Start();
                taskKill.WaitForExit(PROCESS_MAX_WAIT_EXIT_MILLIS);
            }
        }

        static void KillProcessTreeUnix(string pid)
        {
            List<string> pids = new List<string>();
            GetChildProcesses(pid, pids);

            string args = string.Join(" ", pids.ToArray());
            KillProcessForcedUnix(args);
        }

        static void KillProcessForcedUnix(string pidsArg)
        {
            mLog.DebugFormat("executing [{0} {1}]", "kill -9", pidsArg);

            using (Process kill = Process.Start("kill", "-9 " + pidsArg))
            {
                kill.WaitForExit(PROCESS_MAX_WAIT_EXIT_MILLIS);
            }
        }

        static void GetChildProcesses(string requestedPid, List<string> outChildPids)
        {
            outChildPids.Add(requestedPid);

            foreach (string childPid in ProcessInfo.UnixGetChildrenPids(requestedPid))
                GetChildProcesses(childPid, outChildPids);
        }

        const int PROCESS_MAX_WAIT_EXIT_MILLIS = 2000;

        static readonly ILog mLog = LogManager.GetLogger("ProcessKiller");
    }
}
