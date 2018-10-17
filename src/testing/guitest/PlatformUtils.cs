using System;
using System.IO;

namespace GuiTest
{
    public static class PlatformUtils
    {
        public enum Platform
        {
            Unknown,
            Windows,
            MacOS,
            Linux
        }

        public static Platform CurrentPlatform
        {
            get
            {
                if (mCurrentPlatform != Platform.Unknown)
                    return mCurrentPlatform;

                mCurrentPlatform = GetCurrentPlatform();
                return mCurrentPlatform;
            }
        }

        static Platform GetCurrentPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return Platform.MacOS;

                case PlatformID.Unix:
                    return AreMacOSDirectoriesPresent()
                        ? Platform.MacOS
                        : Platform.Linux;

                default:
                    return Platform.Windows;
            }
        }

        static bool AreMacOSDirectoriesPresent()
        {
            return Directory.Exists("/Applications")
                && Directory.Exists("/System")
                && Directory.Exists("/Users")
                && Directory.Exists("/Volumes");
        }

        static Platform mCurrentPlatform = Platform.Unknown;
    }
}
