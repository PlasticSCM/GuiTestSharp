using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.Windows
{
    internal class WindowsGuiMessage : GuiMessage.IGuiMessage
    {
        void GuiMessage.IGuiMessage.ShowError(string title, string message)
        {
            Form dialog = new ErrorDialog(title, message);
            WindowHandler.SetActiveDialogForTesting(dialog);

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(WindowHandler.ApplicationWindow);

            WindowHandler.RemoveDialogForTesting(dialog);
        }
    }
}
