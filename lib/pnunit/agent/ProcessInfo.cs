using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace Codice.Test
{
    public class ProcessInfo
    {
        public static bool IsProcessRunning(string procname, bool bFinish)
        {
            const int MAX_RETRIES = 100;
            int retries = 0;
            do
            {
                Thread.Sleep(100);
                IList procs = GetProcessesByName(procname);
                if (procs.Count > 0)
                {
                    if (bFinish)
                    {
                        KillProcess(procs);
                    }
                    return true;
                }
                retries++;

            } while (retries < MAX_RETRIES);

            return false;
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags,
            uint th32ProcessID);

        [DllImport("kernel32.dll")]
        private static extern int Process32First(IntPtr hSnapshot,
            ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        private static extern int Process32Next(IntPtr hSnapshot,
            ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool CloseHandle(IntPtr hSnapshot);

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)] public string szExeFile;
        }

        private const uint TH32CS_SNAPPROCESS = 0x00000002;

        public static Process LockDirectory(string path)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd";
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            return p;
        }

        public static void UnLockDirectory(Process dirLock)
        {
            dirLock.Kill();
            dirLock.WaitForExit(5000);
            dirLock.Close();
        }

        private static IList GetProcessesByName(string procname)
        {
            try
            {
                return  Process.GetProcessesByName(procname);
            }
            catch
            {
                ArrayList processIds=new ArrayList();
                if(procname.Length>15)
                {
                    procname= procname.Substring(0,15);
                }

                IntPtr handle = IntPtr.Zero;
                try
                {
                    // Create snapshot of the processes
                    handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                    PROCESSENTRY32 info = new PROCESSENTRY32();
                    info.dwSize = (uint)System.Runtime.InteropServices.
                        Marshal.SizeOf(typeof(PROCESSENTRY32));

                    // Get the first process
                    int first = Process32First(handle, ref info);

                    if (first == 0)
                    {
                        return processIds;
                    }

                    do
                    {
                        string strProcessName =System.IO.Path.GetFileNameWithoutExtension(info.szExeFile);
                        if(strProcessName.Length>15)
                        {
                            strProcessName=strProcessName.Substring(0,15);
                        }

                        if (string.Compare(strProcessName,
                            procname, true) == 0)
                        {
                            processIds.Add(info.th32ProcessID);
                        }

                    }
                    while (Process32Next(handle, ref info) != 0);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    CloseHandle(handle);
                    handle = IntPtr.Zero;
                }
                return processIds;
            }
        }

        private static void KillProcess (IList procs)
        {
            foreach (object proc in procs)
            {
                if (proc is Process)
                {
                    try
                    {
                        ((Process)proc).Kill();
                        ((Process)proc).WaitForExit(500);
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        //it may happen that the application has already finished
                    }
                }
                if (proc is uint)
                {
                    KillProcess ((uint) proc);
                }
            }
        }

        public static void CloseProcess(Process proc)
        {
            if (proc == null) return;
            int procId = -1;

            for(int retry = 0; retry < 5; retry++)
            {
                try
                {
                    proc.Refresh();
                    procId = proc.Id;
                    break;
                }
                catch(Exception)
                {
                    Thread.Sleep(100);
                }
            }

            try
            {
                proc.Refresh();
                if (proc.HasExited) return;
            }
            catch (Exception)
            {
                //Nothing to do
            }

            try
            {
                proc.Refresh();
                proc.CloseMainWindow();
                proc.Close();
            }
            catch(Exception)
            {
                //Nothing to do
            }

            if (procId == -1) return;


            for(int retry = 0; retry < 10; retry++)
            {
                try
                {
                    Process p = Process.GetProcessById(procId);
                    if(p == null) return;

                    p.Refresh();
                    p.Kill();
                    p.WaitForExit(500);
                    if (p.HasExited) return;
                }
                catch(ArgumentException)
                {
                    //This process is not running
                    return;
                }
                catch(Exception)
                {
                    Thread.Sleep(100);
                }
            }
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        public const uint PROCESS_KILLRIGHTS=0x00000001;

        private static bool KillProcess(uint uiProcessId)
        {
            System.IntPtr handler= OpenProcess(PROCESS_KILLRIGHTS,false,uiProcessId);
            bool b=TerminateProcess(handler, 0);
            CloseHandle(handler);
            return b;
        }

        public static void KillProcess(string[] procnames)
        {
            foreach (string procname in procnames)
            {
                IList procs = GetProcessesByName(procname);
                KillProcess(procs);
            }
        }

        public static List<string> UnixGetChildrenPids(string requestedPid)
        {
            string psOutput = string.Empty;
            List<string> childProcesses = new List<string>();

            ProcessStartInfo psi = new ProcessStartInfo("ps", "ax -o \"pid ppid\"");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            using (Process ps = Process.Start(psi))
            {
                ps.WaitForExit();
                psOutput = ps.StandardOutput.ReadToEnd();
            }

            StringReader sr = new StringReader(psOutput);
            string line = string.Empty;
            string listedPid, listedPPid;
            while ((line = sr.ReadLine()) != null)
            {
                if (!Regex.IsMatch(line, REGEX_PS_PID_PPID))
                    continue;

                Match m = Regex.Match(line, REGEX_PS_PID_PPID);
                listedPid = m.Groups["pid"].ToString();
                listedPPid = m.Groups["ppid"].ToString();

                if (!listedPPid.Equals(requestedPid))
                    continue;

                childProcesses.Add(listedPid);
            }

            return childProcesses;
        }

        const string REGEX_PS_PID_PPID = @"(\W*)(?<pid>\d+)(\W*)(?<ppid>\d+)(\W*)$";
    }
}
