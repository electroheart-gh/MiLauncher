using KaoriYa.Migemo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MiLauncher
{
    public partial class ListForm : Form
    {
        // Variables
        Migemo migemo = new Migemo("./Dict/migemo-dict");
        Regex regex;

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

        //internal void SetList(List<string> list)
        internal void SetList(List<string> list)
        {

            listView.Items.Clear();

            if (list.Count == 0)
            {
                Height = 0;
                listView.Columns[0].Width= 0;
                Width = 100;
            }
            else
            {
                foreach (string item in list)
                {
                    listView.Items.Add(item);
                }
                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

                Height = listView.GetItemRect(0).Height * listView.Items.Count + 30;
                Width = listView.GetItemRect(0).Width + 40;

                // Select the first line and change its color
                listView.Items[0].Selected = true;
            }
        }
    }
}
