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
            if (mInstance == null)
                mInstance = new GuiTesteableServices();

            mInstance.mTesteableApplicationWindow = window;
        }

        Exception mUnhandledException;
        static GuiTesteableServices mInstance;

        ITesteableApplicationWindow mTesteableApplicationWindow;
    }
}
