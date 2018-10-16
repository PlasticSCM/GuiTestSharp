using System.IO;

namespace Codice.Examples.GuiTesting.Lib
{
    public class ApplicationArgs
    {
        public readonly bool IsTestingMode;
        public readonly string PathToAssemblies;
        public readonly string TestInfoFile;

        ApplicationArgs(
            bool isTestingMode,
            string pathToAssemblies,
            string testInfoFile)
        {
            IsTestingMode = isTestingMode;
            PathToAssemblies = pathToAssemblies;
            TestInfoFile = testInfoFile;
        }

        public static ApplicationArgs Parse(string[] args)
        {
            if (args.Length < 3
                || args[0] != TESTING_FLAG
                || !Directory.Exists(args[1])
                || !File.Exists(args[2]))
            {
                return new ApplicationArgs(false, string.Empty, string.Empty);
            }

            return new ApplicationArgs(true, args[1], args[2]);
        }

        const string TESTING_FLAG = "--testing";
    }
}
