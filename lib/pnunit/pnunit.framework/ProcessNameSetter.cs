using System;
using System.Runtime.InteropServices;
using System.Text;

public class ProcessNameSetter
{
    [DllImport("libc")] // Linux
    private static extern int prctl(int option, byte[] arg2, IntPtr arg3,
        IntPtr arg4, IntPtr arg5);

    [DllImport("libc")] // BSD
    private static extern void setproctitle(byte[] fmt, byte[] str_arg);

    public static void SetProcessName(string name)
    {
        if (IsWindows())
            return;

        try
        {
            if (prctl(15 /* PR_SET_NAME */, Encoding.ASCII.GetBytes(name + "\0"),
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0)
            {
                Console.WriteLine("Error setting process name");
            }
        }
        catch (EntryPointNotFoundException)
        {
            try
            {
                setproctitle(Encoding.ASCII.GetBytes("%s\0"),
                    Encoding.ASCII.GetBytes(name + "\0"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
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
}
