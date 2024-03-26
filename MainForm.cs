using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MiLauncher
{
    /// <summary>
    /// Represents a main window that accept user interface and delegates tasks to other windows/classes.
    /// </summary>
    public partial class MainForm : Form
    {
        // Variables
        private Point dragStart;
        private HotKey hotKey;
        private ListForm listForm;
        // private HashSet<FileStats> baseFileSet;
        private HashSet<FileStats> searchedFileSet;
        private CancellationTokenSource tokenSource;
        private ModeController currentMode = new();

        // Constant
        // TODO: Consider to make FileList.dat configurable
        private const string searchedFileListDataFile = "SearchedFileList.dat";
        private const char wordSeparator = ' ';
        private const int CS_DROPSHADOW = 0x00020000;

        //
        // Constructor
        //
        public MainForm()
        {
            InitializeComponent();
        }

        // Borderless winform with shadow
        protected override CreateParams CreateParams
        {
            get {
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
            listForm.ListViewKeyDown = listView_KeyDown;

            // Load File Set (HashSet<FileStats>)
            searchedFileSet = SettingManager.LoadSettings<HashSet<FileStats>>(searchedFileListDataFile) ?? [];

            // Search Files Async
            var newSearchedFileSet = await Task.Run(FileSet.SearchAllFiles);
            searchedFileSet = newSearchedFileSet.ImportPriorityAndExecTime(searchedFileSet).ToHashSet();
            SettingManager.SaveSettings(searchedFileSet, searchedFileListDataFile);
        }

        private void listView_KeyDown(KeyEventArgs args)
        {
            ActivateMainForm();
        }

        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            ActivateMainForm();
        }

        private void ActivateMainForm()
        {
            Visible = true;
            Activate();
            BringToFront();
        }

        private async void cmdBox_TextChanged(object sender, EventArgs e)
        {
            tokenSource?.Cancel();
            tokenSource = null;

            if (currentMode.IsRestoreMode()) {
                //currentMode.ExitRestore();
                return;
            }
            if (cmdBox.Text.Length == 0 && currentMode.IsPlain()) {
                listForm.Visible = false;
                return;
            }

            // Added ToArray() to apply eager evaluation because lazy evaluation makes it too slow 
            var patternsInCmdBox = cmdBox.Text.Split(wordSeparator, StringSplitOptions.RemoveEmptyEntries);
            var patternsTransformed = patternsInCmdBox.Select(transformByMigemo).ToArray();

            // Set baseFileSet depending on crawlMode or not
            //HashSet<FileStats> baseFileSet = crawlMode?.CrawlFileSet ?? searchedFileSet;
            HashSet<FileStats> baseFileSet = currentMode.GetCrawlFileSet() ?? searchedFileSet;

            tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var filteredList = await Task.Run
                (() => baseFileSet.FilterWithCancellation(patternsTransformed, token), token);
            if (token.IsCancellationRequested) return;

            listForm.SetVirtualList(filteredList);

            // TODO: CMIC
            listForm.ShowAt(Location.X - 6, Location.Y + Height - 5);

            Activate();
            return;

            static string transformByMigemo(string pattern)
            {
                using (Migemo migemo = new("./Dict/migemo-dict")) {
                    // TODO: make it config or const
                    // TODO: Consider to make MatchCondition class,
                    // which should have a method to parse string to select condition
                    var prefix = "-!\\".Contains(pattern[..1]) ? pattern[..1] : "";
                    if (pattern.Length - prefix.Length < Program.appSettings.MinMigemoLength) {
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
            if ((ModifierKeys & (Keys.Control | Keys.Alt)) > 0) {
                e.Handled = true;
            }
            // Disable default behavior for Enter and ESC including beep sound
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape) {
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
            if (e.KeyCode == Keys.Escape) {
                cmdBox.Text = string.Empty;
                Visible = false;
                listForm.Visible = false;
                SettingManager.SaveSettings(searchedFileSet, searchedFileListDataFile);
            }
            // Exec file with associated app
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.M && e.Control)) {
                var execFileStats = listForm.ExecItem();

                // TODO: Consider to make the value to adjust the priority '1' configurable
                var fileStats = searchedFileSet.FirstOrDefault(x => x.FullPathName == execFileStats.FullPathName);
                if (fileStats is null) {
                    // Add to searchedFileSet temporarily, even though it might be removed after searchAllFiles()
                    execFileStats.Priority += 1;
                    execFileStats.ExecTime = DateTime.Now;
                    searchedFileSet.Add(execFileStats);
                }
                else {
                    fileStats.Priority += 1;
                    fileStats.ExecTime = DateTime.Now;
                }
                //SettingManager.SaveSettings(searchedFileSet, searchedFileListDataFile);

                cmdBox.Text = string.Empty;
                Visible = false;
            }
            // beginning of line
            if (e.KeyCode == Keys.A && e.Control) {
                cmdBox.SelectionStart = 0;
            }
            // end of line
            if (e.KeyCode == Keys.E && e.Control) {
                cmdBox.SelectionStart = cmdBox.Text.Length;
            }
            // forward char
            if (e.KeyCode == Keys.F && e.Control) {
                cmdBox.SelectionStart++;
            }
            // backward char
            if (e.KeyCode == Keys.B && e.Control) {
                cmdBox.SelectionStart = Math.Max(0, cmdBox.SelectionStart - 1);
            }
            // backspace
            if (e.KeyCode == Keys.H && e.Control) {
                var pos = cmdBox.SelectionStart;
                if (pos > 0) {
                    cmdBox.Text = cmdBox.Text.Remove(pos - 1, 1);
                    cmdBox.SelectionStart = pos - 1;
                }
            }
            // delete char
            if (e.KeyCode == Keys.D && e.Control) {
                var pos = cmdBox.SelectionStart;
                if (pos < cmdBox.Text.Length) {
                    cmdBox.Text = cmdBox.Text.Remove(pos, 1);
                    cmdBox.SelectionStart = pos;
                }
            }
            // select next item
            if (e.KeyCode == Keys.N && e.Control) {
                listForm.SelectNextItem();
            }
            // select previous item
            if (e.KeyCode == Keys.P && e.Control) {
                listForm.SelectPreviousItem();
            }
            // forward word
            if (e.KeyCode == Keys.F && e.Alt) {
                Regex pattern = NextWordRegex();
                Match m = pattern.Match(cmdBox.Text, cmdBox.SelectionStart);
                cmdBox.SelectionStart = Math.Max(m.Index + m.Length, cmdBox.SelectionStart);
            }
            // backward word
            if (e.KeyCode == Keys.B && e.Alt) {
                Regex pattern = PreviousWordRegex();
                Match m = pattern.Match(cmdBox.Text[..cmdBox.SelectionStart]);
                cmdBox.SelectionStart = m.Index;
            }
            // delete word
            if (e.KeyCode == Keys.D && e.Alt) {
                var cursorPosition = cmdBox.SelectionStart;
                Regex pattern = NextWordRegex();
                cmdBox.Text = pattern.Replace(cmdBox.Text, "", 1, cursorPosition);
                cmdBox.SelectionStart = cursorPosition;
            }
            // backward delete word
            if (e.KeyCode == Keys.H && e.Alt) {
                // Using Non-backtracking and negative lookahead assertion of Regex
                Regex pattern = PreviousWordRegex();
                var firstHalf = pattern.Replace(cmdBox.Text[..cmdBox.SelectionStart], "");
                cmdBox.Text = firstHalf + cmdBox.Text[cmdBox.SelectionStart..];
                cmdBox.SelectionStart = firstHalf.Length;
            }
            // Cycle ListView sort key
            // Keys.Oemtilde indicates @ (at mark)
            if (e.KeyCode == Keys.Oemtilde && e.Control) {
                if (!listForm.Visible) return;

                listForm.CycleSortKey();
                listForm.ShowAt();
            }
            // Crawl folder upwards
            if (e.KeyCode == Keys.Oemcomma && e.Control) {
                if (!listForm.Visible) return;

                // Crawl and check if it works
                //CrawlMode newCrawlMode =
                //    (currentMode.IsCrawlMode()) ?
                //    crawlMode.CrawlUp() :
                //    CrawlMode.Crawl(Path.GetDirectoryName(listForm.CurrentItem().FullPathName),
                //                    cmdBox.Text, listForm.VirtualListIndex, listForm.SortKey, listForm.ListViewItems);
                //if (newCrawlMode is null) return;
                //crawlMode = newCrawlMode;

                // Crawl up and check if it works
                //if (!currentMode.CrawlUp()) {
                //    currentMode.Crawl(Path.GetDirectoryName(listForm.CurrentItem().FullPathName));
                //    currentMode.PrepareRestore(cmdBox.Text, listForm.VirtualListIndex, listForm.SortKey, listForm.ListViewItems);
                //}
                //if (!currentMode.IsCrawlMode()) return;


                //if (!currentMode.CrawlUp() && !currentMode.Crawl(Path.GetDirectoryName(listForm.CurrentItem().FullPathName)))
                //    return;

                if (!currentMode.CrawlUp(listForm.CurrentItem().FullPathName)) return;

                if (!currentMode.IsRestorePrepared()) {
                    currentMode.PrepareRestore(cmdBox.Text, listForm.VirtualListIndex, listForm.SortKey,
                        listForm.ListViewItems, listForm.listView.Columns[0].Width, listForm.Width);
                }

                // Continue process for crawl
                cmdBox.Text = string.Empty;

                listForm.ModeCaption = currentMode.GetCrawlCaption();
                listForm.SetVirtualList(currentMode.GetCrawlFileSet().ToList());

                listForm.ShowAt();
                Activate();
            }
            // Crawl folder downwards
            if (e.KeyCode == Keys.OemPeriod && e.Control) {
                if (!listForm.Visible) return;

                if (!currentMode.CrawlDown(listForm.CurrentItem().FullPathName)) return;

                if (!currentMode.IsRestorePrepared()) {
                    currentMode.PrepareRestore(cmdBox.Text, listForm.VirtualListIndex, listForm.SortKey,
                        listForm.ListViewItems, listForm.listView.Columns[0].Width, listForm.Width);
                }

                // Continue process for crawl
                cmdBox.Text = string.Empty;

                listForm.ModeCaption = currentMode.GetCrawlCaption();
                listForm.SetVirtualList(currentMode.GetCrawlFileSet().ToList());

                listForm.ShowAt();
                Activate();
            }
            // Exit crawl mode
            if (e.KeyCode == Keys.G && e.Control) {
                if (!listForm.Visible) return;
                if (!currentMode.IsCrawlMode()) return;

                currentMode.ExitCrawl();

                currentMode.ActivateRestore();
                listForm.ModeCaption = null;
                listForm.SortKey = currentMode.RestoreSortKey();
                listForm.SetVirtualList(currentMode.RestoreItems());
                listForm.listView.Columns[0].Width = currentMode.RestoreColWidth();
                listForm.Width = currentMode.RestoreFormWidth();
                cmdBox.Text = currentMode.RestoreCmdBoxText();
                listForm.ShowAt(null, null, currentMode.RestoreIndex());
                currentMode.ExitRestore();

                Activate();
            }

            // TODO: Cycle backwards ListView sort key
            // TODO: implement search history using M-p, M-n
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            // TODO: consider when to save fileList
            // SettingManager.SaveSettings<FileList>(fileList, fileListDataPath);
        }

        private void cmdBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control) {
                e.IsInputKey = true;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Move MainForm by left button dragging
            if (e.Button == MouseButtons.Left) {
                dragStart = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Move MainForm by left button dragging
            if (e.Button == MouseButtons.Left) {
                Location = new Point(Location.X + e.Location.X - dragStart.X,
                                     Location.Y + e.Location.Y - dragStart.Y);
            }
        }

        [GeneratedRegex(@"\w*\W*")]
        private static partial Regex NextWordRegex();

        // Using Non-backtracking and negative lookahead assertion of Regex
        [GeneratedRegex(@"(?>\w*\W*)(?!\w)")]
        private static partial Regex PreviousWordRegex();
    }
}
