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
        internal IEnumerable<FileStats> ListViewSource { get; set; }

        // TODO: CMIC
        const int maxLineListView = 30;

        public ListForm()
        {
            InitializeComponent();
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (listView.SelectedIndices.Contains(e.Item.Index))
            {
                // TODO: CMIC
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            }
            e.DrawText();
        }

        //private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        //{
        //    if (e.Item.Selected)
        //    {
        //        // TODO: make it configurable
        //        e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);

        //        // TODO: Display the file in red if file not exist
        //    }
        //    e.DrawText();
        //}

        internal void SetVirtualList(IEnumerable<FileStats> sourceItems)
        {
            ListViewSource = sourceItems;
            listView.VirtualListSize = ListViewSource.Count();

            if (ListViewSource.Any())
            {
                // TODO: CMIC
                Height = listView.GetItemRect(0).Height * Math.Max(maxLineListView, listView.VirtualListSize) + 30;

                // TODO: Check max size in all items
                Width = listView.GetItemRect(0).Width + 40;

                // TODO: Resize Column here?

                // Select the first item, which makes its color change automatically?
                listView.SelectedIndices.Clear();
                listView.SelectedIndices.Add(0);
            }
            else
            {
                // TODO: test what if removing this if-else branch
                Height = 0;
                listView.Columns[0].Width = 0;
                Width = 100;
            }
        }

        //internal void SetList(IEnumerable<string> listItems)
        //{
        //    listView.Items.Clear();

        //    if (listItems.Any())
        //    {
        //        listView.Items.AddRange(listItems.Select(item => new ListViewItem(item)).ToArray());

        //        listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        //        // TODO: Consider make them const or config
        //        Height = listView.GetItemRect(0).Height * listView.Items.Count + 30;
        //        //Height = listView.GetItemRect(0).Height * 30 + 30;
        //        Width = listView.GetItemRect(0).Width + 40;

        //        // Select the first line and change its color
        //        listView.Items[0].Selected = true;
        //    }
        //    else
        //    {
        //        Height = 0;
        //        listView.Columns[0].Width = 0;
        //        Width = 100;
        //    }
        //}

        internal string ExecFile()
        {
            if (Visible & listView.VirtualListSize > 0)
            {
                try
                {
                    var selectedFileInfo = ListViewSource.Skip(listView.SelectedIndices[0]).First();
                    Process.Start("explorer.exe", selectedFileInfo.FullPathName);
                    Visible = false;
                    return selectedFileInfo.FullPathName;
                }
                catch (FileNotFoundException)
                {
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
            if (listView.VirtualListSize > 0)
            {
                var newSelectedIndex = (listView.SelectedIndices[0] + 1) % listView.VirtualListSize;
                listView.SelectedIndices.Clear();
                listView.SelectedIndices.Add(newSelectedIndex);
                listView.EnsureVisible(newSelectedIndex);

                // TODO: Create another method to adjust listForm size including Height, which tends to be too big
                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                Width = listView.GetItemRect(0).Width + 40;

            }
        }

        internal void SelectPreviousItem()
        {
            if (listView.VirtualListSize > 0)
            {
                var newSelectedIndex = (listView.SelectedIndices[0] > 0) ? listView.SelectedIndices[0] - 1 : listView.VirtualListSize - 1;
                listView.SelectedIndices.Clear();
                listView.SelectedIndices.Add(newSelectedIndex);
                listView.EnsureVisible(newSelectedIndex);

                // TODO: Create another method to adjust listForm size including Height, which tends to be too big
                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                Width = listView.GetItemRect(0).Width + 40;
            }
        }

        internal void ShowAt(int x, int y)
        {
            Location = new Point(x, y);
            Visible = true;

            listView.EnsureVisible(listView.SelectedIndices[0]);

            // TODO: CMIC
            Height = listView.GetItemRect(0).Height * Math.Min(maxLineListView, listView.VirtualListSize) + 30;

            // TODO: Resize Column here?
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // TODO: If not using AutoReseize, check max size in all items
            Width = listView.GetItemRect(0).Width + 40;

        }

        // TODO: implement key down event to focus on MainForm

    }
}
