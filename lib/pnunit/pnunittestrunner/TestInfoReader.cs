using System;
using PNUnit.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using log4net;

namespace PNUnitTestRunner
{
    public class TestInfoReader
    {
        public static PNUnitTestInfo ReadTestInfo(string testInfoPath)
        {
            int ini = Environment.TickCount;
            PNUnitTestInfo result = null;

            try
            {
                using (FileStream fs = new FileStream(testInfoPath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                    result = (PNUnitTestInfo)bf.Deserialize(fs);

                    return result;
                }
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("Something wrong happened when reading the agent info: " + e.Message);
                return null;
            }
            finally
            {
                TryDelete(testInfoPath);
                mLog.DebugFormat(
                    "Read test info file {0} {1} ms",
                    testInfoPath, Environment.TickCount - ini);
            }
        }

        static void TryDelete(string testInfoPath)
        {
            try
            {
                File.Delete(testInfoPath);
            }
            catch (Exception e)
            {
                mLog.WarnFormat("Could not delete testInfoFile {0} {1}", testInfoPath, e.Message);
                mLog.Debug(e.StackTrace);
            }
        }

        static readonly ILog mLog = LogManager.GetLogger("PNUnitTestRunner");
    }
}