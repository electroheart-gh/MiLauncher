using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        internal void Reset(FileList fileList)
        {
            // Update listView
            foreach (var fn in fileList.items)
            {
                listView.Items.Add(fn.FullPathName);
            }

            // Set size and location
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            Height = listView.GetItemRect(0).Height * listView.Items.Count + 30;
            Width = listView.GetItemRect(0).Width + 40;
            //Location = new Point(Location.X - 10, Location.Y + Height);
        }
    }
}
