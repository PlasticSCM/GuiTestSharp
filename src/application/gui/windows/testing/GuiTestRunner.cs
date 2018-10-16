using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using PNUnit.Framework;
using NUnit.Util;

namespace Codice.Examples.GuiTesting.Windows.Testing
{
    public class GuiTestRunner
    {
        public interface IGuiFinalizer
        {
            void Finalize(int exitCode);
        }

        public GuiTestRunner(
            string testInfoFile,
            string pathToAssemblies,
            IGuiFinalizer guiFinalizer,
            ITestConsoleAccess guiConsole)
        {
            mTestInfoFile = testInfoFile;
            mPathToAssemblies = pathToAssemblies;
            mGuiFinalizer = guiFinalizer;
            mGuiConsole = guiConsole;
        }

        public void Run(object state)
        {
            InitServices.InitNUnitServices();

            PNUnitTestInfo testInfo = TestInfoReader.ReadTestInfo(mTestInfoFile);

            if (testInfo == null)
            {
                Console.Error.WriteLine("Cannot execute tests without information. Exiting...");
                mGuiFinalizer.Finalize(1);
            }

            // TODO run the test, obviously :-)
        }

        static IPNUnitServices GetPNUnitServices(string server)
        {
            return (IPNUnitServices)Activator.GetObject(
                typeof(IPNUnitServices),
                string.Format("tcp://{0}/IPNUnitServices", server));
        }

        readonly string mTestInfoFile;
        readonly string mPathToAssemblies;
        readonly IGuiFinalizer mGuiFinalizer;
        readonly ITestConsoleAccess mGuiConsole;

        internal static class GuiTestAssemblyResolver
        {
            internal static void InstallAssemblyResolver(
                string pathToAssemblies, List<string> assemblies)
            {
                mPathToAssemblies = pathToAssemblies;
                mAssemblies = assemblies;

                AppDomain.CurrentDomain.AssemblyResolve += Resolve;
            }

            internal static void UninstallAssemblyResolver()
            {
                AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
            }

            static Assembly Resolve(object sender, ResolveEventArgs e)
            {
                string[] asm = e.Name.Split(',');

                string assemblyName = asm[0];

                if (!assemblyName.EndsWith(".dll"))
                    assemblyName += ".dll";

                if (mAssemblies == null || !mAssemblies.Contains(assemblyName))
                    return null;

                string assemblyFullPath = Path.Combine(mPathToAssemblies, assemblyName);

                return Assembly.LoadFrom(assemblyFullPath);
            }

            static string mPathToAssemblies;
            static List<string> mAssemblies;
        }

        class InitServices
        {
            internal static void InitNUnitServices()
            {
                ServiceManager.Services.AddService(new SettingsService());
                ServiceManager.Services.AddService(new DomainManager());
                ServiceManager.Services.AddService(new ProjectService());

                NUnit.Core.CoreExtensions.Host.InitializeService();
                ServiceManager.Services.InitializeServices();
            }
        }
    }
}
