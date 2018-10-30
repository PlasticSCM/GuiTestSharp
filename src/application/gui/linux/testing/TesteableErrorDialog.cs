using System;

using Codice.Examples.GuiTesting.GuiTestInterfaces;

namespace Codice.Examples.GuiTesting.Linux.Testing
{
    internal class TesteableErrorDialog : ITesteableErrorDialog
    {
        void ITesteableErrorDialog.ClickOkButton()
        {
            throw new NotImplementedException();
        }

        string ITesteableErrorDialog.GetMessage()
        {
            throw new NotImplementedException();
        }

        string ITesteableErrorDialog.GetTitle()
        {
            throw new NotImplementedException();
        }
    }
}
