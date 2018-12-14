using System;
using System.Collections.Generic;

using AppKit;
using Foundation;

using GuiTest;

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

            WindowHandler.Initialize(this);
            GuiMessage.Initialize(new MacOsGuiMessage());

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);

            if (appArgs.IsTestingMode)
                InstallTestAssembliesResolver(appArgs.PathToAssemblies);

            // Tip: you could launch different windows depending on the
            // argument flags.
            WindowHandler.LaunchApplicationWindow();

            if (appArgs.IsTestingMode)
            {
                RemotingHack.ApplyRemotingConfigurationWorkaround();
                WindowHandler.LaunchTest(appArgs.TestInfoFile, appArgs.PathToAssemblies);
            }
        }

        static void InstallTestAssembliesResolver(string pathToAssemblies)
        {
            List<string> assemblies = new List<string>();
            assemblies.Add("nunit.core.dll");
            assemblies.Add("nunit.core.interfaces.dll");
            assemblies.Add("nunit.util.dll");
            assemblies.Add("pnunit.framework.dll");

            assemblies.Add("guitestinterfaces.dll");
            assemblies.Add("guitests.dll");

            GuiTestRunner.GuiTestAssemblyResolver.InstallAssemblyResolver(
                pathToAssemblies, assemblies);
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
