using System.Collections.Generic;

using Gtk;

#if NETCORE
using TreeModel = Gtk.ITreeModel;
#endif

namespace Codice.Examples.GuiTesting.Linux.UI
{
    internal class GtkListView<T>
    {
        internal delegate TreePath FindRowFunc(TreeModel model, T value);

        internal readonly TreeView View;

        internal bool HeadersVisible
        {
            get { return View.HeadersVisible; }
            set { View.HeadersVisible = value; }
        }

        internal GtkListView(
            List<TreeViewColumn> columns,
            Dictionary<int, TreeIterCompareFunc> sortFunctionsByColumn,
            TreeModelFilterVisibleFunc visibleFunc)
        {
            View = TreeBuilder.CreateTreeView();
            mSortFunctionByColumn = sortFunctionsByColumn;
            mVisibleFunc = visibleFunc;

            foreach (TreeViewColumn column in columns)
                View.AppendColumn(column);
        }

        internal void Fill(List<T> values)
        {
            if (values == null)
                return;

            ListStore listStore = new ListStore(typeof(T));

            values.ForEach((val) => listStore.AppendValues(val));

            mModelFilter = new TreeModelFilter(listStore, null);
            mModelFilter.VisibleFunc = mVisibleFunc;

            mModelSort = new TreeModelSort(mModelFilter);
            SetSortFunctions(mModelSort, mSortFunctionByColumn);

            if (View.Model != null)
                (View.Model as TreeModelSort).Dispose();

            View.Model = mModelSort;
        }

        internal List<T> GetSelected()
        {
            List<T> result = new List<T>();

            if (View.Selection.CountSelectedRows() == 0)
                return result;

            foreach (TreePath path in View.Selection.GetSelectedRows())
                result.Add(GetObjectFromTreePath(mModelSort, path));

            return result;
        }

        static T GetObjectFromTreePath(TreeModelSort modelSort, TreePath path)
        {
            TreeIter iter;
            modelSort.GetIter(out iter, path);
            return GetObjectFromTreeIter(modelSort, iter);
        }
        static T GetObjectFromTreeIter(TreeModel model, TreeIter iter)
        {
            return (T)model.GetValue(iter, 0);
        }

        static void SetSortFunctions(
            TreeModelSort treeModelSort,
            Dictionary<int, TreeIterCompareFunc> sortFunctions)
        {
            foreach (int sortColumnId in sortFunctions.Keys)
                treeModelSort.SetSortFunc(sortColumnId, sortFunctions[sortColumnId]);
        }

        TreeModelFilter mModelFilter;
        TreeModelSort mModelSort;

        readonly Dictionary<int, TreeIterCompareFunc> mSortFunctionByColumn;
        readonly TreeModelFilterVisibleFunc mVisibleFunc;
    }
}
