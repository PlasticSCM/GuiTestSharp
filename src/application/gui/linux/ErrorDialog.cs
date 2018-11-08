using System;

using Gtk;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Linux.UI;

namespace Codice.Examples.GuiTesting.Linux
{
    internal class ErrorDialog : BaseDialog
    {
        internal ErrorDialog(string title, string message, Window parent)
            : base(title, parent)
        {
            BuildComponents(message);
            SetSizeRequest(DIALOG_WIDTH, DIALOG_HEIGHT);
        }

        public override void Dispose()
        {
            mOkButton.Clicked -= OkButton_Clicked;
            base.Dispose();
        }

        void OkButton_Clicked(object sender, EventArgs e)
        {
            Respond(ResponseType.Ok);
        }

        void BuildComponents(string message)
        {
            AddComponents(
                BuildMessageBox(message),
                BuildButtonBox());

            mOkButton.Clicked += OkButton_Clicked;
        }

        Widget BuildMessageBox(string message)
        {
            HBox result = new HBox();
            
            mMessageLabel = ControlBuilder.CreateLabel(message);
            ControlPacker.Fill(result, mMessageLabel);
            
            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }

        Widget BuildButtonBox()
        {
            HBox result = new HBox();
            
            mOkButton = CreateAcceptButton(Localization.GetText(Localization.Name.Ok));

            ControlPacker.PackActionButtons(
                result,
                AlignmentBuilder.SMALL_PADDING,
                mOkButton);
            
            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }

        Button mOkButton;
        Label mMessageLabel;
        
        const int DIALOG_WIDTH = 360;
        const int DIALOG_HEIGHT = 160;
    }
}
