using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.Windows
{
    internal class WindowsGuiMessage : GuiMessage.IGuiMessage
    {
        void GuiMessage.IGuiMessage.ShowError(string title, string message)
        {
            Form dialog = new ErrorDialog(title, message);
            dialog.StartPosition = FormStartPosition.CenterParent;

            WindowHandler.SetActiveDialogForTesting(dialog);

            try
            {
                dialog.ShowDialog(WindowHandler.ApplicationWindow);
            }
            finally
            {
                WindowHandler.RemoveDialogForTesting(dialog);
                dialog.Dispose();
            }
        }
    }
}
