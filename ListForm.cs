using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MiLauncher
{
    public partial class ListForm : Form
    {
        private IEnumerable<FileStats> ListViewSource { get; set; }
        public Action<KeyEventArgs> ListViewKeyDown;

        // TODO: CMIC
        const int maxLineListView = 30;

        public ListForm()
        {
            InitializeComponent();
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (listView.SelectedIndices.Contains(e.Item.Index)) {
                // TODO: CMIC
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            }
            e.DrawText();
        }

        internal void SetVirtualList(IEnumerable<FileStats> sourceItems,
                                     SortKeyOption sortKey = SortKeyOption.Priority)
        {
            ListViewSource = sourceItems.OrderByDescending(x => x.SortValue(sortKey)).ToList();
            listView.VirtualListSize = ListViewSource.Count();

            //if (ListViewSource.Any()) {
            // TODO: CMIC
            //Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize) + 30;
            // TODO: CMIC
            //Width = listView.GetItemRect(0).Width + 40;

            //listView.SelectedIndices.Clear();

            // Select the first item, which makes its color change automatically
            // To add() SelectedIndices, listView requires focus on, which is explained in MSDN
            // listView.SelectedIndices.Add(0);
            //}
            //else {
            //Height = 0;
            //listView.Columns[0].Width = 0;
            //// TODO: CMIC
            //Width = 100;
            //}
            //listView.Refresh();
        }

        internal void SortVirtualList(SortKeyOption sortKey)
        {
            SetVirtualList(ListViewSource, sortKey);
        }

        internal string ExecItem()
        {
            if (Visible & listView.VirtualListSize > 0) {
                try {
                    FileStats selectedFileInfo = ListViewSource.Skip(listView.SelectedIndices[0]).First();
                    Process.Start("explorer.exe", selectedFileInfo.FullPathName);
                    Visible = false;
                    return selectedFileInfo.FullPathName;
                }
                catch (FileNotFoundException) {
                    Debug.WriteLine("File Not Found");
                }
            }
            return string.Empty;
        }

        private void listView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(ListViewSource.Skip(e.ItemIndex).First().FullPathName);
        }

        internal void SelectNextItem()
        {
            if (listView.VirtualListSize == 0) return;

            var newSelectedIndex = (listView.SelectedIndices[0] + 1) % listView.VirtualListSize;
            // With MultiSelect false, adding a new index automatically removes old one
            listView.SelectedIndices.Add(newSelectedIndex);
            listView.EnsureVisible(newSelectedIndex);

            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            // TODO: CMIC
            Width = listView.GetItemRect(0).Width + 40;

            //if (listView.SelectedIndices.Count == 0) {
            //    listView.SelectedIndices.Add(0);
            //    listView.EnsureVisible(0);
            //}
            //else {
            //    var newSelectedIndex = (listView.SelectedIndices[0] + 1) % listView.VirtualListSize;
            //    listView.SelectedIndices.Clear();
            //    listView.SelectedIndices.Add(newSelectedIndex);
            //    listView.EnsureVisible(newSelectedIndex);
            //}

            //// TODO: Create another method to adjust listForm size including Height, which tends to be too big
            //listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //Width = listView.GetItemRect(0).Width + 40;
        }

        internal void SelectPreviousItem()
        {
            if (listView.VirtualListSize == 0) return;

            var newSelectedIndex = (listView.SelectedIndices[0] > 0) ? listView.SelectedIndices[0] - 1 : listView.VirtualListSize - 1;
            // With MultiSelect false, adding a new index automatically removes old one
            listView.SelectedIndices.Add(newSelectedIndex);
            listView.EnsureVisible(newSelectedIndex);

            // TODO: Create another method to adjust listForm size including Height, which tends to be too big
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            Width = listView.GetItemRect(0).Width + 40;

            //if (listView.SelectedIndices.Count == 0) {
            //    listView.SelectedIndices.Add(0);
            //    listView.EnsureVisible(0);
            //}
            //else {
            //    var newSelectedIndex = (listView.SelectedIndices[0] > 0) ? listView.SelectedIndices[0] - 1 : listView.VirtualListSize - 1;
            //    listView.SelectedIndices.Clear();
            //    listView.SelectedIndices.Add(newSelectedIndex);
            //    listView.EnsureVisible(newSelectedIndex);
            //}

            //// TODO: Create another method to adjust listForm size including Height, which tends to be too big
            //listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //Width = listView.GetItemRect(0).Width + 40;
        }

        internal void ShowAt(int? x = null, int? y = null)
        {
            Location = new Point(x ?? Location.X, y ?? Location.Y);
            Visible = true;

            if (ListViewSource.Any()) {
                // TODO: CMIC
                Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize) + 30;
                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                Width = listView.GetItemRect(0).Width + 40;

                listView.SelectedIndices.Clear();
                // Select the first item, which makes its color change automatically
                // To add() SelectedIndices, listView requires focus on, which is explained in MSDN
                listView.SelectedIndices.Add(0);
            }
            else {
                Height = 0;
                listView.Columns[0].Width = 0;
                // TODO: CMIC
                Width = 100;
            }
            listView.Refresh();

            //if (!ListViewSource.Any()) return;
            //// TODO: CMIC
            //Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize) + 30;
            //listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            //// TODO: CMIC
            //Width = listView.GetItemRect(0).Width + 40;
            //listView.EnsureVisible(listView.SelectedIndices[0]);
        }

        // Key down event in listView makes focus on MainForm
        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            ListViewKeyDown?.Invoke(e);
        }
    }
}
