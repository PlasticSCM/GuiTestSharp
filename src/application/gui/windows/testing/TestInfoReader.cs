using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;

using log4net;

using PNUnit.Framework;

namespace Codice.Examples.GuiTesting.Windows.Testing
{
    internal class TestInfoReader
    {
        internal static PNUnitTestInfo ReadTestInfo(string testInfoPath)
        {
            int init = Environment.TickCount;
            try
            {
                using (FileStream fs = new FileStream(testInfoPath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                    return (PNUnitTestInfo)bf.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                mLog.ErrorFormat(
                    "Something went wrong while reading the agent info: {0}",
                    ex.Message);
                return null;
            }
            finally
            {
                TryDelete(testInfoPath);
                mLog.DebugFormat(
                    "Read test info file {0} in {1} ms.",
                    testInfoPath, Environment.TickCount - init);
            }
        }

        static void TryDelete(string testInfoPath)
        {
            try
            {
                File.Delete(testInfoPath);
            }
            catch (Exception ex)
            {
                mLog.WarnFormat(
                    "Could not delete test info file {0}: {1}",
                    testInfoPath, ex.Message);

                mLog.DebugFormat(
                    "StackTrace:{0}{1}",
                    Environment.NewLine, ex.StackTrace);
            }
        }

        static readonly ILog mLog = LogManager.GetLogger("TestInfoReader");
    }
}
