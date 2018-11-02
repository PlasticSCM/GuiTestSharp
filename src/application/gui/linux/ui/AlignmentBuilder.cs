using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gtk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class AlignmentBuilder
    {
        internal const int SMALL_PADDING = 5;

        internal static void SetRightAlignment(Label label)
        {
            label.SetAlignment(1, (float)0.5);
        }

        internal static Alignment RightPadding(Widget widget, uint rightPadding)
        {
            return TopBottomLeftRightPadding(widget, 0, 0, 0, rightPadding);
        }

        internal static Alignment RightAlignment(Widget widget)
        {
            Alignment result = new Alignment(1, 0, 0, 0);
            result.Add(widget);

            return result;
        }

        static Alignment TopBottomLeftRightPadding(
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
