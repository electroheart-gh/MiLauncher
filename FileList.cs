using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiLauncher
{
    internal class FileList
    {
        // TODO: Update FileList in async as per config
        // TODO: Implement some ways to update FileList manually

        // Variables and Properties
        // JsonSerializer requires properties instead of fields
        private List<FileListInfo> items = new List<FileListInfo>();
        public List<FileListInfo> Items { get => items; set => items = value; }

        public FileList()
        {
            //
            // for pre-release
            //
            string folderPath = @"C:\Users\JUNJI\Desktop\tools"; // フォルダのパスを指定
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Console.WriteLine(Path.GetFileName(file));
                Items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
            }
            //
            //
            //
        }

        public class FileListInfo
        {
            public string FileName { get; set; }
            public string FullPathName { get; set; }
            public int Priority { get; set; }

            public FileListInfo()
            {
                FileName = "";
                FullPathName = "";
                Priority = 0;
            }
            public FileListInfo(string fileName, string pathName, int priority)
            {
                FileName = fileName;
                FullPathName = pathName;
                Priority = priority;
            }
        }

        //
        // Method for test to create sample File List
        //
        public static FileList FileListForTest()
        {
            FileList fileList = new FileList();
            //string folderPath = @"C:\Users\JUNJI\Desktop\tools";
            string folderPath = @"C:\Users\JUNJI\Desktop\";
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Console.WriteLine(Path.GetFileName(file));
                fileList.Items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
            }
            return fileList;
        }

        internal List<string> Select(string[] words, CancellationToken token)
        {
            List<string> selectedList = new List<string>();
            Migemo migemo = new Migemo("./Dict/migemo-dict");
            Regex regex;

            // TODO: Make them configurable
            const int migemoMinLength = 3;
            const int maxListLine = 50;

            try
            {
                //foreach (var fn in Items)
                //foreach (var fn in DirectorySearch.EnumerateAllFiles(@"C:\Users\JUNJI\"))
                foreach (var fn in DirectorySearch.EnumerateAllFiles(@"C:\Users\JUNJI\Desktop\"))
                {
                    //Console.WriteLine(cancellationToken.IsCancellationRequested);
                    var patternMatched = true;

                    foreach (var pattern in words)
                    {
                         token.ThrowIfCancellationRequested();
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
                        selectedList.Add(fn);
                        // max count should be const and configurable
                        if (selectedList.Count > maxListLine)
                        {
                            break;
                        }
                    }
                }
                return selectedList;
            }
            catch (OperationCanceledException)
            {
                // Console.WriteLine("cancel occurs Select");
                selectedList.Clear();
                return selectedList;
            }

        }
    }
}
