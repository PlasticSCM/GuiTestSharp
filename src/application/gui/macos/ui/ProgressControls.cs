using AppKit;

using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.MacOS.UI
{
    internal class ProgressControls : IProgressControls
    {
        public bool HasError { get { return mbHasError; } }
        public NSTextField ProgressTextField { get { return mProgressTextField; } }

        internal ProgressControls(
            NSWindow window, NSTextField progressTextField, NSView[] views)
        {
            mWindow = window;
            mProgressTextField = progressTextField;
            mViews = views;
        }

        void IProgressControls.ShowProgress(string message)
        {
            mbHasError = false;

            mProgressTextField.TextColor = NSColors.NotificationText;
            mProgressTextField.StringValue = message;
            mProgressTextField.Hidden = false;

            mResponder = NSViewArray.GetFirstResponder(mWindow, mViews);

            NSViewArray.Disable(mViews);
        }

        void IProgressControls.HideProgress()
        {
            mProgressTextField.Hidden = true;

            NSViewArray.Enable(mViews);

            if (mResponder != null)
                mWindow.MakeFirstResponder(mResponder);
        }

        void IProgressControls.ShowError(string message)
        {
            mbHasError = true;

            mProgressTextField.TextColor = NSColors.ErrorText;
            mProgressTextField.StringValue = message;
            mProgressTextField.Hidden = false;
        }

        NSResponder mResponder;
        bool mbHasError = false;

        readonly NSWindow mWindow;
        readonly NSTextField mProgressTextField;
        readonly NSView[] mViews;

        static class NSViewArray
        {
            internal static void Enable(NSView[] views)
            {
                SetEnablement(views, true);
            }

            internal static void Disable(NSView[] views)
            {
                SetEnablement(views, false);
            }

            internal static NSView GetFirstResponder(NSWindow window, NSView[] views)
            {
                if (views == null)
                    return null;

                foreach (NSView view in views)
                {
                    if (window.FirstResponder == view)
                        return view;
                }

                return null;
            }

            static void SetEnablement(NSView[] views, bool enabled)
            {
                if (views == null)
                    return;

                foreach (NSView view in views)
                {
                    if (view == null)
                        continue;

                    NSControl control = view as NSControl;
                    if (control != null)
                    {
                        control.Enabled = enabled;
                        continue;
                    }

                    NSTextView textView = view as NSTextView;
                    if (textView != null)
                    {
                        textView.Selectable = enabled;
                        textView.Editable = enabled;

                        textView.TextColor = enabled
                            ? NSColor.ControlText
                            : NSColor.DisabledControlText;
                    }
                }
            }
        }
    }
}
