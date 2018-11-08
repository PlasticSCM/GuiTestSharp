using System;
using System.Collections.Generic;

using Gtk;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.Linux.UI;

namespace Codice.Examples.GuiTesting.Linux
{
    internal class ApplicationWindow : Window, IApplicationWindow
    {
        internal ApplicationWindow(ApplicationOperations operations)
            : base(WindowType.Toplevel)
        {
            mOperations = operations;

            InitializeWindow();
            BuildComponents();
        }

        public override void Dispose()
        {
            mAddButton.Clicked -= AddButton_Clicked;
            mRemoveButton.Clicked -= RemoveButton_Clicked;
            DeleteEvent -= ApplicationWindow_DeleteEvent;

            base.Dispose();
        }

        void ApplicationWindow_DeleteEvent(object sender, DeleteEventArgs e)
        {
            WindowHandler.UnregisterApplicationWindow();
        }

        #region IApplicationWindow methods
        void IApplicationWindow.UpdateItems(List<string> items)
        {
            mListView.Fill(items);
        }

        void IApplicationWindow.ClearInput()
        {
            mTextEntry.Text = string.Empty;
        }
        #endregion

        #region Event handlers
        void AddButton_Clicked(object sender, EventArgs e)
        {
            mOperations.AddElement(mTextEntry.Text, this as IApplicationWindow, mProgressControls);
        }

        void RemoveButton_Clicked(object sender, EventArgs e)
        {
            mOperations.RemoveElement(mTextEntry.Text, this as IApplicationWindow, mProgressControls);
        }
        #endregion

        #region UI building code
        void InitializeWindow()
        {
            Title = Localization.GetText(Localization.Name.ApplicationName);

            DeleteEvent += ApplicationWindow_DeleteEvent;
        }

        void BuildComponents()
        {
            VBox result = new VBox();

            ControlPacker.Add(result, BuildTextEntryBox());
            ControlPacker.Add(result, BuildButtonsBox());
            ControlPacker.Fill(result, BuildListViewBox());
            ControlPacker.Add(result, BuildProgressTextBox());

            mProgressControls = new ProgressControls(
                mProgressLabel,
                new Widget[] { mTextEntry, mAddButton, mRemoveButton, mListView.View });

            mAddButton.Clicked += AddButton_Clicked;
            mRemoveButton.Clicked += RemoveButton_Clicked;

            mListView.Fill(new List<string>() { string.Empty });

            Add(AlignmentBuilder.LeftRightPadding(
                result, AlignmentBuilder.SMALL_PADDING));
        }

        Widget BuildTextEntryBox()
        {
            HBox result = new HBox();

            Label textEntryLabel = ControlBuilder.CreateLabel(
                Localization.GetText(Localization.Name.TextInputLabel));
            AlignmentBuilder.SetRightAlignment(textEntryLabel);

            mTextEntry = ControlBuilder.CreateEntry();

            ControlPacker.Add(result, AlignmentBuilder.RightPadding(
                textEntryLabel, AlignmentBuilder.SMALL_PADDING));
            ControlPacker.Fill(result, mTextEntry);

            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }

        Widget BuildButtonsBox()
        {
            HBox result = new HBox();

            mAddButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.AddButton));
            mRemoveButton = ControlBuilder.CreateButton(
                Localization.GetText(Localization.Name.RemoveButton));

            ControlPacker.PackActionButtons(
                result,
                AlignmentBuilder.SMALL_PADDING,
                mAddButton, mRemoveButton);

            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }

        Widget BuildListViewBox()
        {
            HBox result = new HBox();
            result.WidthRequest = LISTBOX_WIDTH;
            result.HeightRequest = LISTBOX_HEIGHT;
            
            mListView = ApplicationWindowListView.Build();
            mListView.HeadersVisible = false;
            
            ControlPacker.Fill(result, mListView.View);

            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }

        Widget BuildProgressTextBox()
        {
            HBox result = new HBox();

            mProgressLabel = ControlBuilder.CreateSelectableLabel();
            ControlPacker.PackProgressLabel(result, mProgressLabel);

            return AlignmentBuilder.TopBottomPadding(
                result, AlignmentBuilder.SMALL_PADDING);
        }
        #endregion

        ProgressControls mProgressControls;

        Entry mTextEntry;
        Button mAddButton;
        Button mRemoveButton;
        GtkListView<string> mListView;
        Label mProgressLabel;

        readonly ApplicationOperations mOperations;

        const int LISTBOX_WIDTH = 250;
        const int LISTBOX_HEIGHT = 200;

        static class ApplicationWindowListView
        {
            enum Columns
            {
                Text
            }

            internal static GtkListView<string> Build()
            {
                var comparer = new RowComparer();
                var renderer = new ColumnRenderer();
                var columns = new List<TreeViewColumn>();

                columns.Add(
                    TreeBuilder.CreateColumn(
                        "Text", // TODO move constant elsewhere
                        250, // TODO move constant elsewhere
                        renderer.RenderText));

                var result = new GtkListView<string>(
                    columns,
                    new Dictionary<int, TreeIterCompareFunc>()
                    {
                        {(int) Columns.Text, comparer.Compare}
                    },
                    null);

                return result;
            }

            class ColumnRenderer
            {
                internal void RenderText(
                    TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
                {
                    // If you had a more complex object per row, you would use the column
                    // to know which property of the object you should use.
                    CellContentRenderer.RenderText(cell, (string) model.GetValue(iter, 0));
                }
            }

            class RowComparer
            {
                internal int Compare(TreeModel model, TreeIter xIter, TreeIter yIter)
                {
                    string x = (string) model.GetValue(xIter, 0);
                    string y = (string) model.GetValue(yIter, 0);

                    return string.Compare(x, y);
                }
            }
        }
    }
}
