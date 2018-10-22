using System;
using System.Drawing;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Windows.UI;

namespace Codice.Examples.GuiTesting.Windows
{
    internal class ErrorDialog : Form
    {
        internal ErrorDialog(string title, string message)
        {
            Text = title;
            BuildComponents();

            mMessageLabel.Text = message;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                mOkButton.Click -= OkButton_Click;

            base.Dispose(disposing);
        }

        void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        void BuildComponents()
        {
            Width = DIALOG_WIDTH;
            Height = DIALOG_HEIGHT;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;

            ControlPacker.AddControl(
                this, ControlBuilder.CreateVerticalPadding(DockStyle.Top));
            ControlPacker.AddControl(this, BuildMessagePanel());
            ControlPacker.AddControl(
                this, ControlBuilder.CreateVerticalPadding(DockStyle.Top));
            ControlPacker.AddControl(this, BuildButtonPanel());
            ControlPacker.AddControl(
                this, ControlBuilder.CreateVerticalPadding(DockStyle.Bottom));
        }

        Panel BuildMessagePanel()
        {
            Panel result = ControlBuilder.CreatePanel(DockStyle.Top);
            result.Height = 70;

            PictureBox errorPictureBox = ControlBuilder.CreatePictureBox(
                SystemIcons.Error.ToBitmap(), DockStyle.Left);
            mMessageLabel = ControlBuilder.CreateLabel(string.Empty, DockStyle.Fill);
            mMessageLabel.AutoSize = false;

            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Left));
            ControlPacker.AddControl(result, errorPictureBox);
            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Left));
            ControlPacker.AddControl(result, mMessageLabel);
            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Right));

            return result;
        }

        Panel BuildButtonPanel()
        {
            Panel result = ControlBuilder.CreatePanel(
                ControlBuilder.DefaultControlHeight, DockStyle.Top);

            mOkButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.Ok), DockStyle.Right);

            mOkButton.Click += OkButton_Click;
            CancelButton = mOkButton;

            ControlPacker.AddControl(result, mOkButton);
            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Right));

            return result;
        }

        Label mMessageLabel;
        Button mOkButton;

        const int DIALOG_WIDTH = 310;
        const int DIALOG_HEIGHT = 155;
    }
}
