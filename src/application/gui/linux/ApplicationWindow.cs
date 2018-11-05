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

            Add(AlignmentBuilder.LeftRightPadding(
                result, AlignmentBuilder.SMALL_PADDING));
        }

        Widget BuildTextEntryBox()
        {
            HBox result = new HBox();

            Label textEntryLabel = ControlBuilder.CreateLabel(
                Localization.GetText(Localization.Name.TextInputLabel));
            AlignmentBuilder.SetRightAlignment(textEntryLabel);

            mTextEntry = ControlBuilder.CreateEntry();

            ControlPacker.Add(result, AlignmentBuilder.RightPadding(
                textEntryLabel, AlignmentBuilder.SMALL_PADDING));
            ControlPacker.Fill(result, mTextEntry);

            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }

        Widget BuildButtonsBox()
        {
            HBox result = new HBox();

            mAddButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.AddButton));
            mRemoveButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.RemoveButton));
                
            ControlPacker.PackActionButtons(
                result,
                AlignmentBuilder.SMALL_PADDING,
                mAddButton, mRemoveButton);

            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }

        //Widget BuildListBox()
        //{

        //}

        //Widget BuildProgressTextBox()
        //{

        //}

        Entry mTextEntry;
        Button mAddButton;
        Button mRemoveButton;
        Label mProgressLabel;

        readonly ApplicationOperations mOperations;
    }
}
