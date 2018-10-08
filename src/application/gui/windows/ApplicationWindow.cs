using System;
using System.Windows.Forms;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.Windows.UI;

namespace Codice.Examples.GuiTesting.Windows
{
    internal class ApplicationWindow : Form, IApplicationWindow
    {
        internal ApplicationWindow()
        {
            BuildComponents();
        }

        void IApplicationWindow.AddItemToList(string item)
        {
            throw new NotImplementedException();
        }

        void IApplicationWindow.RemoveItemFromList(string item)
        {
            throw new NotImplementedException();
        }

        void IApplicationWindow.ClearInput()
        {
            throw new NotImplementedException();
        }

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

        IProgressControls mProgressControls;

        TextBox mTextBox;
        Button mRemoveButton;
        Button mAddButton;
        ListBox mListBox;

        Label mProgressLabel;
    }
}
