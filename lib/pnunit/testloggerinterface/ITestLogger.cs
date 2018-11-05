using System;

namespace TestLogger
{
    public interface ITestLogger
    {
        bool CheckConnection();

        Guid SaveBuild(
            string type,
            string name,
            string changeset,
            string comment);

        Guid SaveSuiteRun(
            Guid buildId,
            string type,
            string name,
            string host,
            string vmachine);

        void SaveTestRun(
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
            bool isRepeated);
    }
}
