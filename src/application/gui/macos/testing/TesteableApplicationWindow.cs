using AppKit;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.MacOS.Testing
{
    internal class TesteableApplicationWindow : ITesteableApplicationWindow
    {
        public TesteableApplicationWindow(ApplicationWindow window)
        {
            mWindow = window;
            mHelper = new TestHelper();
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

        void ITesteableApplicationWindow.ChangeText(string text)
        {
            mHelper.SetText(mWindow.TextField, text);
        }

        string ITesteableApplicationWindow.GetText()
        {
            return mHelper.GetText(mWindow.TextField);
        }

        int ITesteableApplicationWindow.GetItemsInListCount()
        {
            return mHelper.GetItemCount(mWindow.TableView);
        }

        string ITesteableApplicationWindow.GetItemInListAt(int index)
        {
            return mHelper.GetItemAt(mWindow.TableView, "Text", index);
        }

        string ITesteableApplicationWindow.GetProgressMessage()
        {
            if (mWindow.ProgressControls.HasError)
                return string.Empty;

            return mHelper.GetText(mWindow.ProgressControls.ProgressTextField);
        }

        string ITesteableApplicationWindow.GetErrorMessage()
        {
            if (!mWindow.ProgressControls.HasError)
                return string.Empty;

            return mHelper.GetText(mWindow.ProgressControls.ProgressTextField);
        }

        ITesteableErrorDialog ITesteableApplicationWindow.GetErrorDialog()
        {
            if (WindowHandler.GetActiveDialog() == null)
                return null;

            NSAlert errorDialog = WindowHandler.GetActiveDialog() as NSAlert;
            if (errorDialog == null)
                return null;

            return new TesteableErrorDialog(errorDialog);
        }

        readonly ApplicationWindow mWindow;
        readonly TestHelper mHelper;
    }
}
