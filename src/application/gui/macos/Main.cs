using System;
using System.Runtime.InteropServices;

using AppKit;

namespace Codice.Examples.GuiTesting.MacOS
{
    static class MainClass
    {
        [DllImport("/System/Library/Frameworks/Cocoa.framework/Cocoa", EntryPoint = "NSApplicationLoad")]
        static extern bool NSApplicationLoad();

        static void Main(string[] args)
        {
            NSApplicationLoad();

            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
