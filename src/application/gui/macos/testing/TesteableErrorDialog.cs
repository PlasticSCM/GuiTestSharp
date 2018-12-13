using AppKit;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.MacOS.Testing
{
    internal class TesteableErrorDialog : ITesteableErrorDialog
    {
        internal TesteableErrorDialog(NSAlert alert)
        {
            mAlert = alert;
            mHelper = new TestHelper();
        }

        void ITesteableErrorDialog.ClickOkButton()
        {
            mHelper.CLickDialogButton(mAlert.Buttons[0]);
        }

        string ITesteableErrorDialog.GetTitle()
        {
            return mHelper.GetAlertTitle(mAlert);
        }

        string ITesteableErrorDialog.GetMessage()
        {
            return mHelper.GetAlertMessage(mAlert);
        }

        readonly NSAlert mAlert;
        readonly TestHelper mHelper;
    }
}
