using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.Linux.Testing
{
    internal class TesteableErrorDialog : ITesteableErrorDialog
    {
        internal TesteableErrorDialog(ErrorDialog errorDialog)
        {
            mDialog = errorDialog;
        }

        void ITesteableErrorDialog.ClickOkButton()
        {
            TestHelper.ClickDialogButton(mDialog.OkButton);
        }

        string ITesteableErrorDialog.GetTitle()
        {
            return TestHelper.GetTitle(mDialog);
        }

        string ITesteableErrorDialog.GetMessage()
        {
            return TestHelper.GetText(mDialog.MessageLabel);
        }

        readonly ErrorDialog mDialog;
    }
}
