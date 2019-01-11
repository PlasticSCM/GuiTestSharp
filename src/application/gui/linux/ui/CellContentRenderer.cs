using Gtk;
using Pango;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class CellContentRenderer
    {
        internal static void RenderText(CellRenderer cell, string text)
        {
            CellRendererText textCell = cell as CellRendererText;
            textCell.FontDesc.Weight = Weight.Normal;
            textCell.Text = text;
        }
    }
}
