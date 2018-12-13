using System;
using System.Threading;

using AppKit;
using Foundation;
using ObjCRuntime;

namespace Codice.Examples.GuiTesting.MacOS.Testing
{
    internal class TestHelper : NSObject
    {
        internal string GetText(NSTextField textField)
        {
            string result = string.Empty;
            InvokeOnMainThread(() => { result = textField.StringValue; });
            return result;
        }

        internal void SetText(NSTextField textField, string text)
        {
            InvokeOnMainThread(() => { textField.StringValue = text; });
        }

        internal void ClickButton(NSButton button)
        {
            BeginInvokeOnMainThread(() => { button.PerformClick(this); });
        }

        internal void CLickDialogButton(NSButton button)
        {
            InvokeOnMainThread(() => { button.PerformClick(this); });
        }

        internal bool IsEnabled(NSControl control)
        {
            bool result = false;
            InvokeOnMainThread(() => { result = control.Enabled; });
            return result;
        }

        internal bool IsEditable(NSTextView textView)
        {
            bool result = false;
            InvokeOnMainThread(() => { result = textView.Editable; });
            return result;
        }

        internal int GetItemCount(NSTableView tableView)
        {
            int result = 0;
            InvokeOnMainThread(() =>
            {
                result = (int)tableView.RowCount;
            });
            return result;
        }

        internal string GetItemAt(NSTableView tableView, string columnName, nint rowIndex)
        {
            NSTextField textField = null;
            WaitForViewToBeCreated(() =>
            {
                textField = GetFirstTextField(GetTableCellView(tableView, columnName, rowIndex));
                return textField;
            });

            string result = string.Empty;
            InvokeOnMainThread(() => { result = textField.StringValue; });

            return result;
        }

        void WaitForViewToBeCreated(Func<NSView> func)
        {
            int currentWait = 0;
            do
            {
                Thread.Sleep(RETRY_TIME_INTERVAL);
                if (func() != null)
                    return;

                currentWait += RETRY_TIME_INTERVAL;
            } while (currentWait <= MAX_WAIT_TIME);

            throw new Exception(
                "Max wait time exceeded waiting for view to be created.");
        }

        internal string GetAlertTitle(NSAlert alert)
        {
            string result = string.Empty;
            InvokeOnMainThread(() => { result = alert.MessageText; });
            return result;
        }

        internal string GetAlertMessage(NSAlert alert)
        {
            string result = string.Empty;
            InvokeOnMainThread(() => { result = alert.InformativeText; });
            return result;
        }

        NSTextField GetFirstTextField(NSView view)
        {
            if (view == null)
                return null;

            NSView currentView = null;
            for (int i = 0; i < GetSubviewsCount(view); i++)
            {
                currentView = GetSubviewAt(view, i);

                if (currentView is NSTextField textField)
                    return textField;
            }

            return null;
        }

        NSView GetTableCellView(NSTableView tableView, string columnName, nint rowIndex)
        {
            nint columnIndex = GetColumnIndex(tableView, columnName);

            NSView view = null;
            InvokeOnMainThread(() =>
            {
                if (rowIndex >= GetItemCount(tableView))
                    return;

                tableView.ScrollRowToVisible(rowIndex);
                view = tableView.GetView(columnIndex, rowIndex, false);
            });

            return view;
        }

        nint GetColumnIndex(NSTableView table, string columnName)
        {
            nint result = -1;
            if (table == null)
                return result;

            InvokeOnMainThread(() =>
            {
                result = table.FindColumn(new NSString(columnName));
            });

            return result;
        }

        NSView GetSubviewAt(NSView parent, int index)
        {
            NSView result = null;
            if (parent == null)
                return result;

            InvokeOnMainThread(() =>
            {
                if (parent.Subviews == null)
                    return;

                result = parent.Subviews[index];
            });

            return result;
        }

        int GetSubviewsCount(NSView view)
        {
            int result = -1;
            if (view == null)
                return result;

            InvokeOnMainThread(() =>
            {
                if (view.Subviews == null)
                    return;

                result = view.Subviews.Length;
            });

            return result;
        }

        const int RETRY_TIME_INTERVAL = 100;
        const int MAX_WAIT_TIME = 20000;
    }
}
