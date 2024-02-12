using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
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
            // Debug.WriteLine("blank file list");
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
                    if(fileInfo.IsMatchAllPatterns(patterns))
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

        internal List<string> SelectOld(string[] words, CancellationToken token)
        {
            // Variables
            var selectedList = new List<string>();
            var migemo = new Migemo("./Dict/migemo-dict");
            Regex regex;

            // Local function
            bool IsPatternMatch(string name, string pattern)
            {
                // Simple search
                if (pattern.Length < Program.appSettings.MigemoMinLength)
                {
                    if (name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                // Migemo search
                else
                {
                    try
                    {
                        regex = migemo.GetRegex(pattern);
                        // Debug.WriteLine("\"" + regex.ToString() + "\"");  //生成された正規表現をコンソールに出力
                        if (Regex.IsMatch(name, regex.ToString(), RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch (ArgumentException)
                    {
                        if (name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            // Main Logic
            try
            {
                foreach (var fn in Items)
                {
                    var patternMatched = true;

                    foreach (var pattern in words)
                    {
                        token.ThrowIfCancellationRequested();

                        patternMatched = pattern[..1] switch
                        {
                            "-" => !IsPatternMatch(fn.FullPathName, pattern[1..]),
                            "!" => !IsPatternMatch(fn.FileName, pattern[1..]),
                            "\\" => IsPatternMatch(fn.FullPathName, pattern[1..]),
                            _ => IsPatternMatch(fn.FileName, pattern),
                        };
                        if (!patternMatched)
                        {
                            break;
                        }
                    }
                    if (patternMatched)
                    {
                        selectedList.Add(fn.FullPathName);

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
                        if (pattern.Length < Program.appSettings.MigemoMinLength)
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

    }

}
