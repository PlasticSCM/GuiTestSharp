using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Reflection;

using log4net;
using log4net.Config;

namespace PNUnit.Launcher
{
    internal class Configurator
    {
        internal static bool ConfigureRemoting(int port, string ipToBind)
        {
            BinaryClientFormatterSinkProvider clientProvider = null;
            BinaryServerFormatterSinkProvider serverProvider =
                new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel =
                System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IDictionary props = new Hashtable();
            props["port"] = port;
            props["typeFilterLevel"] = TypeFilterLevel.Full;

            if (!string.IsNullOrEmpty(ipToBind))
            {
                mLog.InfoFormat("Binding the ip to {0}", ipToBind);
                props["bindTo"] = ipToBind;
            }

            try
            {
                TcpChannel chan = new TcpChannel(
                    props, clientProvider, serverProvider);

                mLog.DebugFormat("Registering channel on port {0}", port);
                ChannelServices.RegisterChannel(chan, false);
                return true;
            }
            catch (Exception e)
            {
                mLog.InfoFormat("Can't register channel.\n{0}", e.Message);
                return false;
            }
        }

        internal static void ConfigureLogging(
            bool bIsAutomatedLauncher, string customLogOutputPath)
        {
            string logConfFileName = "launcher.log.conf";

            if (bIsAutomatedLauncher)
                logConfFileName = "automated" + logConfFileName;

            if (!File.Exists(logConfFileName))
            {
                logConfFileName = GetConfigFilePath(logConfFileName);
            }

            // the following property must be declared in the "file" tag in .log.conf appenders:
            // "%property{LogFolder}"
            log4net.GlobalContext.Properties["LogFolder"] = customLogOutputPath;

            XmlConfigurator.Configure(new FileInfo(logConfFileName));
        }

        static string GetConfigFilePath(string fileName)
        {
            return Path.Combine(GetAppPath(), fileName);
        }

        static string GetAppPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        static readonly ILog mLog = LogManager.GetLogger("launcher");
    }
}