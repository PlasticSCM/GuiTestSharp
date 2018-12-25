using System;
using System.Runtime.InteropServices;
using System.Text;

#if !NETCORE
using log4net;
#endif

namespace Codice.Examples.GuiTesting.Linux
{
    internal class ProcessNameSetter
    {
        [DllImport("libc")] // Linux
        static extern int prctl(
            int option, byte[] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

        [DllImport("libc")] // BSD
        static extern int setproctitle(byte[] fmt, byte[] str_arg);

        internal static void SetProcessName(string name)
        {
            if (IsWindows())
                return;

            try
            {
                if (PrctlSetName(name) != 0)
                {
#if NETCORE
                    Console.Error.WriteLine("Error setting process name");
#else
                    mLog.Debug("Error setting process name");
#endif
                }
            }
            catch (EntryPointNotFoundException)
            {
                try
                {
                    setproctitle(
                        Encoding.ASCII.GetBytes("%s\0"),
                        Encoding.ASCII.GetBytes(name + "\0"));
                }
                catch (Exception ex)
                {
#if NETCORE
                    Console.Error.WriteLine($"Couldn't change process name: {ex.Message}");
#else
                    mLog.DebugFormat(
                        "Couldn't change process name: {0}", ex.Message);
#endif
                }
            }
        }

        static int PrctlSetName(string name)
        {
            return prctl(
                (int)OPTION.PR_SET_NAME,
                Encoding.ASCII.GetBytes(name + "\0"),
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        enum OPTION
        {
            PR_SET_NAME = 15
        }

        private static bool IsWindows()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                    return true;
                default:
                    return false;
            }
        }

#if !NETCORE
        static readonly ILog mLog = LogManager.GetLogger("ProcessNameSetter");
#endif
    }
}
