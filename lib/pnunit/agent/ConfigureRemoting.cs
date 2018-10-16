using System;
using System.IO;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

using log4net;

namespace PNUnit.Agent
{
    class ConfigureRemoting
    {
        internal static void Configure(int port)
        {
            mLog.InfoFormat("Registering channel");

            if (File.Exists("agent.remoting.conf"))
            {
                mLog.Info("Using agent.remoting.conf");
                RemotingConfiguration.Configure("agent.remoting.conf", false);
                return;
            }

            // init remoting
            BinaryClientFormatterSinkProvider clientProvider =
                new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider serverProvider =
                new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel =
                System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IDictionary props = new Hashtable();
            props["port"] = port;
            string s = System.Guid.NewGuid().ToString();
            props["name"] = s;
            props["typeFilterLevel"] = TypeFilterLevel.Full;
            try
            {
                TcpChannel chan = new TcpChannel(
                    props, clientProvider, serverProvider);

                mLog.InfoFormat("Registering channel on port {0}", port);
                ChannelServices.RegisterChannel(chan, false);
                //RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            }
            catch (Exception e)
            {
                mLog.InfoFormat("Can't register channel.\n{0}", e.Message);
                Console.WriteLine("Can't register channel.\n{0}", e.Message);
                throw;
            }
        }

        static readonly ILog mLog = LogManager.GetLogger("ConfigureRemoting");
    }
}