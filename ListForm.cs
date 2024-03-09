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
            //ListViewSource = sourceItems;

            listView.VirtualListSize = ListViewSource.Count();

            if (ListViewSource.Any()) {
                // TODO: CMIC
                Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize) + 30;

                // TODO: Check max size in all items
                Width = listView.GetItemRect(0).Width + 40;

                // Select the first item, which makes its color change automatically?
                listView.SelectedIndices.Clear();
                listView.SelectedIndices.Add(0);
            }
            else {
                // TODO: test what if removing this if-else branch
                Height = 0;
                listView.Columns[0].Width = 0;
                Width = 100;
            }
            listView.Refresh();
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
            listView.SelectedIndices.Clear();
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
            listView.SelectedIndices.Clear();
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

        internal void ShowAt(int x, int y)
        {
            Location = new Point(x, y);
            Visible = true;

            if (!ListViewSource.Any()) return;

            listView.EnsureVisible(listView.SelectedIndices[0]);

            // TODO: CMIC
            Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize) + 30;

            // TODO: Resize Column here?
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // TODO: If not using AutoReseize, check max size in all items
            Width = listView.GetItemRect(0).Width + 40;
        }

        // Key down event in listView makes focus on MainForm
        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            ListViewKeyDown?.Invoke(e);
        }
    }
}
