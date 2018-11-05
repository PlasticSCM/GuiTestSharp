using Codice.Examples.GuiTesting.GuiTestInterfaces;
using Codice.Examples.GuiTesting.Windows.Testing;

namespace Codice.Examples.GuiTesting.Windows.Testing
{
    internal class TesteableErrorDialog : ITesteableErrorDialog
    {
        internal TesteableErrorDialog(ErrorDialog dialog)
        {
            mErrorDialog = dialog;
            mHelper = new TestHelper(mErrorDialog);
        }

        void ITesteableErrorDialog.ClickOkButton()
        {
            mHelper.ClickDialogResultButton(mErrorDialog.OkButton);
        }

        string ITesteableErrorDialog.GetTitle()
        {
            return mHelper.GetText(mErrorDialog);
        }

        string ITesteableErrorDialog.GetMessage()
        {
            return mHelper.GetText(mErrorDialog.MessageLabel);
        }

        readonly ErrorDialog mErrorDialog;
        readonly TestHelper mHelper;
    }
}
