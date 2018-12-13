using AppKit;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.MacOS
{
    public class MacOsGuiMessage : GuiMessage.IGuiMessage
    {
        void GuiMessage.IGuiMessage.ShowError(string title, string message)
        {
            NSAlert alert = new NSAlert();
            alert.MessageText = title;
            alert.InformativeText = message;
            alert.Icon = NSImage.ImageNamed(NSImageName.Caution);
            alert.AlertStyle = NSAlertStyle.Warning;

            NSButton closeButton = alert.AddButton(
                Localization.GetText(Localization.Name.Ok));
            closeButton.KeyEquivalent = ESC_KEY_EQUIVALENT;

            WindowHandler.SetActiveDialogForTesting(alert);

            alert.RunModal();

            WindowHandler.RemoveDialogForTesting(alert);
        }

        internal static readonly string ESC_KEY_EQUIVALENT =
            ((char)0x001B).ToString();
    }
}
