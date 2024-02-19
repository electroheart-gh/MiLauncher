using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MiLauncher
{
    internal class FileSet
    {
        // Variables and Properties
        // JsonSerializer requires properties instead of fields
        private HashSet<FileInfo> items = [];
        public HashSet<FileInfo> Items { get => items; set => items = value; }

        public FileSet()
        {
        }
        public FileSet(IEnumerable<FileInfo> fileInfoSet)
        {
            Items.UnionWith(fileInfoSet);
        }

        internal List<string> SelectWithCancellation(IEnumerable<string> patterns, CancellationToken token)
        {
            // Variables
            var selectedList = new List<string>();

            try
            {
                foreach (var fileInfo in Items)
                {
                    token.ThrowIfCancellationRequested();

                    if (fileInfo.IsMatchAllPatterns(patterns))
                    {
                        selectedList.Add(fileInfo.FullPathName);
                        if (selectedList.Count > Program.appSettings.MaxListLine)
                        {
                            break;
                        }
                    }
                }
                return selectedList;
            }
            catch (OperationCanceledException)
            {
                // Debug.WriteLine("cancel occurs Select");
                selectedList.Clear();
                return selectedList;
            }
        }

        internal static FileSet SearchFiles(IEnumerable<string> searchPaths)
        {
            return new FileSet(
                searchPaths.SelectMany(x => DirectorySearch.EnumerateAllFiles(x).Select(fn => new FileInfo(fn))));
        }

        internal List<string> SelectRealtimeSearch(string searchPath, string[] words, CancellationToken token)
        {
            var selectedList = new List<string>();
            var migemo = new Migemo("./Dict/migemo-dict");
            Regex regex;

            try
            {
                //foreach (var fn in DirectorySearch.EnumerateAllFiles(@"C:\Users\JUNJI\Desktop\"))
                foreach (var fn in DirectorySearch.EnumerateAllFiles(searchPath))
                {
                    var patternMatched = true;

                    foreach (var pattern in words)
                    {
                        token.ThrowIfCancellationRequested();
                        // Simple string search
                        if (pattern.Length < Program.appSettings.MinMigemoLength)
                        {
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
                            if (!Regex.IsMatch(Path.GetFileName(fn), regex.ToString(), RegexOptions.IgnoreCase))
                            {
                                patternMatched = false;
                                break;
                            }
                        }
                    }
                    if (patternMatched)
                    {
                        selectedList.Add(fn);

                        if (selectedList.Count > Program.appSettings.MaxListLine)
                        {
                            break;
                        }
                    }
                }
                return selectedList;
            }
            catch (OperationCanceledException)
            {
                selectedList.Clear();
                return selectedList;
            }
        }

        internal void AddPriority(string fullPathName, int value)
        {
            Items.FirstOrDefault(x => x.FullPathName == fullPathName)?.AddPriority(value);
        }
    }
}
