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

        public static ITesteableApplicationWindow GetApplicationWindow()
        {
            return mInstance.mTesteableApplicationWindow;
        }

        public static void SetErrorDialog(ITesteableErrorDialog dialog)
        {
            mInstance.mTesteableErrorDialog = dialog;
        }

        public static ITesteableErrorDialog GetErrorDialog()
        {
            return mInstance.mTesteableErrorDialog;
        }

        Exception mUnhandledException;
        static GuiTesteableServices mInstance;

        ITesteableApplicationWindow mTesteableApplicationWindow;
        ITesteableErrorDialog mTesteableErrorDialog;
    }
}
