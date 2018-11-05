using Gtk;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal static class TreeBuilder
    {
        internal static TreeView CreateTreeView()
        {
            TreeView result = new TreeView();
            result.FixedHeightMode = true;
            result.ButtonPressEvent += TreeView_ProtectButtonPressMultipleSelection;
            result.KeyPressEvent += TreeView_ProtectActivateMultipleSelection;

            return result;
        }

        internal static TreeViewColumn CreateColumn(
            string label,
            int fixedWidth,
            TreeCellDataFunc renderFunc)
        {
            TreeViewColumn result = new TreeViewColumn()
            {
                Title = label,
                Sizing = TreeViewColumnSizing.Fixed,
                FixedWidth = fixedWidth,
                Resizable = true,
                Reorderable = false,
                SortIndicator = false
            };

            CellRendererText cell = new CellRendererText();
            result.PackStart(cell, true);
            result.SetCellDataFunc(cell, renderFunc);

            return result;
        }

        [GLib.ConnectBefore]
        static void TreeView_ProtectButtonPressMultipleSelection(
            object sender, ButtonPressEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            if (!Mouse.IsRightMouseButtonPressed(e.Event))
            {
                e.RetVal = false;
                return;
            }

            TreePath treePath;
            treeView.GetPathAtPos((int)e.Event.X, (int)e.Event.Y, out treePath);

            foreach (TreePath selectedRow in treeView.Selection.GetSelectedRows())
            {
                if (selectedRow.Equals(treePath))
                {
                    e.RetVal = true;
                    return;
                }
            }

            e.RetVal = false;
        }

        [GLib.ConnectBefore]
        static void TreeView_ProtectActivateMultipleSelection(
            object sender, KeyPressEventArgs args)
        {
            TreeView treeView = sender as TreeView;

            if (Keyboard.IsEnterPressed(args.Event)
                || Keyboard.IsSpacePressed(args.Event))
            {
                treeView.ActivateRow(null, null);
                args.RetVal = true;
                return;
            }

            args.RetVal = false;
        }
    }
}
