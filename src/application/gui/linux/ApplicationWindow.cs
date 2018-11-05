using System;

using Gtk;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Linux.UI;

namespace Codice.Examples.GuiTesting.Linux
{
    internal class ApplicationWindow : Window
    {
        internal ApplicationWindow(ApplicationOperations operations)
            : base(WindowType.Toplevel)
        {
            mOperations = operations;

            InitializeWindow();
            BuildComponents();
        }

        void AddButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void RemoveButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ApplicationWindow_DeleteEvent(object sender, DeleteEventArgs e)
        {
            WindowHandler.UnregisterApplicationWindow();
        }

        void InitializeWindow()
        {
            Title = Localization.GetText(Localization.Name.ApplicationName);

            DeleteEvent += ApplicationWindow_DeleteEvent;
        }

        void BuildComponents()
        {
            VBox result = new VBox();

            ControlPacker.Add(result, BuildTextEntryBox());
            ControlPacker.Add(result, BuildButtonsBox());
            //ControlPacker.Fill(result, BuildListBox());
            //ControlPacker.Add(result, BuildProgressTextBox());

            mAddButton.Clicked += AddButton_Clicked;
            mRemoveButton.Clicked += RemoveButton_Clicked;

            Add(result);
        }

        HBox BuildTextEntryBox()
        {
            HBox result = new HBox();

            Label textEntryLabel = ControlBuilder.CreateLabel(
                Localization.GetText(Localization.Name.TextInputLabel));
            AlignmentBuilder.SetRightAlignment(textEntryLabel);

            mTextEntry = ControlBuilder.CreateEntry();

            ControlPacker.Fill(result, textEntryLabel);
            ControlPacker.Add(result, AlignmentBuilder.RightPadding(
                mTextEntry, AlignmentBuilder.SMALL_PADDING));

            return result;
        }

        HBox BuildButtonsBox()
        {
            HBox result = new HBox();

            mAddButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.AddButton));
            mRemoveButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.RemoveButton));

            HBox buttonsBox = new HBox();
            ControlPacker.Add(
                buttonsBox,
                AlignmentBuilder.RightPadding(
                    mAddButton, AlignmentBuilder.SMALL_PADDING));
            ControlPacker.Add(
                buttonsBox,
                AlignmentBuilder.RightPadding(
                    mRemoveButton, AlignmentBuilder.SMALL_PADDING));

            ControlPacker.Add(result, AlignmentBuilder.RightAlignment(buttonsBox));

            return result;
        }

        //HBox BuildListBox()
        //{

        //}

        //HBox BuildProgressTextBox()
        //{

        //}

        Entry mTextEntry;
        Button mAddButton;
        Button mRemoveButton;
        Label mProgressLabel;

        readonly ApplicationOperations mOperations;
    }
}
