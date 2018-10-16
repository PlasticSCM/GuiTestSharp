using System;
using System.IO;
using System.Reflection;

using log4net.Config;

using NUnit.Util;

namespace PNUnit.Agent
{
    class Agent
    {
        [STAThread]
        static void Main(string[] args)
        {
            ProcessNameSetter.SetProcessName("agent");

            ConfigureLogging();

            AgentConfig config = new AgentConfig();

            // read --daemon
            bool bDaemonMode = ReadFlag(args, "--daemon");

            bool bNoTimeout = ReadFlag(args, "--notimeout");

            string preloadTestRunners = ReadKeyVal(args, "--preloadrunners");

            string configfile = ReadArg(args);

            int port = DEFAULT_PORT;

            string pathtoassemblies = ReadArg(args);

            if (pathtoassemblies != null)
            {
                port = int.Parse(configfile);
                configfile = null;
            }

            // Load the test configuration file
            if (pathtoassemblies == null && configfile == null)
            {
                Console.WriteLine(
                    "Usage: agent [configfile | port path_to_assemblies]"+
                    " [--daemon] [--noTimeout]");
                return;
            }

            if (configfile != null)
            {
                config = AgentConfigLoader.LoadFromFile(configfile);

                if (config == null)
                {
                    Console.WriteLine("No agent.conf file found");
                }
            }
            else
            {
                config.Port = port;
                config.PathToAssemblies = pathtoassemblies;
            }

            if( bNoTimeout )
            {
                config.NoTimeout = true;
            }

            InitNUnitServices();

            PNUnitAgent agent = new PNUnitAgent();
            agent.Run(config, bDaemonMode, preloadTestRunners);
        }

        private static void InitNUnitServices()
        {
            // initialize NUnit services
            // Add Standard Services to ServiceManager
            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());

            // initialize NUnit services
            // Add Standard Services to ServiceManager
            ServiceManager.Services.AddService(new SettingsService());
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());

            NUnit.Core.CoreExtensions.Host.InitializeService();

            // Initialize Services
            ServiceManager.Services.InitializeServices();
        }

        private static bool ReadFlag(string[] args, string flag)
        {
            for (int i = args.Length - 1; i >= 0; --i)
                if (args[i] == flag)
                {
                    args[i] = null;
                    return true;
                }

            return false;
        }

        private static string ReadKeyVal(string[] args, string key)
        {
            for (int i = args.Length - 1; i >= 0; --i)
            {
                if (args[i] == null)
                {
                    break;
                }
                if (args[i].StartsWith(key))
                {
                    string entry = args[i];
                    args[i] = null;

                    string[] parts = entry.Split('=');

                    if (parts.Length == 1)
                        return string.Empty;

                    return parts[1];
                }
            }
            return null;
        }

        private static string ReadArg(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
                if (args[i] != null)
                {
                    string result = args[i];
                    args[i] = null;
                    return result;
                }
            return null;
        }

        private static void ConfigureLogging()
        {
            string log4netpath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "agent.log.conf");

            XmlConfigurator.ConfigureAndWatch(new FileInfo(log4netpath));
        }

        const int DEFAULT_PORT = 8080;
    }
}