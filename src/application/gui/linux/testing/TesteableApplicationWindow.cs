using System;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.Linux.Testing
{
    internal class TesteableApplicationWindow : ITesteableApplicationWindow
    {
        bool ITesteableApplicationWindow.AreButtonsEnabled()
        {
            throw new NotImplementedException();
        }

        void ITesteableApplicationWindow.ChangeText(string text)
        {
            throw new NotImplementedException();
        }

        void ITesteableApplicationWindow.ClickAddButton()
        {
            throw new NotImplementedException();
        }

        void ITesteableApplicationWindow.ClickRemoveButton()
        {
            throw new NotImplementedException();
        }

        ITesteableErrorDialog ITesteableApplicationWindow.GetErrorDialog()
        {
            throw new NotImplementedException();
        }

        string ITesteableApplicationWindow.GetErrorMessage()
        {
            throw new NotImplementedException();
        }

        string ITesteableApplicationWindow.GetItemInListAt(int index)
        {
            throw new NotImplementedException();
        }

        int ITesteableApplicationWindow.GetItemsInListCount()
        {
            throw new NotImplementedException();
        }

        string ITesteableApplicationWindow.GetProgressMessage()
        {
            throw new NotImplementedException();
        }

        string ITesteableApplicationWindow.GetText()
        {
            throw new NotImplementedException();
        }
    }
}
