using Gdk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class Keyboard
    {
        internal static bool IsEnterPressed(EventKey eventKey)
        {
            Key key = eventKey.Key;

            return key == Key.Return
                || key == Key.ISO_Enter
                || key == Key.Key_3270_Enter
                || key == Key.KP_Enter;
        }

        internal static bool IsSpacePressed(EventKey eventKey)
        {
            Key key = eventKey.Key;

            return key == Key.space
                || key == Key.KP_Space;
        }
    }
}
