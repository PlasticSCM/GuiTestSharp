using System;
using System.IO;
using System.Reflection;

using log4net;

namespace TestLogger
{
    public class TestLoggerClient
    {
        public TestLoggerClient()
        {
            mServerLoggerUrl = LoadLoggerServerUrl();
        }

        public TestLoggerClient(string serverLoggerUrl)
        {
            mServerLoggerUrl = serverLoggerUrl;
        }

        public bool CheckConnection()
        {
            ITestLogger logger = (ITestLogger)
            Activator.GetObject(typeof(ITestLogger), mServerLoggerUrl);
            return logger.CheckConnection();
        }

        public Guid SaveBuild(string type, string name, string changeset, string comment)
        {
            ITestLogger logger = (ITestLogger)
            Activator.GetObject(typeof(ITestLogger), mServerLoggerUrl);
            return logger.SaveBuild(type, name, changeset, comment);
        }

        public Guid SaveSuiteRun(
            Guid buildId,
            string type,
            string name,
            string host, 
            string vmachine)
        {
            ITestLogger logger = (ITestLogger)
            Activator.GetObject(typeof(ITestLogger), mServerLoggerUrl);
            return logger.SaveSuiteRun(buildId, type, name, host, vmachine);
        }

        public void SaveTestRun(
            Guid buildId,
            Guid suiteRunId,
            string suiteType,
            string testName,
            string clientConfig,
            string serverConfig,
            string backendType,
            int executionTimeSecs,
            string status,
            string log,
            bool isRepeated)
        {
            ITestLogger logger = (ITestLogger)
            Activator.GetObject(typeof(ITestLogger), mServerLoggerUrl);
            logger.SaveTestRun(
                buildId,
                suiteRunId,
                suiteType,
                testName,
                clientConfig,
                serverConfig,
                backendType,
                executionTimeSecs,
                status,
                log,
                isRepeated);
        }

        private string LoadLoggerServerUrl()
        {
            string serverLoggerConfPath =
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                TEST_LOGGER_SERVER_CONF_FILE);
            try
            {
                using (StreamReader sr = new StreamReader(serverLoggerConfPath))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
            catch (Exception)
            {
                return "tcp://192.168.1.78:9999/TestLogger";
            }
        }

        private string mServerLoggerUrl = string.Empty;
        private const string TEST_LOGGER_SERVER_CONF_FILE = "testloggerserver.conf";
        private readonly ILog log = LogManager.GetLogger("launcher");
    }
}
