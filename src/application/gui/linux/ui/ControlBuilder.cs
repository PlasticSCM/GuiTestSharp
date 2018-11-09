using Gtk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class ControlBuilder
    {
        internal static Button CreateButton(string text)
        {
            Button result = new Button(text);
            result.Relief = ReliefStyle.Normal;
            result.SetSizeRequest(DEFAULT_BUTTON_WIDTH, DEFAULT_BUTTON_HEIGHT);

            return result;
        }

        internal static Entry CreateEntry()
        {
            return new Entry();
        }

        internal static Label CreateExplanationLabel(string text)
        {
            Label result = CreateLabel();
            result.Name = text;
            result.LabelProp = Mono.Unix.Catalog.GetString(text);
            result.Justify = Justification.Left;
            result.LineWrapMode = Pango.WrapMode.Word;
            result.LineWrap = true;
            result.SizeAllocated += (sender, args) =>
            {
                Label label = sender as Label;
                label.SetSizeRequest(args.Allocation.Width - 1, -1);
            };
            return result;
        }

        internal static Label CreateLabel(string text)
        {
            Label result = CreateLabel();
            result.Name = text;
            result.LabelProp = Mono.Unix.Catalog.GetString(text);
            return result;
        }

        internal static Label CreateSelectableLabel()
        {
            Label result = CreateLabel();
            result.Selectable = true;
            return result;
        }

        internal static Label CreateLabel()
        {
            Label result = new Label();
            result.SetPadding(5, 0);
            return result;
        }

        internal static Widget CreateEmptyArea()
        {
            return new HBox(true, 0);
        }

        const int DEFAULT_BUTTON_WIDTH = 100;
        const int DEFAULT_BUTTON_HEIGHT = 34;
    }
}
