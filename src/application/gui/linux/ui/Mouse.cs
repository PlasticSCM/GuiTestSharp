using Gdk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class Mouse
    {
        internal static bool IsRightMouseButtonPressed(EventButton eventButton)
        {
            return eventButton.Button == RIGTH_MOUSE_BUTTON;
        }

        const int RIGTH_MOUSE_BUTTON = 3;
    }
}
