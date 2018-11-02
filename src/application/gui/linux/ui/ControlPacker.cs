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
    }
}
