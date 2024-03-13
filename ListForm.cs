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
        //
        // Properties
        //
        private List<FileStats> ListViewSource { get; set; }
        private SortKeyOption SortKey { get; set; } = SortKeyOption.Priority;

        private int _virtualListIndex;
        private int VirtualListIndex
        {
            get {
                return _virtualListIndex;
            }
            set {
                _virtualListIndex = PositiveModulo(value, listView.VirtualListSize);

                // With MultiSelect false, adding a new index automatically removes old one
                listView.SelectedIndices.Add(_virtualListIndex);

                // Resize column at first
                // And if GetItemRect(0).Y changed, list view scrolls, then resize column
                var originalScrollPosition = listView.GetItemRect(0).Y;
                listView.EnsureVisible(_virtualListIndex);
                if (_virtualListIndex == 0 || originalScrollPosition != listView.GetItemRect(0).Y)
                    listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                // TODO: CMIC
                Width = listView.GetItemRect(0).Width + 40;

                FileStats selectedFileInfo = ListViewSource[_virtualListIndex];

                if (SortKey == SortKeyOption.FullPathName) {
                    Path.Text = "Path";
                }
                else {
                    Path.Text = string.Format("{0}: {1}", SortKey.ToString(), selectedFileInfo.SortValue(SortKey));
                }
            }
        }
        private static int PositiveModulo(int x, int y)
        {
            int z = x % y;
            return (z >= 0) ? z : z + y;
        }

        //
        // Delegate
        //
        public Action<KeyEventArgs> ListViewKeyDown;

        //
        // Constants
        //
        // TODO: CMIC
        const int maxLineListView = 30;

        public ListForm()
        {
            InitializeComponent();
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (VirtualListIndex == e.Item.Index) {
                // TODO: CMIC
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            }
            e.DrawText();
        }

        internal void SetVirtualList(List<FileStats> sourceItems = null)
        {
            sourceItems ??= ListViewSource;
            ListViewSource = sourceItems.OrderByDescending(x => x.SortValue(SortKey)).ToList();
            listView.VirtualListSize = ListViewSource.Count;
        }

        internal void SortVirtualList(SortKeyOption sortKey)
        {
            SortKey = sortKey;
            SetVirtualList();
        }

        internal string ExecItem()
        {
            if (Visible & listView.VirtualListSize > 0) {
                try {
                    //FileStats selectedFileInfo = ListViewSource.ElementAt(VirtualListIndex);
                    FileStats selectedFileInfo = ListViewSource[VirtualListIndex];
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
            e.Item = new ListViewItem(ListViewSource[e.ItemIndex].FullPathName);
        }

        internal void SelectNextItem()
        {
            if (listView.VirtualListSize == 0) return;

            VirtualListIndex++;
        }

        internal void SelectPreviousItem()
        {
            if (listView.VirtualListSize == 0) return;

            VirtualListIndex--;
        }

        internal void ShowAt(int? x = null, int? y = null)
        {
            Location = new Point(x ?? Location.X, y ?? Location.Y);
            Visible = true;

            if (ListViewSource.Any()) {
                // To add() SelectedIndices, listView requires focus on, which is mentioned by MSDN
                // And changing height seems to focus on list view
                // TODO: CMIC
                Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize + 1) + 30;
                VirtualListIndex = 0;
            }
            else {
                Height = 0;
                listView.Columns[0].Width = 0;
                // TODO: CMIC
                Width = 100;
            }
            listView.Refresh();
        }

        // Key down event in listView makes focus on MainForm
        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            ListViewKeyDown?.Invoke(e);
        }

        internal void CycleSortKey()
        {
            SortKey = SortKey switch {
                SortKeyOption.Priority => SortKeyOption.ExecTime,
                SortKeyOption.ExecTime => SortKeyOption.UpdateTime,
                SortKeyOption.UpdateTime => SortKeyOption.FullPathName,
                _ => SortKeyOption.Priority,
            };
            SetVirtualList();
        }

        private void listView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            //e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            //TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, e.Bounds, e.ForeColor, TextFormatFlags.Left);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, e.Bounds, Color.Gray, TextFormatFlags.Left);
            using (Pen pen = new Pen(Color.Gray)) {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }
    }
}
