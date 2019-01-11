using System;
using System.IO;
using System.Runtime.Remoting;
using System.Reflection;

namespace Codice.Examples.GuiTesting.MacOS
{
    internal static class RemotingHack
    {
        internal static void ApplyRemotingConfigurationWorkaround()
        {
            new MonoBug44707_Workaround().HackMonoBug_44707();
        }

        class MonoBug44707_Workaround
        {
            // Bug #44707
            // RemotingConfiguration.Configure() throws RemotingException because
            // it cannot load 'machine.config'
            // https://bugzilla.xamarin.com/show_bug.cgi?id=44707
            internal void HackMonoBug_44707()
            {
                string machineConfigPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "machine.config");

                MethodInfo readConfigMethod =
                    typeof(RemotingConfiguration).GetMethod(
                        "ReadConfigFile",
                        BindingFlags.NonPublic | BindingFlags.Static);
                CheckValidReadConfigFileMethod(readConfigMethod);

                readConfigMethod.Invoke(this, new[] { machineConfigPath });

                FieldInfo defaultReadField =
                    typeof(RemotingConfiguration).GetField(
                        "defaultConfigRead",
                        BindingFlags.NonPublic | BindingFlags.Static);
                CheckValidDefaultConfigReadField(defaultReadField);

                defaultReadField.SetValue(this, true);
            }

            static void CheckValidReadConfigFileMethod(MethodInfo mi)
            {
                if (mi != null)
                    return;

                throw new InvalidOperationException(
                    "RemotingHack failed - ReadConfigFile method not found " +
                    "in RemotingConfiguration class.");
            }

            static void CheckValidDefaultConfigReadField(FieldInfo fi)
            {
                if (fi != null)
                    return;

                throw new InvalidOperationException(
                    "RemotingHack failed - 'defaulConfigRead' field not found " +
                    "in RemotingConfiguration class.");
            }
        }
    }
}
