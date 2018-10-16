namespace PNUnit.Agent
{
    public static class AssemblyPreload
    {
        public const string PRELOADED_ASSEMBLY = "cmtest.dll";

        public const string PRELOADED_PROCESS_FILE_PREFIX = "pnunit_";

        public static bool CanUsePreload(string assembly)
        {
            return assembly == PRELOADED_ASSEMBLY;
        }
    }
}
