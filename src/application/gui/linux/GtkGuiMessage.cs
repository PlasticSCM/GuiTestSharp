using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.Linux
{
    internal class GtkGuiMessage : GuiMessage.IGuiMessage
    {
        void GuiMessage.IGuiMessage.ShowError(string title, string message)
        {
            ErrorDialog dialog = new ErrorDialog(
                title, message, WindowHandler.ApplicationWindow);

#if !NETCORE
            WindowHandler.SetActiveDialogForTesting(dialog);

            try
            {
                dialog.RunModal();
            }
            finally
            {
                WindowHandler.RemoveDialogForTesting(dialog);
                dialog.Dispose();
            }
#else
            using (dialog) { dialog.RunModal(); }
#endif
        }
    }
}
