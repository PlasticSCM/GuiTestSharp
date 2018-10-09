using System.Drawing;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.Windows.UI
{
    internal class ProgressControls : IProgressControls
    {
        internal ProgressControls(Label progressLabel, Control[] controls)
        {
            mProgressLabel = progressLabel;
            mControls = controls;
        }

        void IProgressControls.ShowProgress(string message)
        {
            mProgressLabel.ForeColor = Color.Black;
            mProgressLabel.Text = message;
            mProgressLabel.Show();

            mFocusedControl = GetFocusedControl(mControls);

            DisableControls(mControls);
        }

        void IProgressControls.HideProgress()
        {
            mProgressLabel.Hide();

            EnableControls(mControls);

            if (mFocusedControl != null)
                mFocusedControl.Focus();
        }

        void IProgressControls.ShowError(string message)
        {
            mProgressLabel.ForeColor = Color.Red;
            mProgressLabel.Text = message;
            mProgressLabel.Show();
        }

        static void EnableControls(Control[] controls)
        {
            foreach (Control control in controls)
                control.Enabled = true;
        }

        static void DisableControls(Control[] controls)
        {
            foreach (Control control in controls)
                control.Enabled = false;
        }

        static Control GetFocusedControl(Control[] controls)
        {
            foreach (Control control in controls)
            {
                if (control.Focused)
                    return control;
            }

            return null;
        }

        Control mFocusedControl;

        readonly Label mProgressLabel;
        readonly Control[] mControls;
    }
}
