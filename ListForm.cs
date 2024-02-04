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
        public ListForm()
        {
            InitializeComponent();
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                // TODO: make it configurable
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);

                // TODO: Display the file in red if file not exist
            }
            e.DrawText();
        }

        internal void SetList(IEnumerable<string> listItems)
        {
            listView.Items.Clear();

            if (listItems.Any())
            {
                //foreach (string item in list)
                //{
                //    listView.Items.Add(item);
                //}
                listView.Items.AddRange(listItems.Select(item => new ListViewItem(item)).ToArray());

                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                Height = listView.GetItemRect(0).Height * listView.Items.Count + 30;
                Width = listView.GetItemRect(0).Width + 40;

                // Select the first line and change its color
                listView.Items[0].Selected = true;
            }
            else
            {
                Height = 0;
                listView.Columns[0].Width = 0;
                Width = 100;
            }
        }

        internal string ExecFile()
        {
            if (Visible & listView.Items.Count > 0)
            {
                try
                {
                    Process.Start("explorer.exe", listView.SelectedItems[0].Text);
                    Visible = false;
                    return listView.SelectedItems[0].Text;
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine("File Not Found");
                }
            }
            return string.Empty;
        }
    }
}
