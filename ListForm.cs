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
        internal (string, string) ModeCaptions { get; set; }
        //internal string ModeCaptionOpt { get; set; }

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
            }
        }

        private static int PositiveModulo(int x, int y)
        {
            int z = x % y;
            return (z >= 0) ? z : z + y;
        }

        private void DisplayColumnHeader(int index)
        {
            Header.Text = SortKey switch {
                SortKeyOption.FullPathName => "Path",
                _ => Header.Text = string.Format("{0}: {1}", SortKey.ToString(), ListViewItems[index].SortValue(SortKey))
            };

            // If any, display additional information in column header
            //if (ModeCaptions is not null)
            //    Header.Text += String.Format("  <{0}>", ModeCaptions);
            if (ModeCaptions is not (null, null)) {
                Header.Text += "  " + ModeCaptions.Item1;
                var baseWidth = TextRenderer.MeasureText(Header.Text, listView.Font).Width;
                Header.Text += GetShortenedString(ModeCaptions.Item2, baseWidth) ?? ModeCaptions.Item2;
            }
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

        //
        // Constructor
        //
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
            e.Item = new ListViewItem(ListViewItems[e.ItemIndex].ShortPathName ?? ListViewItems[e.ItemIndex].FullPathName);

            //// CMIC
            //var maxColumnWidth = 500;
            //var truncatedPath = GetTruncatedString(ListViewItems[e.ItemIndex].FullPathName, maxColumnWidth);
            //e.Item = new ListViewItem(truncatedPath);
        }

        internal static string GetShortenedString(string str, int offset = 0)
        {
            // CMIC
            var font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular);
            // CMIC
            var realWidth = 1000 - offset;

            if (TextRenderer.MeasureText(str, font).Width < realWidth) return null;

            while (TextRenderer.MeasureText(str + "...", font).Width > realWidth) {
                str = str[1..];
            }
            return "..." + str;
        }

        private string GetTruncatedString(string str, int width)
        {
            if (TextRenderer.MeasureText(str, listView.Font).Width > width) {
                while (TextRenderer.MeasureText(str + "...", listView.Font).Width > width) {
                    str = str[1..];
                }
                str = "..." + str;
            }
            return str;
        }

        internal FileStats CurrentItem()
        {
            return listView.VirtualListSize == 0 ? null : ListViewItems[VirtualListIndex];
        }

        internal void SelectNextItem()
        {
            if (listView.VirtualListSize == 0) return;

            VirtualListIndex++;
            DisplayColumnHeader(VirtualListIndex);

            var originalScrollPosition = listView.GetItemRect(0).Y;
            listView.EnsureVisible(VirtualListIndex);

            // Resize column width only if displaying initially or scrolling list in order to reduce flickers
            // If GetItemRect(0).Y changes after EnsureVisible(), list view scrolls. Then, resize column
            if (VirtualListIndex == 0 || originalScrollPosition != listView.GetItemRect(0).Y)
                AdjustWidth();
        }

        internal void SelectPreviousItem()
        {
            if (listView.VirtualListSize == 0) return;

            VirtualListIndex--;
            DisplayColumnHeader(VirtualListIndex);

            var originalScrollPosition = listView.GetItemRect(0).Y;
            listView.EnsureVisible(VirtualListIndex);

            // Resize column width only if displaying initially or scrolling list in order to reduce flickers
            // If GetItemRect(0).Y changes after EnsureVisible(), list view scrolls. Then, resize column
            if (VirtualListIndex == 0 || originalScrollPosition != listView.GetItemRect(0).Y)
                AdjustWidth();
        }
        internal void ShowAt(int? x = null, int? y = null, int index = 0)
        {
            Location = new Point(x ?? Location.X, y ?? Location.Y);
            Visible = true;

            if (ListViewItems.Any()) {
                // To add() SelectedIndices, listView requires focus on, which is mentioned by MSDN
                // Changing height in AdjustHeight() seems to focus on list view
                AdjustHeight();
                VirtualListIndex = index;
                DisplayColumnHeader(VirtualListIndex);
                listView.EnsureVisible(VirtualListIndex);
                AdjustWidth();
            }
            else {
                Height = 0;
                listView.Columns[0].Width = 0;
                // TODO: CMIC
                Width = 100;
            }
            listView.Refresh();
        }

        internal void AdjustHeight()
        {
            // TODO: CMIC
            Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize + 1) + 30;
        }
        internal void AdjustWidth()
        {
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            int headerWidth = TextRenderer.MeasureText(Header.Text, listView.Font).Width;
            var maxWidth = Math.Max(listView.GetItemRect(0).Width, headerWidth);

            //Debug.WriteLine(maxWidth);

            listView.Columns[0].Width = maxWidth;
            // TODO: CMIC
            Width = maxWidth + 40;
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
            TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, e.Bounds, Color.Gray, TextFormatFlags.Left);
            using (Pen pen = new Pen(Color.Gray)) {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }
    }
}
