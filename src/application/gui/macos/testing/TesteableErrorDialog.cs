using AppKit;

using Codice.Examples.GuiTesting.GuiTestInterfaces;
using Codice.Examples.GuiTesting.Lib;

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
            mHelper.ClickDialogButton(
                mHelper.GetButton(
                    mAlert,
                    Localization.GetText(Localization.Name.Ok)));
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
