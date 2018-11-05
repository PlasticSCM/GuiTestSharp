using System;

namespace Codice.Test
{
    public class PlatformIdentifier
    {
        private static bool bIsWindowsInitialized = false;
        private static bool bIsWindows = false;
        public static bool IsWindows()
        {
            if( !bIsWindowsInitialized )
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                        bIsWindows = true;
                        break;
                }
                bIsWindowsInitialized = true;
            }
            return bIsWindows;
        }

        private static bool bIsMacInitialized = false;
        private static bool bIsMac = false;
        public static bool IsMac()
        {
            if (!bIsMacInitialized)
            {
                if (!IsWindows())
                {
                    // The first versions of the framework (1.0 and 1.1)
                    // didn't include any PlatformID value for Unix,
                    // so Mono used the value 128. The newer framework 2.0
                    // added Unix to the PlatformID enum but,
                    // sadly, with a different value: 4 and newer versions of
                    // .NET distinguished between Unix and MacOS X,
                    // introducing yet another value 6 for MacOS X.

                    System.Version v = Environment.Version;
                    int p = (int) Environment.OSVersion.Platform;

                    if( (v.Major >= 3 && v.Minor >= 5) ||
                        (IsRunningUnderMono() && v.Major >= 2 && v.Minor >= 2))
                    {
                        //MacOs X exist in the enumeration
                        bIsMac = p == 6;
                    }
                    else
                    {
                        if ((p == 4) || (p == 128))
                        {
                            int major = Environment.OSVersion.Version.Major;
                            //18.x.x  macOS 10.14.x Mojave
                            //17.x.x  macOS 10.13.x High Sierra
                            //16.x.x  macOS 10.12.x Sierra
                            //15.x.x  OS X 10.11.x El Capitan
                            //14.x.x  OS X 10.10.x Yosemite
                            //13.x.x  OS X 10.9.x Mavericks
                            //12.x.x  OS X 10.8.x Mountain Lion
                            //11.x.x  OS X 10.7.x Lion
                            //10.x.x  OS X 10.6.x Snow Leopard
                            //9.x.x  OS X 10.5.x Leopard
                            //8.x.x  OS X 10.4.x Tiger
                            // DAVE: this is not very nice, as it may conflict
                            // on other OS like solaris or aix.
                            bIsMac = major >= 8;
                        }
                    }
                }

                bIsMacInitialized = true;
            }

            return bIsMac;
        }


        private static bool IsRunningUnderMono()
        {
            Type t = Type.GetType("Mono.Runtime");

            return (t != null);
        }
    }
}
    