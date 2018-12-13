using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace Codice.Examples.GuiTesting.MacOS.UI
{
    internal static class NSBuilder
    {
        internal static readonly nfloat DEFAULT_COLUMN_WIDTH = 250.0f;

        internal static NSButton CreateDefaultButton(string text)
        {
            return CreateButton(text, NSBezelStyle.Rounded);
        }

        internal static NSButton CreateRoundButton(string text)
        {
            return CreateButton(text, NSBezelStyle.RoundRect);
        }

        internal static NSButton CreateButton(string text, NSBezelStyle bezelStyle)
        {
            NSButton result = new NSButton();
            result.Title = text;
            result.BezelStyle = bezelStyle;

            return result;
        }

        internal static NSTextField CreateLabel(string text)
        {
            NSTextField result = CreateTextField(text);
            result.Cell.UsesSingleLineMode = false;
            result.Cell.DrawsBackground = false;

            return result;
        }

        internal static NSTextField CreateTextField()
        {
            return CreateTextField(string.Empty);
        }

        internal static NSTextField CreateTextField(string text)
        {
            return CreateTextField(text, NSTextAlignment.Left);
        }

        internal static NSTextField CreateTextField(
            string text, NSTextAlignment alignment)
        {
            return CreateTextField(text, alignment, NSColor.ControlText);
        }

        internal static NSTextField CreateTextField(
            string text, NSTextAlignment alignment, NSColor color)
        {
            NSTextField result = new NSTextField();
            result.Cell = CreateTextFieldCell(text, alignment, color);
            return result;
        }

        internal static NSTextField CreateInputTextField()
        {
            NSTextField result = new NSTextField();

            NSTextFieldCell cell = new NSTextFieldCell();
            cell.StringValue = string.Empty;
            cell.Scrollable = true;
            result.Cell = cell;

            result.Editable = true;
            result.Bordered = true;
            result.Bezeled = true;

            return result;
        }

        internal static NSScrollView CreateScrollView(bool bHasScrollers = true)
        {
            NSScrollView result = new NSScrollView();
            result.BorderType = NSBorderType.NoBorder;
            result.HasVerticalScroller = bHasScrollers;
            result.HasHorizontalScroller = bHasScrollers;
            result.AutohidesScrollers = bHasScrollers;
            result.AutoresizingMask =
                NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;

            return result;
        }

        internal static NSTableView CreateTableView()
        {
            NSTableView result = new NSTableView();
            result.UsesAlternatingRowBackgroundColors = true;
            result.AllowsEmptySelection = true;

            return result;
        }

        internal static NSTableColumn CreateColumn(string name)
        {
            return CreateColumn(name, DEFAULT_COLUMN_WIDTH);
        }

        internal static NSTableColumn CreateColumn(string name, nfloat width)
        {
            NSTableColumn result = new NSTableColumn(name);
            result.HeaderCell.Title = name;
            result.HeaderToolTip = name;
            if (width > 0.0f)
                result.Width = width;

            return result;
        }

        internal static NSView CreateTableRow(CGRect frame, string text)
        {
            NSTextField textField = CreateTextField(text);
            NSTableCellView result = new NSTableCellView();

            NSViewPacker.PackViews(
                result,
                new string[]
                {
                    "H:|-4-[text]-4-|",
                    "V:|-4-[text(23)]-4-|"
                },
                new NSDictionary(
                    "text", textField)
                );

            return result;
        }

        static NSTextFieldCell CreateTextFieldCell(
            string text, NSTextAlignment alignment, NSColor color)
        {
            NSTextFieldCell result = new NSTextFieldCell(text);
            result.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            result.Alignment = alignment;
            result.TextColor = color;
            return result;
        }
    }
}
