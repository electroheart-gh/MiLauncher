using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MiLauncher
{
    internal class SearchedFileSet : FileSet
    {
        // TODO: Update FileList in async as per config
        // TODO: Implement some ways to update FileList manually

        public SearchedFileSet()
        {
            // Debug.WriteLine("blank file list");
        }

        internal static SearchedFileSet SearchFiles(IEnumerable<string> searchPaths)
        {
            var fileSet = new SearchedFileSet();

            //foreach (var searchPath in searchPaths)
            //{
            //    foreach (var fn in DirectorySearch.EnumerateAllFiles(searchPath))
            //    {
            //        // Debug.WriteLine(Path.GetFileName(file));
            //        fileList.Items.Add(new FileListInfo(Path.GetFileName(fn), fn, 0));
            //    }
            //}

            //fileList.Items.AddRange(
            //    searchPaths.SelectMany(DirectorySearch.EnumerateAllFiles,
            //                           (_, fn) => new FileListInfo(Path.GetFileName(fn), fn, 0)));

            fileSet.Items.UnionWith(
                searchPaths.SelectMany(searchPath => DirectorySearch.EnumerateAllFiles(searchPath)
                .Select(fn => new FileInfo(fn))));

            return fileSet;
        }
        public static SearchedFileSet FileListForTest()
        {
            var fileListTest = new SearchedFileSet();

            string folderPath = @"C:\Users\JUNJI\Desktop\tools";
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Debug.WriteLine(Path.GetFileName(file));
                fileListTest.Items.Add(new FileInfo(file));
            }
            //Debug.WriteLine("FileListForTest count: " + fileListTest.Items.Count);
            return fileListTest;
        }

        internal void UpdateHistory(string fullPathName)
        {
            throw new NotImplementedException();
        }
    }
}
