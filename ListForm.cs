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

        // Constant
        // TODO: Make them configurable
        const int migemoMinLength = 3;
        const int maxListLine = 50;

        public ListForm()
        {
            InitializeComponent();
        }

        internal bool Reset(FileList fileList, string text)
        {
            // Update listView
            listView.Items.Clear();

            // TODO: Parse text to exec special command such as multi pattern search,  full path search, calc etc.
            // TODO: Consider to change async method because performance issue should happen with network drive
            // TODO: examine if hyphen(-) works properly in regex

            // Simple Parse by space to split patterns
            string[] patterns = text.Split(' ');

            // Create list view by pattern matching from fileList
            // TODO: change or combine input of list view by user command or config,
            // real time search and/or pre-scanned file list (priority, access time, created time etc.)

            //foreach (var fn in fileList.Items)
            //foreach (var fn in Directory.EnumerateFiles(@"C:\Users\JUNJI\", "*", SearchOption.AllDirectories))
            foreach (var fn in DirectorySearch.EnumerateAllFiles(@"C:\Users\JUNJI\Desktop\"))
            {
                var patternMatched = true;

                foreach (var pattern in patterns)
                {
                    // Simple string search
                    if (pattern.Length < migemoMinLength)
                    {
                        //if (!fn.FileName.Contains(pattern))
                        if (!Path.GetFileName(fn).Contains(pattern))
                        {
                            patternMatched = false;
                            break;
                        }
                    }
                    // Migemo search
                    else
                    {
                        try
                        {
                            regex = migemo.GetRegex(pattern);
                        }
                        catch (ArgumentException)
                        {
                            regex = new Regex(pattern);
                        }
                        // Debug.WriteLine("\"" + regex.ToString() + "\"");  //生成された正規表現をデバッグコンソールに出力
                        //if (!Regex.IsMatch(fn.FileName, regex.ToString(), RegexOptions.IgnoreCase))
                        if (!Regex.IsMatch(Path.GetFileName(fn), regex.ToString(), RegexOptions.IgnoreCase))
                        {
                            patternMatched = false;
                            break;
                        }
                    }
                }
                if (patternMatched)
                {
                    //listView.Items.Add(fn.FullPathName);
                    listView.Items.Add(fn);
                    // max count should be const and configurable
                    if (listView.Items.Count > maxListLine)
                    {
                        break;
                    }
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

            // Select the first line and change its color
            listView.Items[0].Selected = true;
            return true;
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.Item.Selected)
            {
                // TODO: make it configurable
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            }
            e.DrawText();
        }
    }
}
