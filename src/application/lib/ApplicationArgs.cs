using System.IO;

namespace Codice.Examples.GuiTesting.Lib
{
    public class ApplicationArgs
    {
        public readonly bool IsTestingMode;
        public readonly string PathToAssemblies;

        ApplicationArgs(bool isTestingMode, string pathToAssemblies)
        {
            IsTestingMode = isTestingMode;
            PathToAssemblies = pathToAssemblies;
        }

        public static ApplicationArgs Parse(string[] args)
        {
            if (args.Length < 2
                || args[0] != TESTING_FLAG
                || !Directory.Exists(args[1]))
            {
                return new ApplicationArgs(false, string.Empty);
            }

            return new ApplicationArgs(true, args[1]);
        }

        const string TESTING_FLAG = "--testing";
    }
}
