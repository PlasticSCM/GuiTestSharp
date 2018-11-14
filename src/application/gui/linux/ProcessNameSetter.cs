using System;
using System.Runtime.InteropServices;
using System.Text;

using log4net;

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
            try
            {
                if (PrctlSetName(name) != 0)
                    mLog.Debug("Error setting process name");
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
                    mLog.DebugFormat(
                        "Couldn't change process name: {0}", ex.Message);
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

        static readonly ILog mLog = LogManager.GetLogger("ProcessNameSetter");
    }
}
