using System;
using System.Collections.Generic;

using AppKit;
using Foundation;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Threading;
using Codice.Examples.GuiTesting.MacOS.threading;
using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return !GetApplicationArgs().IsTestingMode;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            ApplicationArgs appArgs = GetApplicationArgs();

            ExceptionsHandler.SetExceptionHandlers(appArgs.IsTestingMode);

            ThreadWaiterBuilder.Initialize(new MacApplicationTimerBuilder());

            GuiMessage.Initialize(new MacOsGuiMessage());

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);

            if (appArgs.IsTestingMode)
                InstallTestAssembliesResolver(appArgs.PathToAssemblies);

            // Tip: you could launch different windows depending on the
            // argument flags.
            WindowHandler.LaunchApplicationWindow();

            if (appArgs.IsTestingMode)
                WindowHandler.LaunchTest(appArgs.TestInfoFile, appArgs.PathToAssemblies);
        }

        static void InstallTestAssembliesResolver(string pathToAssemblies)
        {

        }

        ApplicationArgs GetApplicationArgs()
        {
            string[] args = NSProcessInfo.ProcessInfo.Arguments;

            // NSProcessInfo.ProcessInfo.Arguments contains the executable
            // path. Just remove it.
            string[] result = new string[args.Length - 1];
            Array.Copy(args, 1, result, 0, args.Length - 1);

            return ApplicationArgs.Parse(result);
        }
    }
}
