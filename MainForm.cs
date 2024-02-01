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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MiLauncher
{
    public partial class MainForm : Form
    {
        // Variables
        private Point dragStart;
        private HotKey hotKey;
        private ListForm listForm;
        private FileList fileList;
        private CancellationTokenSource tokenSource;
        //private AppSettings appSettings;

        // Constant
        private string settingsFilePath = "mySettings.json"; // 設定ファイルのパス
        private const string fileListDataPath = "FileList.dat";
        private const char wordSeparator = ' ';
        private const int CS_DROPSHADOW = 0x00020000;


        //
        // Constructor
        //
        public MainForm()
        {
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        //
        // Event handler
        //
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

        private async void MainForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("MainForm Load");

            // Global Hot Key
            hotKey = new HotKey(MOD_KEY.ALT | MOD_KEY.CONTROL, Keys.F);
            hotKey.HotKeyPush += new EventHandler(hotKey_HotKeyPush);

            // List Form
            listForm = new ListForm();

            // File List
            fileList = SettingManager.LoadSettings<FileList>(fileListDataPath);
            if (fileList == null)
            {
                fileList = new FileList();
            }
            // Test Code
            // fileList = FileList.FileListForTest();

            // TODO: consider when to run the file search !!!
            // TODO: MUST make it configurable !!!
            //string[] searchPaths = { @"C:\Users\JUNJI\Desktop\", @"E:\Documents\RocksmithTabs\" };
            //string[] searchPaths = { @"C:\Users\JUNJI\Desktop\"};
            //string[] searchPaths = { @"C:\Users\JUNJI\Desktop\", @"E:\Documents\" };
            var searchPaths = Program.appSettings.TargetFolders;

            fileList = await Task.Run(() => fileList.Search(searchPaths));
            //Debug.WriteLine("fileList.count after search: " + fileList.Items.Count);
            SettingManager.SaveSettings(fileList, fileListDataPath);
        }
        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            Visible = true;
            Activate();
            BringToFront();
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

        private async void cmdBox_TextChanged(object sender, EventArgs e)
        {
            // Console.WriteLine("text change started: " + cmdBox.Text);
            listForm.Visible = false;

            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }

            if (cmdBox.Text.Count() == 0)
            {
                return;
            }

            // TODO: Parse text to determine what todo, exec special command such as calculation etc.
            var words = cmdBox.Text.Split(wordSeparator, StringSplitOptions.RemoveEmptyEntries);

            //
            // The followings are codes for normal search
            //
            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var selectedList = await Task.Run(() => fileList.Select(words, token), token);
            // For sync test
            // var selectedList = fileList.Select(words, token);

            // TODO: Consider how to show file search is running/finished
            if (!token.IsCancellationRequested)
            {
                listForm.SetList(selectedList);
                listForm.Location = new Point(Location.X - 10, Location.Y + Height);

                // Console.WriteLine("visible true: " + cmdBox.Text);
                listForm.Visible = true;
                Activate();
            }
            // tokenSource.Dispose();
            // tokenSource = null;
            // Console.WriteLine("text change finished: " + cmdBox.Text);
        }

        // Implement Ctrl- and Alt- commands in KeyDown event
        // It is because e.KeyChar of KeyPress returns a value depending on modifiers input,
        // which requires to check KeyChar of Ctrl-(char) in advance of coding
        private void cmdBox_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: implement keymap class to make keymap configurable

            // Close MainForm
            if (e.KeyCode == Keys.Escape)
            {
                cmdBox.Text = string.Empty;
                Visible = false;

                Console.WriteLine("visible false by ESC");
                listForm.Visible = false;
            }
            // Exec file with associated app
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.M && ModifierKeys == Keys.Control))
            {
                // TODO: Make it method of listForm
                if (listForm.Visible & listForm.listView.Items.Count > 0)
                {
                    try
                    {
                        Process.Start("explorer.exe", listForm.listView.SelectedItems[0].Text);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        // TODO: Display the file in red if file not exist
                        Console.WriteLine("File Not Found");
                    }
                }
                cmdBox.Text = string.Empty;
                Visible = false;
                listForm.Visible = false;
            }
            // TODO: Fix beginning of line
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
                // TODO: Make it method of listForm
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
                // TODO: Make it method of listForm
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
            // forward word
            if (e.KeyCode == Keys.F && ModifierKeys == Keys.Alt)
            {
                var pattern = new Regex(@"\w*\W*");
                var m = pattern.Match(cmdBox.Text, cmdBox.SelectionStart);
                cmdBox.SelectionStart = Math.Max(m.Index + m.Length, cmdBox.SelectionStart);
            }
            // backward word
            if (e.KeyCode == Keys.B && ModifierKeys == Keys.Alt)
            {
                // Using Non-backtracking and negative lookahead assertion of Regex
                var pattern = new Regex(@"(?>\w*\W*)(?!\w)");
                var m = pattern.Match(cmdBox.Text.Substring(0, cmdBox.SelectionStart));
                cmdBox.SelectionStart = m.Index;
            }
            // delete word
            if (e.KeyCode == Keys.D && ModifierKeys == Keys.Alt)
            {
                var cursorPosition = cmdBox.SelectionStart;
                //var pattern = new Regex(@"\w+\W*");
                var pattern = new Regex(@"\w*\W*");
                cmdBox.Text = pattern.Replace(cmdBox.Text, "", 1, cursorPosition);
                cmdBox.SelectionStart = cursorPosition;
            }
            // backward delete word
            if (e.KeyCode == Keys.H && ModifierKeys == Keys.Alt)
            {
                // Using Non-backtracking and negative lookahead assertion of Regex
                //var pattern = new Regex(@"(?>\w+\W*)(?!\w)");
                var pattern = new Regex(@"(?>\w*\W*)(?!\w)");
                var firstHalf = pattern.Replace(cmdBox.Text.Substring(0, cmdBox.SelectionStart), "");
                cmdBox.Text = firstHalf + cmdBox.Text.Substring(cmdBox.SelectionStart);
                cmdBox.SelectionStart = firstHalf.Length;
            }

            // TODO: implement crawl folder mode like zii launcher, which requires another ListView class
            // TODO: implement sorting list by timestamp, priority and alphabetic, which should update ListForm and FileList classes as well
            // TODO: implement search history using M-p, M-n
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            // TODO: consider when to save fileList
            // SettingManager.SaveSettings<FileList>(fileList, fileListDataPath);
        }
    }
}
