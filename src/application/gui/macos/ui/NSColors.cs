using AppKit;

namespace Codice.Examples.GuiTesting.MacOS.UI
{
    internal static class NSColors
    {
        internal static readonly NSColor NotificationText = GetColorFromRgb(0, 103, 149);
        internal static readonly NSColor ErrorText = GetColorFromRgb(198, 38, 53);

        internal static NSColor GetColorFromRgb(int r, int g, int b)
        {
            return NSColor.FromCalibratedRgba(
                r / 255.0f, g / 255.0f, b / 255.0f, 1.0f);
        }
    }
}
