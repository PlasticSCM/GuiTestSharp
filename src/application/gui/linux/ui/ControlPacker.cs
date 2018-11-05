using Gtk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class ControlPacker
    {
        internal static void Add(Box box, Widget widget, uint padding = 0)
        {
            box.PackStart(widget, false, false, padding);
        }

        internal static void Fill(Box box, Widget widget, uint padding = 0)
        {
            box.PackStart(widget, true, true, padding);
        }

        internal static void PackActionButtons(HBox box, uint padding, params Widget[] buttons)
        {
            HBox buttonsBox = new HBox();

            Add(buttonsBox, buttons[0]);
            for (int i = 1; i < buttons.Length; i++)
                Add(buttonsBox, AlignmentBuilder.LeftPadding(buttons[i], padding));

            Fill(box, ControlBuilder.CreateEmptyArea());
            Add(box, buttonsBox);
        }
    }
}
