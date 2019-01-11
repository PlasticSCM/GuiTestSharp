using System;
using System.Collections.Generic;

using AppKit;
using Foundation;

using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.Lib.Interfaces;
using Codice.Examples.GuiTesting.MacOS.UI;

namespace Codice.Examples.GuiTesting.MacOS
{
    public class ApplicationWindow : NSWindow, IApplicationWindow
    {
        internal NSTextField TextField { get { return mTextField; } }
        internal NSButton RemoveButton { get { return mRemoveButton; } }
        internal NSButton AddButton { get { return mAddButton; } }
        internal NSTableView TableView { get { return mTableView; } }
        internal ProgressControls ProgressControls { get { return mProgressControls; } }

        public ApplicationWindow(ApplicationOperations operations) : base()
        {
            mOperations = operations;
            Initialize();
        }

        void Initialize()
        {
            Title = Localization.GetText(Localization.Name.ApplicationName);
            StyleMask = NSWindowStyle.Closable
                | NSWindowStyle.Miniaturizable
                | NSWindowStyle.Resizable
                | NSWindowStyle.Titled;

            AutorecalculatesKeyViewLoop = true;

            ContentView = BuildComponents();

            SetFrame(new CoreGraphics.CGRect(0, 0, 300, 375), true);
            Center();

            this.WillClose += Window_WillClose;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                mAddButton.Activated -= AddButton_Activated;
                mRemoveButton.Activated -= RemoveButton_Activated;
            }

            base.Dispose(disposing);
        }

        #region  IApplicationWindow methods
        void IApplicationWindow.UpdateItems(List<string> items)
        {
            StringDataSource tableViewDataSource = new StringDataSource(items);
            ApplicationWindowTableViewDelegate tableViewDelegate =
                new ApplicationWindowTableViewDelegate(tableViewDataSource);

            mTableView.DataSource = tableViewDataSource;
            mTableView.Delegate = tableViewDelegate;
        }

        void IApplicationWindow.ClearInput()
        {
            mTextField.StringValue = string.Empty;
        }
        #endregion

        #region Event handlers
        void Window_WillClose(object sender, EventArgs e)
        {
            WindowHandler.UnregisterApplicationWindow();
        }

        void AddButton_Activated(object sender, EventArgs e)
        {
            mOperations.AddElement(mTextField.StringValue, this, mProgressControls);
        }

        void RemoveButton_Activated(object sender, EventArgs e)
        {
            mOperations.RemoveElement(mTextField.StringValue, this, mProgressControls);
        }
        #endregion

        #region UI building code
        NSView BuildComponents()
        {
            NSView result = new NSView();

            NSTextField label = NSBuilder.CreateTextField(
                Localization.GetText(Localization.Name.TextInputLabel),
                NSTextAlignment.Right);
            mTextField = NSBuilder.CreateInputTextField();

            mAddButton = NSBuilder.CreateRoundButton(
                Localization.GetText(Localization.Name.AddButton));
            mRemoveButton = NSBuilder.CreateRoundButton(
                Localization.GetText(Localization.Name.RemoveButton));

            NSScrollView scrollView = NSBuilder.CreateScrollView(false);

            mTableView = NSBuilder.CreateTableView();
            mTableView.AddColumn(NSBuilder.CreateColumn(
                "Text", // TODO move constant elsewhere
                250));  // TODO move constant elsewhere

            scrollView.DocumentView = mTableView;

            mProgressTextField = NSBuilder.CreateTextField();
            mProgressTextField.Hidden = true;

            NSViewPacker.PackViews(
                result,
                new string[]
                {
                    "H:|-[label(40)]-[textInput]-|",
                    "H:[removeButton(80)]-[addButton(80)]-|",
                    "H:|-[list]-|",
                    "H:|-[progressText]-|",
                    "V:|-[label]-[removeButton]-[list(200)]-[progressText]-|",
                    "V:|-[textInput]-[removeButton]-[list(200)]-[progressText]-|",
                    "V:|-[label]-[addButton]-[list(200)]-[progressText]-|"
                },
                new NSDictionary(
                    "label", label,
                    "textInput", mTextField,
                    "removeButton", mRemoveButton,
                    "addButton", mAddButton,
                    "list", scrollView,
                    "progressText", mProgressTextField)
                );

            mProgressControls = new ProgressControls(
                this,
                mProgressTextField,
                new NSView[] { mTextField, mAddButton, mRemoveButton, mTableView });

            mAddButton.Activated += AddButton_Activated;
            mRemoveButton.Activated += RemoveButton_Activated;

            return result;
        }
        #endregion

        ProgressControls mProgressControls;

        NSTextField mTextField;
        NSButton mRemoveButton;
        NSButton mAddButton;
        NSTableView mTableView;

        NSTextField mProgressTextField;

        readonly ApplicationOperations mOperations;

        #region TableView related classes
        class StringDataSource : NSTableViewDataSource
        {
            internal string this[int index]
            {
                get
                {
                    return mData[index];
                }
            }

            internal StringDataSource(List<string> data)
            {
                mData = data;
            }

            public override nint GetRowCount(NSTableView tableView)
            {
                return mData.Count;
            }

            readonly List<string> mData;
        }

        class ApplicationWindowTableViewDelegate : NSTableViewDelegate
        {
            internal ApplicationWindowTableViewDelegate(
                StringDataSource dataSource)
            {
                mDataSource = dataSource;
            }

            public override nfloat GetRowHeight(NSTableView tableView, nint row)
            {
                return DEFAULT_ROW_HEIGHT;
            }

            public override NSView GetViewForItem(
                NSTableView tableView, NSTableColumn tableColumn, nint row)
            {
                // If you had a more complex object per row, you would use the column
                // to know which property of the object you should use.
                return NSBuilder.CreateTableRow(tableView.Frame, mDataSource[(int)row]);
            }

            readonly StringDataSource mDataSource;
            static readonly nfloat DEFAULT_ROW_HEIGHT = 23.0f;
        }
        #endregion
    }
}
