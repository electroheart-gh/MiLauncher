using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiLauncher
{
    public partial class MainForm : Form
    {
        // TODO: class FileList


        // Variable
        private Point dragStart;
        private HotKey hotKey;
        private ListForm listForm;
        private FileList fileList;

        //
        // Constructor
        //
        public MainForm()
        {
            InitializeComponent();
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragStart = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Location = new Point(Location.X + e.Location.X - dragStart.X, Location.Y + e.Location.Y - dragStart.Y);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Global Hot Key
            hotKey = new HotKey(MOD_KEY.ALT | MOD_KEY.CONTROL, Keys.F);
            hotKey.HotKeyPush += new EventHandler(hotKey_HotKeyPush);

            // File List Form
            listForm = new ListForm();
            fileList=new FileList();

        }
        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            Visible = true;

            RenewListView();
            listForm.Visible = true;

            Activate();
            BringToFront();
        }

        public void RenewListView()
        {
            // Update listView
            foreach (var fn in fileList.items)
            {
                listForm.listView.Items.Add(fn.FullPathName);
            }

            // Set size and location
            listForm.listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listForm.Height = listForm.listView.GetItemRect(0).Height * listForm.listView.Items.Count + 30;
            listForm.Width = listForm.listView.GetItemRect(0).Width + 40;
            listForm.Location = new Point(Location.X - 10, Location.Y + Height);
        }


        private void cmdBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                cmdBox.Text = string.Empty;
                e.Handled = true;
                Visible = false;
                listForm.Visible = false;
            }
        }
    }
}
