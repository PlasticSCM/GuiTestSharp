using System;
using System.Threading;

using Gtk;
using log4net;

using Codice.Examples.GuiTesting.Linux.UI;

namespace Codice.Examples.GuiTesting.Linux.Testing
{
    internal static class TestHelper
    {
        internal static string GetText(Entry entry)
        {
            string result = string.Empty;
            GtkGuiActionRunner.RunGuiAction(() =>
            {
                result = entry.Text;
            });

            return result;
        }

        internal static string GetText(Label label)
        {
            string result = string.Empty;
            GtkGuiActionRunner.RunGuiAction(() =>
            {
                result = label.Text;
            });

            return result;
        }

        internal static string GetTitle(Window window)
        {
            string result = string.Empty;
            GtkGuiActionRunner.RunGuiAction(() =>
            {
                result = window.Title;
            });

            return result;
        }

        internal static void SetText(Entry entry, string text)
        {
            GtkGuiActionRunner.RunGuiAction(() =>
            {
                entry.Text = text;
            });
        }

        internal static void ClickButton(Button button)
        {
            GtkGuiActionRunner.RunGuiAction(() =>
            {
                button.Click();
            });
        }

        internal static void ClickDialogButton(Button button)
        {
            GtkGuiActionRunner.RunDialogAction(() =>
            {
                button.Click();
            });
        }

        internal static bool IsSensitive(Widget widget)
        {
            bool result = false;
            GtkGuiActionRunner.RunGuiAction(() =>
            {
                result = widget.Sensitive;
            });

            return result;
        }

        internal static int GetItemCount<T>(GtkListView<T> listView)
        {
            int result = -1;

            GtkGuiActionRunner.RunGuiAction(() =>
            {
                if (listView.View.Model == null)
                {
                    result = 0;
                    return;
                };

                result = listView.View.Model.IterNChildren();
            });

            return result;
        }

        internal static T GetItemAt<T>(GtkListView<T> listView, int index)
        {
            T result = default(T);

            GtkGuiActionRunner.RunGuiAction(() =>
            {
                TreeView tree = listView.View;
                TreeIter treeIter;
                if (!tree.Model.IterNthChild(out treeIter, index))
                    return;

                result = (T)tree.Model.GetValue(treeIter, 0);
            });

            return result;
        }

        internal static class GtkGuiActionRunner
        {
            internal static void RunGuiAction(System.Action action)
            {
                if (!IsInvokeNeeded())
                {
                    action?.Invoke();
                    return;
                }

                ManualResetEvent syncEvent = new ManualResetEvent(false);

                Application.Invoke(delegate
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogException("GUI action failed", ex);
                        throw;
                    }
                    finally
                    {
                        syncEvent.Set();
                    }
                });

                syncEvent.WaitOne();
            }

            internal static void RunDialogAction(System.Action action)
            {
                if (!IsInvokeNeeded())
                {
                    action?.Invoke();
                    return;
                }

                ManualResetEvent syncEvent = new ManualResetEvent(false);

                Application.Invoke(delegate
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogException("Dialog action failed", ex);
                        throw;
                    }
                    finally
                    {
                        syncEvent.Set();
                    }
                });
            }

            static bool IsInvokeNeeded()
            {
                return Thread.CurrentThread.ManagedThreadId != 1;
            }

            static void LogException(string message, Exception ex)
            {
                mLog.ErrorFormat("{0}: {1}", message, ex.Message);
                mLog.ErrorFormat("StackTrace:{0}{1}", Environment.NewLine, ex.StackTrace);
            }

            static readonly ILog mLog = LogManager.GetLogger("GktGuiActionRunner");
        }
    }
}
