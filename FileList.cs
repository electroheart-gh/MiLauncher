﻿using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<FileListInfo> items = [];
        public List<FileListInfo> Items { get => items; set => items = value; }

        public FileList()
        {
            Debug.WriteLine("blank file list");
            //Items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
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
        
        internal List<string> Select(string[] words, CancellationToken token)
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
                        // Console.WriteLine("\"" + regex.ToString() + "\"");  //生成された正規表現をコンソールに出力
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

                        patternMatched = pattern.Substring(0, 1) switch
                        {
                            "-" => !IsPatternMatch(fn.FullPathName, pattern.Substring(1)),
                            "!" => !IsPatternMatch(fn.FileName, pattern.Substring(1)),
                            "\\" => IsPatternMatch(fn.FullPathName, pattern.Substring(1)),
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
                // Console.WriteLine("cancel occurs Select");
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

        internal FileList SearchFiles(IEnumerable<string> searchPaths)
        {
            var fileList = new FileList();

            //foreach (var searchPath in searchPaths)
            //{
            //    foreach (var fn in DirectorySearch.EnumerateAllFiles(searchPath))
            //    {
            //        // Console.WriteLine(Path.GetFileName(file));
            //        fileList.Items.Add(new FileListInfo(Path.GetFileName(fn), fn, 0));
            //    }
            //}

            //fileList.Items.AddRange(
            //    searchPaths.SelectMany(DirectorySearch.EnumerateAllFiles, 
            //                           (_, fn) => new FileListInfo(Path.GetFileName(fn), fn, 0)));

            fileList.Items.AddRange(
                searchPaths.SelectMany(searchPath => DirectorySearch.EnumerateAllFiles(searchPath)
                .Select(fn => new FileListInfo(Path.GetFileName(fn), fn, 0))));

            return fileList;
        }
        public static FileList FileListForTest()
        {
            var fileListTest = new FileList();
            string folderPath = @"C:\Users\JUNJI\Desktop\tools";
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Console.WriteLine(Path.GetFileName(file));
                fileListTest.Items.Add(new FileListInfo(Path.GetFileName(file), file, 0));
            }
            Console.WriteLine("FileListForTest count: " + fileListTest.Items.Count);
            return fileListTest;
        }

    }
}
