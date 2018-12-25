using Gtk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class AlignmentBuilder
    {
        internal const int SMALL_PADDING = 5;

        internal static void SetLeftAlignment(Label label)
        {
#if !NETCORE
            label.SetAlignment(0, (float)0.5);
#else
            label.Xalign = 0;
            label.Yalign = 0.5f;
#endif
        }

        internal static void SetRightAlignment(Label label)
        {
#if !NETCORE
            label.SetAlignment(1, (float)0.5);
#else
            label.Xalign = 1;
            label.Yalign = 0.5f;
#endif
        }

        internal static Alignment LeftPadding(Widget widget, uint leftPadding)
        {
            return Padding(widget, 0, 0, leftPadding, 0);
        }

        internal static Alignment RightPadding(Widget widget, uint rightPadding)
        {
            return Padding(widget, 0, 0, 0, rightPadding);
        }

        internal static Alignment LeftRightPadding(Widget widget, uint leftRightPadding)
        {
            return Padding(widget, 0, 0, leftRightPadding, leftRightPadding);
        }

        internal static Alignment TopBottomPadding(Widget widget, uint topBottomPadding)
        {
            return Padding(widget, topBottomPadding, topBottomPadding, 0, 0);
        }

        internal static Alignment RightAlignment(Widget widget)
        {
            Alignment result = new Alignment(1, 0, 0, 0);
            result.Add(widget);

            return result;
        }

        static Alignment Padding(
            Widget widget,
            uint topPadding,
            uint bottomPadding,
            uint leftPadding,
            uint rightPadding)
        {
            Alignment result = new Alignment(1, 1, 1, 1);
            result.TopPadding = topPadding;
            result.BottomPadding = bottomPadding;
            result.LeftPadding = leftPadding;
            result.RightPadding = rightPadding;
            result.Add(widget);

            return result;
        }
    }
}
