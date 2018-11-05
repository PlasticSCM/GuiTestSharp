using System.Drawing;
using System.Windows.Forms;

namespace Codice.Examples.GuiTesting.Windows.UI
{
    internal static class ControlBuilder
    {
        internal const int DefaultControlHeight = 23;

        internal const int DefaultHorizontalPaddingWidth = 6;
        internal const int DefaultVerticalPaddingHeight = 8;

        internal static Control CreateVerticalPadding(DockStyle dock)
        {
            Panel result = CreatePanel(dock);
            result.Height = DefaultVerticalPaddingHeight;

            return result;
        }

        internal static Control CreateHorizontalPadding(DockStyle dock)
        {
            Panel result = CreatePanel(dock);
            result.Width = DefaultHorizontalPaddingWidth;

            return result;
        }

        internal static Panel CreatePanel(int height, DockStyle dock)
        {
            Panel result = CreatePanel(dock);
            result.Height = height;

            return result;
        }

        internal static Panel CreatePanel(DockStyle dock)
        {
            Panel result = new Panel();
            result.Dock = dock;

            return result;
        }

        internal static Label CreateLabel(string text, DockStyle dock)
        {
            Label result = new Label();
            result.Text = text;
            result.Dock = dock;
            result.AutoSize = true;

            return result;
        }

        internal static TextBox CreateTextbox(DockStyle dock)
        {
            TextBox result = new TextBox();
            result.Dock = dock;

            return result;
        }

        internal static Button CreateButton(string text, DockStyle dock)
        {
            Button result = new Button();
            result.Text = text;
            result.Dock = dock;

            return result;
        }

        internal static ListBox CreateListBox(DockStyle dock)
        {
            ListBox result = new ListBox();
            result.Dock = dock;

            return result;
        }

        internal static PictureBox CreatePictureBox(Image image, DockStyle dock)
        {
            PictureBox result = new PictureBox();
            result.Image = image;
            result.Width = image.Width;
            result.Height = image.Height;
            result.Dock = dock;

            return result;
        }
    }
}
