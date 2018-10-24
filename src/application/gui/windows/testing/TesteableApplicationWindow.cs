using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.Windows.Testing
{
    internal class TesteableApplicationWindow : ITesteableApplicationWindow
    {
        internal TesteableApplicationWindow(ApplicationWindow window)
        {
            mWindow = window;
            mHelper = new TestHelper(mWindow);
        }

        void ITesteableApplicationWindow.ChangeText(string text)
        {
            mHelper.SetText(mWindow.TextBox, text);
        }

        string ITesteableApplicationWindow.GetText()
        {
            return mHelper.GetText(mWindow.TextBox);
        }

        void ITesteableApplicationWindow.ClickAddButton()
        {
            mHelper.ClickButton(mWindow.AddButton);
        }

        void ITesteableApplicationWindow.ClickRemoveButton()
        {
            mHelper.ClickButton(mWindow.RemoveButton);
        }

        bool ITesteableApplicationWindow.AreButtonsEnabled()
        {
            return mHelper.IsEnabled(mWindow.AddButton)
                && mHelper.IsEnabled(mWindow.RemoveButton);
        }

        string ITesteableApplicationWindow.GetItemInListAt(int index)
        {
            return mHelper.GetItemAt(mWindow.ListBox, index).ToString();
        }

        int ITesteableApplicationWindow.GetItemsInListCount()
        {
            return mHelper.GetItemCount(mWindow.ListBox);
        }

        string ITesteableApplicationWindow.GetProgressMessage()
        {
            if (mWindow.ProgressControls.HasError)
                return string.Empty;

            return mHelper.GetText(mWindow.ProgressControls.ProgressLabel);
        }

        string ITesteableApplicationWindow.GetErrorMessage()
        {
            if (!mWindow.ProgressControls.HasError)
                return string.Empty;

            return mHelper.GetText(mWindow.ProgressControls.ProgressLabel);
        }

        ITesteableErrorDialog ITesteableApplicationWindow.GetErrorDialog()
        {
            if (WindowHandler.GetActiveDialog() == null)
                return null;

            ErrorDialog errorDialog = WindowHandler.GetActiveDialog() as ErrorDialog;
            if (errorDialog == null)
                return null;

            return new TesteableErrorDialog(errorDialog);
        }

        readonly ApplicationWindow mWindow;
        readonly TestHelper mHelper;
    }
}
