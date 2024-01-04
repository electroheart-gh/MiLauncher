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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MiLauncher
{
    public partial class MainForm : Form
    {
        // Variable
        private Point dragStart;
        private HotKey hotKey;
        private ListForm listForm;
        private FileList fileList;

        // Const
        private string settingsFilePath = "mySettings.json"; // 設定ファイルのパス
        private string fileListDataPath = "FileList.dat";


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
            fileList = SettingManager.LoadSettings<FileList>(fileListDataPath);

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
            foreach (var fn in fileList.Items)
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

            // select previous file
            if (e.KeyCode == Keys.P && ModifierKeys == Keys.Control)
            {
                // Assuming number of selected items should be one
                if (listForm.listView.SelectedIndices.Count > 0)
                {
                    var selectedIndex = listForm.listView.SelectedIndices[0];
                    listForm.listView.Items[selectedIndex].Selected = false;
                    if (selectedIndex == 0)
                    {
                        listForm.listView.Items[listForm.listView.Items.Count - 1].Selected = true;
                    }
                    else
                    {
                        listForm.listView.Items[selectedIndex - 1].Selected = true;
                    }
                }

            }

            // TODO: forward word
            if (e.KeyCode == Keys.F && ModifierKeys == Keys.Alt)
            {
                //for (int i = cmdBox.SelectionStart+ 1; i < cmdBox.Text.Length; i++)
                //{
                //    if (char.IsWhiteSpace(cmdBox.Text[i]) || char.IsPunctuation(cmdBox.Text[i]))
                //    {
                //        // 空白または句読点の後に単語が始まる場合、その位置までカーソルを移動
                //        // sample text, abc$ ,. def10, 
                //        while (i < cmdBox.Text.Length && (char.IsWhiteSpace(cmdBox.Text[i]) || char.IsPunctuation(cmdBox.Text[i])))
                //        {
                //            i++;
                //        }

                //        cmdBox.SelectionStart = i;
                //        cmdBox.SelectionLength = 0;
                //        return;
                //    }
                //}

                var pattern = new Regex(@"\w+\W*");
                var m = pattern.Match(cmdBox.Text, cmdBox.SelectionStart);

                //Console.WriteLine("m: " + m.ToString());
                //Console.WriteLine("index: " + m.Index);
                //Console.WriteLine("length: " + m.Length);
                //Console.WriteLine("selectStart: " + cmdBox.SelectionStart);
                //Console.WriteLine("text: " + cmdBox.Text);

                cmdBox.SelectionStart = Math.Max(m.Index + m.Length, cmdBox.SelectionStart);
            }

            // backward word
            if (e.KeyCode == Keys.B && ModifierKeys == Keys.Alt)
            {
                // Regex is difficult for backward search
                var pattern = new Regex(@"(?>\w+\W*)(?!\w)");
                //var pattern = new Regex(@"\w+\W*(?!\w)");
                //var pattern = new Regex(@"\w+\W*");
                //var searchText=cmdBox.Text.Substring(0, cmdBox.SelectionStart);

                var m = pattern.Match(cmdBox.Text.Substring(0, cmdBox.SelectionStart));

                //Console.WriteLine("M-b");
                //Console.WriteLine("index: " + m.Index);
                //Console.WriteLine("length: " + m.Length);
                //Console.WriteLine("selectStart: " + cmdBox.SelectionStart);
                //Console.WriteLine("search text: " + cmdBox.Text.Substring(0, cmdBox.SelectionStart));
                //Console.WriteLine("text: " + cmdBox.Text);

                cmdBox.SelectionStart = m.Index;

            }

            // TODO: delete word
            if (e.KeyCode == Keys.D && ModifierKeys == Keys.Alt)
            {
            }

            // TODO: backward delete word
            if (e.KeyCode == Keys.H && ModifierKeys == Keys.Alt)
            {
            }

            // TODO: implement crawl folder mode like zii launcher, which requires another ListView class
            // TODO: implement full path search mode, which should update ListForm class as well
            // TODO: implement sorting list by timestamp, priority and alphabetic, which should update ListForm and FileList classes as well
            // TODO: implement search history using C-i, C-o
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            // TODO: save FileList
            SettingManager.SaveSettings<FileList>(fileList, fileListDataPath);

        }
    }
}
