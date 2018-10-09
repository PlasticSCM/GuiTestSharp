using System;

namespace Codice.Examples.GuiTesting.GuiTestInterfaces
{
    public class GuiTesteableServices
    {
        public static Exception UnhandledException
        {
            get { lock (mInstance) return mInstance.mUnhandledException; }
            set { lock (mInstance) mInstance.mUnhandledException = value; }
        }

        public static void Init(ITesteableApplicationWindow window)
        {
            throw new NotImplementedException();
        }

        Exception mUnhandledException;
        static GuiTesteableServices mInstance;
    }
}
