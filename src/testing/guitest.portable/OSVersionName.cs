using System;
using System.Diagnostics;
using System.IO;

namespace GuiTest
{
    internal static class OSVersionName
    {
        internal static string GetOSVersionName()
        {
            PlatformID platform = Environment.OSVersion.Platform;

            /* Depending on the installed mono version, Mac OS X system can return 
             * PlatformID.Unix value, so if it fails (/etc/issue doesn't exist)
             * then return the MAC OS X version */
            if (platform == PlatformID.Unix)
            {
                string osVersion = GetUnixVersion();
                if (!string.IsNullOrEmpty(osVersion))
                    return osVersion;
            }

            return GetMacOSXVersion();
        }

        internal static string GetOSArchitectureType()
        {
            return GetLinuxArchitectureType();
        }

        static string GetUnixVersion()
        {
            string issueFilePath = "/etc/issue";
            if (!File.Exists(issueFilePath))
                return string.Empty;

            return File.ReadAllText(issueFilePath);
        }

        static string GetMacOSXVersion()
        {
            return MacVersion.GetMacVersionWithName();
        }

        static string GetLinuxArchitectureType()
        {
            Mono.Unix.Native.Utsname result;
            int res = Mono.Unix.Native.Syscall.uname(out result);

            if (res < 0)
                return "N/A";

            return result.machine;
        }

        static class MacVersion
        {
            internal static string GetMacVersionWithName()
            {
                string osVersionNumber = GetOSVersionNumber();
                string macVersion = GetMacVersion(osVersionNumber);

                if (!string.IsNullOrEmpty(macVersion))
                    return macVersion;

                return GetMacVersionFromCommand();
            }

            static string GetOSVersionNumber()
            {
                return Environment.OSVersion.Version.ToString(3);
            }

            static string GetMacVersion(string osVersionNumber)
            {
                if (osVersionNumber.StartsWith("18"))
                    return "macOS 10.14 Mojave";
                if (osVersionNumber.StartsWith("17"))
                    return "macOS 10.13 High Sierra";
                if (osVersionNumber.StartsWith("16"))
                    return "macOS 10.12 Sierra";
                if (osVersionNumber.StartsWith("15"))
                    return "Mac OS X 10.11 El Capitan";
                if (osVersionNumber.StartsWith("14"))
                    return "Mac OS X 10.10 Yosemite";
                if (osVersionNumber.StartsWith("13"))
                    return "Mac OS X 10.9 Mavericks";
                if (osVersionNumber.StartsWith("12"))
                    return "Mac OS X 10.8 Mountain Lion";
                if (osVersionNumber.StartsWith("11"))
                    return "Mac OS X 10.7 Lion";
                if (osVersionNumber.StartsWith("10"))
                    return "Mac OS X 10.6 Snow Leopard";
                if (osVersionNumber.StartsWith("9"))
                    return "Mac OS X 10.5 Leopard";
                if (osVersionNumber.StartsWith("8"))
                    return "Mac OS X 10.4 Tiger";
                if (osVersionNumber.StartsWith("7"))
                    return "Mac OS X 10.3 Panther";
                if (osVersionNumber.StartsWith("6"))
                    return "Mac OS X 10.2 Jaguar";
                if (osVersionNumber.StartsWith("5") ||
                    osVersionNumber.StartsWith("1.4"))
                    return "Mac OS X 10.1 Puma";
                if (osVersionNumber.StartsWith("1.3"))
                    return "Mac OS X 10.0 Cheetah";
                if (osVersionNumber.StartsWith("1.2"))
                    return "Mac OS X Public Beta Kodiak";
                if (osVersionNumber.StartsWith("1.1"))
                    return "Mac OS X DP4";
                if (osVersionNumber.StartsWith("1.0"))
                    return "Mac OS X DP3";
                if (osVersionNumber.StartsWith("0.2"))
                    return "mac OS X DP2";
                if (osVersionNumber.StartsWith("0.1"))
                    return "Mac OS X DP";

                return string.Format("macOS {0}", osVersionNumber);
            }

            static string GetMacVersionFromCommand()
            {
                string output = ExecuteCommandWithResult("sw_vers", "-productVersion");
                return "Mac OS X " + output;
            }

            static string ExecuteCommandWithResult(string command, string arguments)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.FileName = command;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = true;

                string result = string.Empty;

                Process proc = new Process();
                proc.StartInfo = startInfo;

                try
                {
                    proc.Start();
                    result = proc.StandardOutput.ReadToEnd();
                }
                finally
                {
                    proc.Close();
                }

                return result;
            }
        }
    }
}
