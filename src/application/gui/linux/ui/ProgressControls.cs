using Gdk;
using Gtk;

using Codice.Examples.GuiTesting.Lib.Interfaces;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal class ProgressControls : IProgressControls
    {
        public bool HasError { get { return mbHasError; } }
        public Label ProgressLabel { get { return mProgressLabel; } }
        
        internal ProgressControls(Label progressLabel, Widget[] widgets)
        {
            mProgressLabel = progressLabel;
            mWidgets = widgets;
        }

        void IProgressControls.HideProgress()
        {
            mProgressLabel.Visible = false;

            EnableWidgets(mWidgets);

            if (mbFocusedWidget != null)
                mbFocusedWidget.GrabFocus();
        }

        void IProgressControls.ShowError(string message)
        {
            mbHasError = true;

            mProgressLabel.ModifyFg(StateType.Normal, COLOR_RED);
            mProgressLabel.Text = message;
            mProgressLabel.Visible = true;
        }

        void IProgressControls.ShowProgress(string message)
        {
            mbHasError = false;

            mProgressLabel.ModifyFg(StateType.Normal, COLOR_BLACK);
            mProgressLabel.Text = message;
            mProgressLabel.Visible = true;

            mbFocusedWidget = GetFocusedWidget(mWidgets);

            DisableWidgets(mWidgets);
        }

        static void DisableWidgets(Widget[] widgets)
        {
            foreach (Widget widget in  widgets)
                widget.Sensitive = false;
        }

        static void EnableWidgets(Widget[] widgets)
        {
            foreach (Widget widget in  widgets)
                widget.Sensitive = true;
        }

        static Widget GetFocusedWidget(Widget[] widgets)
        {
            foreach (Widget widget in widgets)
            {
                if (widget.HasFocus)
                    return widget;
            }

            return null;
        }

        Widget mbFocusedWidget;
        bool mbHasError;

        readonly Label mProgressLabel;
        readonly Widget[] mWidgets;

        static readonly Color COLOR_RED = new Color(255, 0, 0);
        static readonly Color COLOR_BLACK = new Color(0, 0, 0);
    }
}
