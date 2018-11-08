using System;
using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.Linux.UI;

namespace Codice.Examples.GuiTesting.Linux
{
    internal class GtkGuiMessage : GuiMessage.IGuiMessage
    {
        void GuiMessage.IGuiMessage.ShowError(string title, string message)
        {
            BaseDialog dialog = new ErrorDialog(
                title, message, WindowHandler.ApplicationWindow);

            try
            {
                dialog.RunModal();
            }
            finally
            {
                dialog.Dispose();
            }
        }
    }
}
