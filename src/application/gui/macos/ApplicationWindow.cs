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
            throw new NotImplementedException();
        }

        void RemoveButton_Activated(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region UI building code
        NSView BuildComponents()
        {
            NSView result = BuildButtonsView();

            mAddButton.Activated += AddButton_Activated;
            mRemoveButton.Activated += RemoveButton_Activated;

            return result;
        }

        void MAddButton_Activated(object sender, EventArgs e)
        {
        }


        NSView BuildTextInputView()
        {
            throw new NotImplementedException();
        }

        NSView BuildButtonsView()
        {

            mAddButton = NSBuilder.CreateRoundButton(
                Localization.GetText(Localization.Name.AddButton));
            mRemoveButton = NSBuilder.CreateRoundButton(
                Localization.GetText(Localization.Name.RemoveButton));

            NSStackView result = new NSStackView();
            result.UserInterfaceLayoutDirection = NSUserInterfaceLayoutDirection.RightToLeft;
            result.AddView(mAddButton, NSStackViewGravity.Trailing);
            result.AddView(mRemoveButton, NSStackViewGravity.Trailing);

            return result;
        }

        NSView BuildTableView()
        {
            throw new NotImplementedException();
        }

        NSView BuildProgressTextPanel()
        {
            throw new NotImplementedException();
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
