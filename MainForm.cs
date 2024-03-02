using KaoriYa.Migemo;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiLauncher
{
    public partial class MainForm : Form
    {
        // Variables
        private Point dragStart;
        private HotKey hotKey;
        private ListForm listForm;
        private FileSet searchedFileSet;
        private CancellationTokenSource tokenSource;
        private SortKeyOption sortKeyOption = SortKeyOption.Priority;

        // Constant
        // TODO: Consider to make FileList.dat configurable
        private const string searchedFileListDataFile = "SearchedFileList.dat";
        // private const string recentFileListDataFile = "RecentFileList.dat";
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
        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Global Hot Key
            hotKey = new HotKey(MOD_KEY.ALT | MOD_KEY.CONTROL, Keys.F);
            hotKey.HotKeyPush += new EventHandler(hotKey_HotKeyPush);

            // List Form
            listForm = new ListForm();

            // File Set
            searchedFileSet = SettingManager.LoadSettings<FileSet>(searchedFileListDataFile) ?? new FileSet();
            //Test Code: fileList = FileList.FileListForTest();
            //recentFileList = SettingManager.LoadSettings<recentFileList>(recentFileListDataFile) ?? new recentFileList();

            //var searchPaths = Program.appSettings.TargetFolders;
            //searchedFileSet = await Task.Run(() => SearchedFileSet.SearchFiles(searchPaths));
            var newSearchedFileSet =
                await Task.Run(() => FileSet.SearchFiles(Program.appSettings.TargetFolders));
            searchedFileSet = newSearchedFileSet.CopyPriority(searchedFileSet);

            //Debug.WriteLine("start");
            ////var prioritizedFileList = searchedFileSet.OrderByPriority();
            ////var prioritizedFileList = searchedFileSet.Items.OrderByDescending(x => x.Priority).ToList();
            //Debug.WriteLine("end");

            // Test Code: var searchPaths = new List<string>{ @"C:\Users\JUNJI\Desktop\", @"E:\Documents\RocksmithTabs\" };

            SettingManager.SaveSettings(searchedFileSet, searchedFileListDataFile);
        }

        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            Visible = true;
            Activate();
            BringToFront();
        }

        private async void cmdBox_TextChanged(object sender, EventArgs e)
        {
            listForm.Visible = false;

            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource = null;
            }

            if (cmdBox.Text.Length == 0) return;

            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            // Added ToArray() to apply eager evaluation because lazy evaluation makes it too slow 
            var patternsInCmdBox = cmdBox.Text.Split(wordSeparator, StringSplitOptions.RemoveEmptyEntries);
            var patternsTransformed = patternsInCmdBox.Select(transformByMigemo).ToArray();
            var selectedList = await Task.Run(() => searchedFileSet.SelectWithCancellation(patternsTransformed, token), token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            listForm.SetVirtualList(selectedList, sortKeyOption);

            // TODO: CMIC
            listForm.ShowAt(Location.X - 6, Location.Y + Height - 5);

            Activate();
            return;

            static string transformByMigemo(string pattern)
            {
                using (Migemo migemo = new("./Dict/migemo-dict"))
                {
                    // TODO: make it config or const
                    // TODO: Consider to make MatchCondition class, which should have a method to parse string to select condition
                    var prefix = "-!\\".Contains(pattern[..1]) ? pattern[..1] : "";
                    if (pattern.Length - prefix.Length < Program.appSettings.MinMigemoLength)
                    {
                        return pattern;
                    }
                    return prefix + migemo.GetRegex(pattern[prefix.Length..]);
                };
            }
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

                // Debug.WriteLine("visible false by ESC");
                listForm.Visible = false;
            }
            // Exec file with associated app
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.M && e.Control))
            {
                var fullPathName = listForm.ExecFile();
                // TODO: Consider to make the value to adjust the priority '1' configurable
                // searchedFileSet.AddPriority(fullPathName, 1);
                searchedFileSet.Items.First(x => x.FullPathName == fullPathName).Priority += 1;

                SettingManager.SaveSettings(searchedFileSet, searchedFileListDataFile);

                //recentFileList.UpdateHistory(fullPathName);
                cmdBox.Text = string.Empty;
                Visible = false;

            }
            // beginning of line
            if (e.KeyCode == Keys.A && e.Control)
            {
                cmdBox.SelectionStart = 0;
            }
            // end of line
            if (e.KeyCode == Keys.E && e.Control)
            {
                cmdBox.SelectionStart = cmdBox.Text.Length;
            }
            // forward char
            if (e.KeyCode == Keys.F && e.Control)
            {
                cmdBox.SelectionStart++;
            }
            // backward char
            if (e.KeyCode == Keys.B && e.Control)
            {
                cmdBox.SelectionStart = Math.Max(0, cmdBox.SelectionStart - 1);
            }
            // backspace
            if (e.KeyCode == Keys.H && e.Control)
            {
                var pos = cmdBox.SelectionStart;
                if (pos > 0)
                {
                    cmdBox.Text = cmdBox.Text.Remove(pos - 1, 1);
                    cmdBox.SelectionStart = pos - 1;
                }
            }
            // delete char
            if (e.KeyCode == Keys.D && e.Control)
            {
                var pos = cmdBox.SelectionStart;
                if (pos < cmdBox.Text.Length)
                {
                    cmdBox.Text = cmdBox.Text.Remove(pos, 1);
                    cmdBox.SelectionStart = pos;
                }
            }
            // select next item
            if (e.KeyCode == Keys.N && e.Control)
            {
                listForm.SelectNextItem();
            }
            // select previous item
            if (e.KeyCode == Keys.P && e.Control)
            {
                listForm.SelectPreviousItem();
            }
            // forward word
            if (e.KeyCode == Keys.F && e.Alt)
            {
                var pattern = NextWordRegex();
                var m = pattern.Match(cmdBox.Text, cmdBox.SelectionStart);
                cmdBox.SelectionStart = Math.Max(m.Index + m.Length, cmdBox.SelectionStart);
            }
            // backward word
            if (e.KeyCode == Keys.B && e.Alt)
            {
                var pattern = PreviousWordRegex();
                var m = pattern.Match(cmdBox.Text[..cmdBox.SelectionStart]);
                cmdBox.SelectionStart = m.Index;
            }
            // delete word
            if (e.KeyCode == Keys.D && e.Alt)
            {
                var cursorPosition = cmdBox.SelectionStart;
                var pattern = NextWordRegex();
                cmdBox.Text = pattern.Replace(cmdBox.Text, "", 1, cursorPosition);
                cmdBox.SelectionStart = cursorPosition;
            }
            // backward delete word
            if (e.KeyCode == Keys.H && e.Alt)
            {
                // Using Non-backtracking and negative lookahead assertion of Regex
                var pattern = PreviousWordRegex();
                var firstHalf = pattern.Replace(cmdBox.Text[..cmdBox.SelectionStart], "");
                cmdBox.Text = firstHalf + cmdBox.Text[cmdBox.SelectionStart..];
                cmdBox.SelectionStart = firstHalf.Length;
            }
            // Change ListView order
            // Keys.Oemtilde indicates @ (at mark)
            if (e.KeyCode == Keys.Oemtilde && e.Control)
            {
                sortKeyOption = sortKeyOption switch
                {
                    SortKeyOption.Priority => SortKeyOption.FullPathName,
                    SortKeyOption.FullPathName => SortKeyOption.UpdateTime,
                    SortKeyOption.UpdateTime => SortKeyOption.Priority,
                    _ => SortKeyOption.Priority,
                };
                listForm.SortVirtualList(sortKeyOption);
            }

            // TODO: implement crawl folder mode like zii launcher, which requires another ListView class
            // TODO: implement sorting list by timestamp, priority and alphabetic, which should update ListForm and  classes as well
            // TODO: implement search history using M-p, M-n
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            // TODO: consider when to save fileList
            // SettingManager.SaveSettings<FileList>(fileList, fileListDataPath);
        }

        private void cmdBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                e.IsInputKey = true;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Move MainForm by left button dragging
            if (e.Button == MouseButtons.Left)
            {
                dragStart = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Move MainForm by left button dragging
            if (e.Button == MouseButtons.Left)
            {
                Location = new Point(Location.X + e.Location.X - dragStart.X, Location.Y + e.Location.Y - dragStart.Y);
            }
        }

        [GeneratedRegex(@"\w*\W*")]
        private static partial Regex NextWordRegex();

        // Using Non-backtracking and negative lookahead assertion of Regex
        [GeneratedRegex(@"(?>\w*\W*)(?!\w)")]
        private static partial Regex PreviousWordRegex();


    }

}
