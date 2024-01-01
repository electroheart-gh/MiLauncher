using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

            // List Form
            listForm = new ListForm();
            // File List
            fileList = new FileList();
        }
        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            Visible = true;
            //listForm.Reset(fileList, cmdBox.Text);
            //listForm.StartPosition = FormStartPosition.Manual;
            //listForm.Location = new Point(Location.X - 10, Location.Y + Height);
            //listForm.Invalidate();
            //listForm.Visible = true;

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


        // Implement Ctrl- and Alt- commands in KeyDown event
        // It is because e.KeyChar of KeyPress returns a value depending on modifiers input,
        // which requires to check KeyChar of Ctrl-(char) in advance of coding
        private void cmdBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Disable default behavior for modifier keys, including beep sound
            if ((ModifierKeys & (Keys.Control | Keys.Alt)) > 0)
            {
                e.Handled = true;
            }

            // Disable default behavior for Enter and ESC including beep sound
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }
        }

        private void cmdBox_TextChanged(object sender, EventArgs e)
        {
            if (listForm.Reset(fileList, cmdBox.Text))
            {
                //if list has any items, 
                listForm.StartPosition = FormStartPosition.Manual;
                listForm.Location = new Point(Location.X - 10, Location.Y + Height);
                listForm.Visible = true;
                Activate();
            }
            else
            {
                listForm.Visible = false;
            }

        }

        // Implement Ctrl- and Alt- commands in KeyDown event
        // It is because e.KeyChar of KeyPress returns a value depending on modifiers input,
        // which requires to check KeyChar of Ctrl-(char) in advance of coding
        private void cmdBox_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: make keymap to be configurable

            // Close MainForm
            if (e.KeyCode == Keys.Escape)
            {
                cmdBox.Text = string.Empty;
                Visible = false;
                listForm.Visible = false;
            }

            // Exec file with associated app
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.M && ModifierKeys == Keys.Control))
            {
                try
                {
                    Process.Start(listForm.listView.SelectedItems[0].Text);
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.WriteLine("File Not Found");
                }

                cmdBox.Text = string.Empty;
                Visible = false;
                listForm.Visible = false;
            }

            // beginning of line
            if (e.KeyCode == Keys.A && ModifierKeys == Keys.Control)
            {
                cmdBox.SelectionStart = 0;
            }

            // end of line
            if (e.KeyCode == Keys.E && ModifierKeys == Keys.Control)
            {
                cmdBox.SelectionStart = cmdBox.Text.Length;
            }

            // forward char
            if (e.KeyCode == Keys.F && ModifierKeys == Keys.Control)
            {
                cmdBox.SelectionStart++;
            }

            // backward char
            if (e.KeyCode == Keys.B && ModifierKeys == Keys.Control)
            {
                if (cmdBox.SelectionStart > 0)
                {
                    cmdBox.SelectionStart--;
                }
            }

            // backspace
            if (e.KeyCode == Keys.H && ModifierKeys == Keys.Control)
            {
                var pos = cmdBox.SelectionStart;
                if (pos > 0)
                {
                    cmdBox.Text = cmdBox.Text.Remove(pos - 1, 1);
                    cmdBox.SelectionStart = pos - 1;
                }
            }

            // delete char
            if (e.KeyCode == Keys.D && ModifierKeys == Keys.Control)
            {
                var pos = cmdBox.SelectionStart;
                if (pos < cmdBox.Text.Length)
                {
                    cmdBox.Text = cmdBox.Text.Remove(pos, 1);
                    cmdBox.SelectionStart = pos;
                }
            }

            // select next file
            if (e.KeyCode == Keys.N && ModifierKeys == Keys.Control)
            {
                // TODO: Try MultiSelect false
                // Assuming number of selected items should be one
                if (listForm.listView.SelectedIndices.Count > 0)
                {
                    var selectedIndex = listForm.listView.SelectedIndices[0];
                    listForm.listView.Items[selectedIndex].Selected = false;
                    if (selectedIndex == listForm.listView.Items.Count - 1)
                    {
                        listForm.listView.Items[0].Selected = true;
                    }
                    else
                    {
                        listForm.listView.Items[selectedIndex + 1].Selected = true;
                    }
                }
            }

            // TODO: select previous file
            if (e.KeyCode == Keys.P && ModifierKeys == Keys.Control)
            {

            }

            // TODO: forward word
            if (e.KeyCode == Keys.F && ModifierKeys == Keys.Alt)
            {
            }

            // TODO: backward word
            if (e.KeyCode == Keys.B && ModifierKeys == Keys.Alt)
            {
            }

            // TODO: delete word
            if (e.KeyCode == Keys.D && ModifierKeys == Keys.Alt)
            {
            }

            // TODO: backward delete word
            if (e.KeyCode == Keys.H && ModifierKeys == Keys.Alt)
            {
            }
        }
    }
}
