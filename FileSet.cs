using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace MiLauncher
{
    internal class FileSet
    {
        // Variables and Properties
        // JsonSerializer requires properties instead of fields
        [JsonInclude]
        public HashSet<FileStats> Items { get; set; } = [];

        public FileSet()
        {
        }
        public FileSet(IEnumerable<FileStats> fileInfoSet)
        {
            Items.UnionWith(fileInfoSet);
        }

        internal List<FileStats> SelectWithCancellation(IEnumerable<string> patterns, CancellationToken token)
        {
            // Variables
            var selectedList = new List<FileStats>();

            try {
                foreach (var fileStats in Items) {
                    token.ThrowIfCancellationRequested();

                    if (fileStats.IsMatchAllPatterns(patterns)) {
                        selectedList.Add(fileStats);
                    }
                }
                return selectedList;
            }
            catch (OperationCanceledException) {
                // Debug.WriteLine("cancel occurs Select");
                selectedList.Clear();
                return selectedList;
            }
        }
        internal static FileSet SearchFiles(IEnumerable<string> searchPaths)
        {
            return new FileSet(
                searchPaths.SelectMany(
                    x => DirectorySearch.EnumerateAllFileSystemEntries(x).Select(fn => new FileStats(fn))));
        }
        internal FileSet CopyPriority(FileSet sourceFileSet)
        {
            return new FileSet(
                Items.GroupJoin(sourceFileSet.Items,
                                x => x.FullPathName,
                                y => y.FullPathName,
                                (x, y) => new FileStats(x.FullPathName, x.UpdateTime, y.FirstOrDefault()?.Priority ?? 0)));
        }

        //internal List<string> SelectRealtimeSearch(string searchPath, string[] words, CancellationToken token)
        //{
        //    var selectedList = new List<string>();
        //    var migemo = new Migemo("./Dict/migemo-dict");
        //    Regex regex;

        //    try {
        //        //foreach (var fn in DirectorySearch.EnumerateAllFiles(@"C:\Users\JUNJI\Desktop\"))
        //        foreach (var fn in DirectorySearch.EnumerateAllFileSystemEntries(searchPath)) {
        //            var patternMatched = true;

        //            foreach (var pattern in words) {
        //                token.ThrowIfCancellationRequested();
        //                // Simple string search
        //                if (pattern.Length < Program.appSettings.MinMigemoLength) {
        //                    if (!Path.GetFileName(fn).Contains(pattern)) {
        //                        patternMatched = false;
        //                        break;
        //                    }
        //                }
        //                // Migemo search
        //                else {
        //                    try {
        //                        regex = migemo.GetRegex(pattern);
        //                    }
        //                    catch (ArgumentException) {
        //                        regex = new Regex(pattern);
        //                    }
        //                    // Debug.WriteLine("\"" + regex.ToString() + "\"");  //生成された正規表現をデバッグコンソールに出力
        //                    if (!Regex.IsMatch(Path.GetFileName(fn), regex.ToString(), RegexOptions.IgnoreCase)) {
        //                        patternMatched = false;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (patternMatched) {
        //                selectedList.Add(fn);

        //                if (selectedList.Count > Program.appSettings.MaxListLine) {
        //                    break;
        //                }
        //            }
        //        }
        //        return selectedList;
        //    }
        //    catch (OperationCanceledException) {
        //        selectedList.Clear();
        //        return selectedList;
        //    }
        //}

        internal void AddPriority(string fullPathName, int value)
        {
            Items.FirstOrDefault(x => x.FullPathName == fullPathName)?.AddPriority(value);
        }
    }
}
