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


        private void cmdBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Disable default behavior for modifier keys, including beep sound
            if (ModifierKeys > 0)
            {
                e.Handled = true;
            }

            // Disable default behavior for Enter and ESC including beep sound
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
            }

            //Console.WriteLine("KeyPress");
            //Console.WriteLine("KeyChar: " + e.KeyChar);
            //Console.WriteLine("modifier: " + ModifierKeys);

            ////int asciiCode = (int)Keys.Control + (int)Keys.F;
            ////Console.WriteLine("Control-F の ASCII コード: " + asciiCode);
            //Console.WriteLine("Kyes.F: " + Keys.F);
            ////Console.WriteLine("(int)Kyes.F: " + (int)Keys.F);
            //Console.WriteLine("(int)KeyChar: " + (int)e.KeyChar);
            //Console.WriteLine("(char)KeyChar: " + (char)e.KeyChar);
            //Console.WriteLine("KeyChar.toString: " + e.KeyChar.ToString());
            //Console.WriteLine(": " + ((int)'f'));
            //Console.WriteLine("ASCIIコード f: " + Convert.ToInt32('f'));


            //if (e.KeyChar == (char)Keys.Escape)
            //{
            //    cmdBox.Text = string.Empty;
            //    e.Handled = true;
            //    Visible = false;
            //    listForm.Visible = false;
            //}

            //if (e.KeyChar == (char)Keys.Enter || (e.KeyChar == (char)Keys.M && ModifierKeys == Keys.Control))
            //if (e.KeyChar == 'M' && ModifierKeys == Keys.Control)
            //{
            //    try
            //    {
            //        Process.Start(listForm.listView.SelectedItems[0].Text);
            //    }
            //    catch (System.IO.FileNotFoundException)
            //    {
            //        Console.WriteLine("File Not Found");
            //    }
            //    cmdBox.Text = string.Empty;
            //    e.Handled = true;
            //    Visible = false;
            //    listForm.Visible = false;
            //}

            //if (e.KeyChar == (char)Keys.A && ModifierKeys == Keys.Control)
            //{
            //    //e.Handled = true;
            //    cmdBox.SelectionStart = 0;
            //}

            //if (e.KeyChar == (char)Keys.E && ModifierKeys == Keys.Control)
            //{
            //    cmdBox.SelectionStart = cmdBox.Text.Length;
            //}

            //if (e.KeyChar == (char)Keys.F && ModifierKeys == Keys.Control)
            //if (e.KeyChar == ((int) 'f') && ModifierKeys == Keys.Control)
            //{
            //    e.Handled = true;
            //    Console.WriteLine("C-f at KeyPress");
            //}

            //if (e.KeyChar == (char)Keys.B && ModifierKeys == Keys.Control)
            //{
            //    cmdBox.SelectionStart = Math.Max(cmdBox.Text.Length, cmdBox.SelectionStart--);
            //}

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

        private void cmdBox_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: make keymap to be configurable

            //Console.WriteLine("KeyDown");
            //Console.WriteLine("KeyCode: " + e.KeyCode);
            //Console.WriteLine("modifier: " + ModifierKeys);
            //Console.WriteLine("Control: " + e.Control + "Alt: " + e.Alt);

            // Close MainForm
            if (e.KeyCode == Keys.Escape)
            {
                cmdBox.Text = string.Empty;
                e.Handled = true;
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
                e.Handled = true;
                Visible = false;
                listForm.Visible = false;
            }

            // beginning of line
            if (e.KeyCode == Keys.A && ModifierKeys == Keys.Control)
            {
                e.Handled = true;
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
                e.Handled = true;
                //Console.WriteLine("\nC-f");
                //Console.WriteLine("text select start: " + cmdBox.SelectionStart);
                cmdBox.SelectionStart++;
                //Console.WriteLine("text select start: " + cmdBox.SelectionStart);
                //Console.WriteLine();
            }

            // backward char
            if (e.KeyCode == Keys.B && ModifierKeys == Keys.Control)
            {
                //cmdBox.SelectionStart = Math.Max(cmdBox.Text.Length, cmdBox.SelectionStart--);
                cmdBox.SelectionStart--;
                e.Handled = true;
            }

            // backspace
            if (e.KeyCode == Keys.H && ModifierKeys == Keys.Control)
            {
                var pos = cmdBox.SelectionStart;
                if (pos > 0)
                {
                    cmdBox.Text = cmdBox.Text.Remove(pos - 1, 1);
                    cmdBox.SelectionStart = pos;
                }

                //Console.WriteLine("Control H");
                //Console.WriteLine("cmdBox.SelectionStart: " + cmdBox.SelectionStart);
                //Console.WriteLine("cmdBox.Text: " + cmdBox.Text);
                //cmdBox.Text = cmdBox.Text.Remove(cmdBox.SelectionStart - 1, 1);
                //Console.WriteLine("cmdBox.SelectionStart: " + cmdBox.SelectionStart);
                //Console.WriteLine("cmdBox.Text: " + cmdBox.Text);

                e.Handled = true;
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

                e.Handled = true;
            }

            // TODO: forward word
            if (e.KeyCode == Keys.F && ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }

            // TODO: backward word
            if (e.KeyCode == Keys.B && ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }

            // TODO: delete word
            if (e.KeyCode == Keys.D && ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }


            // TODO: backward delete word
            if (e.KeyCode == Keys.H && ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }

        }
    }
}
