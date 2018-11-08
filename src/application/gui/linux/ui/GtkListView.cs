using System.Collections.Generic;

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal class GtkListView<T>
    {
        internal delegate Gtk.TreePath FindRowFunc(Gtk.TreeModel model, T value);

        internal readonly Gtk.TreeView View;
        
        internal bool HeadersVisible
        {
            get { return View.HeadersVisible; }
            set { View.HeadersVisible = value; }
        }

        internal GtkListView(
            List<Gtk.TreeViewColumn> columns,
            Dictionary<int, Gtk.TreeIterCompareFunc> sortFunctionsByColumn,
            Gtk.TreeModelFilterVisibleFunc visibleFunc)
        {
            View = TreeBuilder.CreateTreeView();
            mSortFunctionByColumn = sortFunctionsByColumn;
            mVisibleFunc = visibleFunc;

            foreach (Gtk.TreeViewColumn column in columns)
                View.AppendColumn(column);
        }

        internal void Fill(List<T> values)
        {
            if (values == null)
                return;

            Gtk.ListStore listStore = new Gtk.ListStore(typeof(T));

            values.ForEach((val) => listStore.AppendValues(val));

            mModelFilter = new Gtk.TreeModelFilter(listStore, null);
            mModelFilter.VisibleFunc = mVisibleFunc;

            mModelSort = new Gtk.TreeModelSort(mModelFilter);
            SetSortFunctions(mModelSort, mSortFunctionByColumn);

            if (View.Model != null)
                (View.Model as Gtk.TreeModelSort).Dispose();

            View.Model = mModelSort;
        }

        internal List<T> GetSelected()
        {
            List<T> result = new List<T>();

            if (View.Selection.CountSelectedRows() == 0)
                return result;

            foreach (Gtk.TreePath path in View.Selection.GetSelectedRows())
                result.Add(GetObjectFromTreePath(mModelSort, path));

            return result;
        }

        static T GetObjectFromTreePath(Gtk.TreeModelSort modelSort, Gtk.TreePath path)
        {
            Gtk.TreeIter iter;
            modelSort.GetIter(out iter, path);
            return GetObjectFromTreeIter(modelSort, iter);
        }

        static T GetObjectFromTreeIter(Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            return (T)model.GetValue(iter, 0);
        }

        static void SetSortFunctions(
            Gtk.TreeModelSort treeModelSort,
            Dictionary<int, Gtk.TreeIterCompareFunc> sortFunctions)
        {
            foreach (int sortColumnId in sortFunctions.Keys)
                treeModelSort.SetSortFunc(sortColumnId, sortFunctions[sortColumnId]);
        }

        Gtk.TreeModelFilter mModelFilter;
        Gtk.TreeModelSort mModelSort;

        readonly Dictionary<int, Gtk.TreeIterCompareFunc> mSortFunctionByColumn;
        readonly Gtk.TreeModelFilterVisibleFunc mVisibleFunc;
    }
}
