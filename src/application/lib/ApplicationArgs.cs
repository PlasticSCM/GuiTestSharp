using System.IO;

namespace Codice.Examples.GuiTesting.Lib
{
    public class ApplicationArgs
    {
        public readonly bool IsTestingMode;
        public readonly string TestInfoFile;
        public readonly string PathToAssemblies;

        ApplicationArgs(
            bool isTestingMode,
            string testInfoFile,
            string pathToAssemblies)
        {
            IsTestingMode = isTestingMode;
            TestInfoFile = testInfoFile;
            PathToAssemblies = pathToAssemblies;
        }

        public static ApplicationArgs Parse(string[] args)
        {
            if (args.Length < 3
                || args[0] != TESTING_FLAG
                || !File.Exists(args[1])
                || !Directory.Exists(args[2]))
            {
                return new ApplicationArgs(false, string.Empty, string.Empty);
            }

            return new ApplicationArgs(true, args[1], args[2]);
        }

        const string TESTING_FLAG = "--testing";
    }
}
