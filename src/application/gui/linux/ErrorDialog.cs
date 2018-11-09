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

        void BuildComponents(string message)
        {
            HBox messageBox = new HBox();

            mMessageLabel = ControlBuilder.CreateExplanationLabel(message);
            Image messageImage = Image.NewFromIconName(
                "dialog-error", IconSize.Dialog);

            ControlPacker.Add(messageBox, messageImage);
            ControlPacker.Fill(
                messageBox,
                AlignmentBuilder.LeftPadding(
                    mMessageLabel, AlignmentBuilder.SMALL_PADDING));

            AddComponents(
                AlignmentBuilder.TopBottomPadding(
                    messageBox, AlignmentBuilder.SMALL_PADDING));

            mOkButton = CreateOkButton(
                Localization.GetText(Localization.Name.Ok));

            DefaultResponse = (ResponseType)GetResponseForWidget(mOkButton);
        }

        Button mOkButton;
        Label mMessageLabel;

        const int DIALOG_WIDTH = 360;
        const int DIALOG_HEIGHT = 160;
    }
}
