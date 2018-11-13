using System;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.Linux.Testing
{
    internal class TesteableApplicationWindow : ITesteableApplicationWindow
    {
        internal TesteableApplicationWindow(ApplicationWindow window)
        {
            mWindow = window;
        }

        void ITesteableApplicationWindow.ChangeText(string text)
        {
            TestHelper.SetText(mWindow.TextEntry, text);
        }

        string ITesteableApplicationWindow.GetText()
        {
            return TestHelper.GetText(mWindow.TextEntry);
        }

        void ITesteableApplicationWindow.ClickAddButton()
        {
            TestHelper.ClickButton(mWindow.AddButton);
        }

        void ITesteableApplicationWindow.ClickRemoveButton()
        {
            TestHelper.ClickButton(mWindow.RemoveButton);
        }

        bool ITesteableApplicationWindow.AreButtonsEnabled()
        {
            return TestHelper.IsSensitive(mWindow.AddButton)
                && TestHelper.IsSensitive(mWindow.RemoveButton);
        }

        string ITesteableApplicationWindow.GetItemInListAt(int index)
        {
            return TestHelper.GetItemAt(mWindow.ListView, index);
        }

        int ITesteableApplicationWindow.GetItemsInListCount()
        {
            return TestHelper.GetItemCount(mWindow.ListView);
        }

        string ITesteableApplicationWindow.GetProgressMessage()
        {
            if (mWindow.ProgressControls.HasError)
                return string.Empty;

            return TestHelper.GetText(mWindow.ProgressControls.ProgressLabel);
        }

        string ITesteableApplicationWindow.GetErrorMessage()
        {
            if (!mWindow.ProgressControls.HasError)
                return string.Empty;

            return TestHelper.GetText(mWindow.ProgressControls.ProgressLabel);
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
    }
}
