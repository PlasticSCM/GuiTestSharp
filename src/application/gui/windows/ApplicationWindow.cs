using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.Windows.UI;

namespace Codice.Examples.GuiTesting.Windows
{
    internal class ApplicationWindow : Form, IApplicationWindow
    {
        internal TextBox TextBox { get { return mTextBox; } }
        internal Button RemoveButton { get { return mRemoveButton; } }
        internal Button AddButton { get { return mAddButton; } }
        internal ListBox ListBox { get { return mListBox; } }
        internal ProgressControls ProgressControls { get { return mProgressControls; } }

        internal ApplicationWindow(ApplicationOperations operations)
        {
            mOperations = operations;
            BuildComponents();
        }

        protected override void Dispose(bool disposing)
        {
            mAddButton.Click -= AddButton_Click;
            mRemoveButton.Click -= RemoveButton_Click;

            base.Dispose(disposing);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WindowHandler.UnregisterApplicationWindow();
        }

        #region IApplicationWindow methods
        void IApplicationWindow.UpdateItems(List<string> items)
        {
            mListBox.DataSource = items.AsReadOnly();
            mListBox.Update();
        }

        void IApplicationWindow.ClearInput()
        {
            mTextBox.ResetText();
        }
        #endregion

        #region Event handlers
        void AddButton_Click(object sender, EventArgs e)
        {
            mOperations.AddElement(mTextBox.Text, this, mProgressControls);
        }

        void RemoveButton_Click(object sender, EventArgs e)
        {
            mOperations.RemoveElement(mTextBox.Text, this, mProgressControls);
        }
        #endregion

        #region UI building code
        void BuildComponents()
        {
            ControlPacker.AddControl(
                this, ControlBuilder.CreateVerticalPadding(DockStyle.Top));
            ControlPacker.AddControl(this, BuildTextInputPanel());
            ControlPacker.AddControl(
                this, ControlBuilder.CreateVerticalPadding(DockStyle.Top));
            ControlPacker.AddControl(this, BuildButtonsPanel());
            ControlPacker.AddControl(
                this, ControlBuilder.CreateVerticalPadding(DockStyle.Top));
            ControlPacker.AddControl(this, BuildListBoxPanel());
            ControlPacker.AddControl(
                this, ControlBuilder.CreateVerticalPadding(DockStyle.Bottom));
            ControlPacker.AddControl(this, BuildProgressTextPanel());

            mProgressControls = new ProgressControls(
                mProgressLabel,
                new Control[] { mTextBox, mRemoveButton, mAddButton, mListBox });

            mAddButton.Click += AddButton_Click;
            mRemoveButton.Click += RemoveButton_Click;
        }

        Panel BuildTextInputPanel()
        {
            Panel result = ControlBuilder.CreatePanel(
                ControlBuilder.DefaultControlHeight, DockStyle.Top);

            Label textInputLabel = ControlBuilder.CreateLabel(
                Localization.GetText(Localization.Name.TextInputLabel),
                DockStyle.Left);
            mTextBox = ControlBuilder.CreateTextbox(DockStyle.Fill);

            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Left));
            ControlPacker.AddControl(result, textInputLabel);
            ControlPacker.AddControl(result, mTextBox);
            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Right));

            return result;
        }

        Panel BuildButtonsPanel()
        {
            Panel result = ControlBuilder.CreatePanel(
                ControlBuilder.DefaultControlHeight, DockStyle.Top);

            mRemoveButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.RemoveButton),
                DockStyle.Right);

            mAddButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.AddButton),
                DockStyle.Right);

            ControlPacker.AddControl(result, mRemoveButton);
            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Right));
            ControlPacker.AddControl(result, mAddButton);
            ControlPacker.AddControl(
                result, ControlBuilder.CreateHorizontalPadding(DockStyle.Right));

            return result;
        }

        Panel BuildListBoxPanel()
        {
            Panel result = ControlBuilder.CreatePanel(DockStyle.Fill);

            mListBox = ControlBuilder.CreateListBox(DockStyle.Fill);
            mListBox.SelectionMode = SelectionMode.None;

            ControlPacker.AddControl(result, ControlBuilder.CreateHorizontalPadding(DockStyle.Left));
            ControlPacker.AddControl(result, ControlBuilder.CreateHorizontalPadding(DockStyle.Right));
            ControlPacker.AddControl(result, mListBox);

            return result;
        }

        Panel BuildProgressTextPanel()
        {
            Panel result = ControlBuilder.CreatePanel(
                ControlBuilder.DefaultControlHeight, DockStyle.Bottom);

            mProgressLabel = ControlBuilder.CreateLabel(string.Empty, DockStyle.Left);

            ControlPacker.AddControl(result, ControlBuilder.CreateHorizontalPadding(DockStyle.Left));
            ControlPacker.AddControl(result, mProgressLabel);

            return result;
        }
        #endregion

        ProgressControls mProgressControls;

        TextBox mTextBox;
        Button mRemoveButton;
        Button mAddButton;
        ListBox mListBox;

        Label mProgressLabel;

        readonly ApplicationOperations mOperations;
    }
}
