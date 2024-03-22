using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace MiLauncher
{
    /// <summary>
    /// Represents a window of the file list that has responsibilities
    /// to control selected list item and display information for user.
    /// </summary>
    internal partial class ListForm : Form
    {
        //
        // Properties
        //
        internal List<FileStats> ListViewItems { get; private set; }
        internal SortKeyOption SortKey { get; set; } = SortKeyOption.Priority;
        internal string ModeCaption { get; set; }

        private int _virtualListIndex;
        internal int VirtualListIndex
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

                var headerString = DisplayColumnHeader(_virtualListIndex);

                if (_virtualListIndex == 0 || originalScrollPosition != listView.GetItemRect(0).Y) {

                    // *** Size is fine but too many flickers
                    //Width = listView.GetItemRect(0).Width;
                    //listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    //listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    //Width = listView.GetItemRect(0).Width + 40;

                    // *** All header text cannot be shown when header is longer than content
                    //listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    //Width = listView.GetItemRect(0).Width + 40;

                    // *** Works fine!! No flickers!!
                    listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    int headerWidth = TextRenderer.MeasureText(headerString, listView.Font).Width;
                    var maxWidth = Math.Max(listView.GetItemRect(0).Width, headerWidth);

                    listView.Columns[0].Width = maxWidth;
                    // TODO: CMIC
                    Width = maxWidth + 40;
                }
            }
        }

        private static int PositiveModulo(int x, int y)
        {
            int z = x % y;
            return (z >= 0) ? z : z + y;
        }

        private string DisplayColumnHeader(int index)
        {
            Header.Text = SortKey switch {
                SortKeyOption.FullPathName => "Path",
                _ => Header.Text = string.Format("{0}: {1}", SortKey.ToString(), ListViewItems[index].SortValue(SortKey))
            };

            //if (SortKey == SortKeyOption.FullPathName) {
            //    Header.Text = "Path";
            //}
            //else {
            //    Header.Text = string.Format("{0}: {1}", SortKey.ToString(),
            //                              ListViewItems[index].SortValue(SortKey));
            //}

            // Indicate mode information in column header
            if (ModeCaption is not null) {
                Header.Text += String.Format("  <{0}>", ModeCaption);
            }

            return Header.Text;
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
            sourceItems ??= ListViewItems;
            ListViewItems = sourceItems.OrderByDescending(x => x.SortValue(SortKey)).ToList();
            listView.VirtualListSize = ListViewItems.Count;
        }

        internal void SortVirtualList(SortKeyOption sortKey)
        {
            SortKey = sortKey;
            SetVirtualList();
        }

        internal FileStats ExecItem()
        {
            if (Visible & listView.VirtualListSize > 0) {
                try {
                    //FileStats selectedFileInfo = ListViewSource.ElementAt(VirtualListIndex);
                    FileStats selectedFileStats = ListViewItems[VirtualListIndex];
                    Process.Start("explorer.exe", selectedFileStats.FullPathName);
                    Visible = false;
                    return selectedFileStats;
                }
                catch (FileNotFoundException) {
                    Debug.WriteLine("File Not Found");
                }
            }
            return null;
        }

        private void listView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(ListViewItems[e.ItemIndex].FullPathName);
        }

        internal FileStats CurrentItem()
        {
            if (listView.VirtualListSize == 0) return null;
            return ListViewItems[VirtualListIndex];
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

        internal void ShowAt(int? x = null, int? y = null, int index = 0)
        {
            Location = new Point(x ?? Location.X, y ?? Location.Y);
            Visible = true;

            if (ListViewItems.Any()) {
                // To add() SelectedIndices, listView requires focus on, which is mentioned by MSDN
                // And changing height seems to focus on list view
                // TODO: CMIC
                Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize + 1) + 30;
                VirtualListIndex = index;
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
