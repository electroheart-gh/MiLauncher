using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiLauncher
{
    public partial class ListForm : Form
    {
        public ListForm()
        {
            InitializeComponent();
        }

        internal bool Reset(FileList fileList, string text)
        {
            // Update listView
            listView.Items.Clear();

            // TODO: Parse text to some patterns to be matched.
            // TODO: Parse text to exec special command such as multi pattern search,  full path search, calc etc.


            foreach (var fn in fileList.items)
            {
                // TODO: Do migemo !!
                if (Regex.IsMatch(fn.FileName, text))
                {
                    listView.Items.Add(fn.FullPathName);
                }
            }
            if (listView.Items.Count == 0)
            {
                return false;
            }

            // Set size and location
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            Height = listView.GetItemRect(0).Height * listView.Items.Count + 30;
            Width = listView.GetItemRect(0).Width + 40;

            // Select the first line and change its color to exec the file
            listView.Items[0].Selected = true;
            // listView.OwnerDraw = true;

            //listView.Items[0].ForeColor = Color.White;
            //listView.Items[0].BackColor= Color.Gray;

            //Location = new Point(Location.X - 10, Location.Y + Height);
            return true;

        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            }
            e.DrawText();
        }
    }
}
