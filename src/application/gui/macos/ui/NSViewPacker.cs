using System.Collections.Generic;

using AppKit;
using Foundation;

namespace Codice.Examples.GuiTesting.MacOS.UI
{
    internal static class NSViewPacker
    {
        internal static void PackViews(
            NSView parent, string[] constraints, NSDictionary views)
        {
            foreach (NSView view in views.Values)
            {
                view.TranslatesAutoresizingMaskIntoConstraints = false;
                parent.AddSubview(view);
            }

            parent.AddConstraints(BuildConstraints(constraints, views));
        }

        internal static void Fill(NSView parent, NSView child)
        {
            PackViews(
                parent,
                new string[]
                {
                    "H:|[child]|",
                    "V:|[child]|"
                },
                new NSDictionary("child", child));
        }

        static NSLayoutConstraint[] BuildConstraints(
            string[] constraints, NSDictionary views)
        {
            List<NSLayoutConstraint> result = new List<NSLayoutConstraint>();

            foreach (string format in constraints)
                result.AddRange(NSLayoutConstraint.FromVisualFormat(format, 0, null, views));

            return result.ToArray();
        }
    }
}
