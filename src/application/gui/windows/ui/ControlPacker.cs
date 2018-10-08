using System.Windows.Forms;

namespace Codice.Examples.GuiTesting.Windows.UI
{
    internal static class ControlPacker
    {
        internal static void AddControl(Control parent, Control child)
        {
            parent.Controls.Add(child);
            if (child.Dock == DockStyle.Left || child.Dock == DockStyle.Top ||
                child.Dock == DockStyle.Fill || child.Dock == DockStyle.None)
            {
                child.BringToFront();
            }
        }
    }
}
