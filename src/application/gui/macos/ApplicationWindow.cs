using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using Codice.Examples.GuiTesting.Lib;
using Codice.Examples.GuiTesting.MacOS.UI;

namespace Codice.Examples.GuiTesting.MacOS
{
    public class ApplicationWindow : NSWindow
    {
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

            SetFrame(new CoreGraphics.CGRect(0, 0, 200, 400), true);
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

        #region Event handlers
        void Window_WillClose(object sender, EventArgs e)
        {
            WindowHandler.UnregisterApplicationWindow();
        }

        void AddButton_Activated(object sender, EventArgs e)
        {

        }

        void RemoveButton_Activated(object sender, EventArgs e)
        {

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

            NSViewPacker.PackViews(
                result,
                new string[]
                {
                    "H:|-[label(40)]-[textField]-|",
                    "H:[removeButton(80)]-[addButton(80)]-|",
                    "V:|-[label]-[removeButton]-|",
                    "V:|-[textField]-[removeButton]-|",
                    "V:|-[label]-[addButton]-|"
                },
                new NSDictionary(
                    "label", label,
                    "textField", mTextField,
                    "removeButton", mRemoveButton,
                    "addButton", mAddButton)
                );

            mAddButton.Activated += AddButton_Activated;
            mRemoveButton.Activated += RemoveButton_Activated;

            return result;
        }
        #endregion

        NSTextField mTextField;
        NSButton mRemoveButton;
        NSButton mAddButton;
        NSTableView mTableView;

        NSTextField mProgressTextField;

        readonly ApplicationOperations mOperations;
    }
}
