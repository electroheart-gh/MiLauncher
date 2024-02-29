using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiLauncher
{
    internal class SearchedFileSet : FileSet
    {
        // TODO: Update FileList in async as per config
        // TODO: Implement some ways to update FileList manually

        //public SearchedFileSet()
        //{
        //    // Debug.WriteLine("blank file list");
        //}

        internal static SearchedFileSet SearchFilesNew(IEnumerable<string> searchPaths)
        {
            var fileSet = new SearchedFileSet();

            fileSet.Items.UnionWith(
                searchPaths.SelectMany(x => DirectorySearch.EnumerateAllFileSystemEntries(x).Select(fn => new FileStats(fn)))
                );

            return fileSet;

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
        }

        //public static SearchedFileSet FileListForTest()
        //{
        //    var fileListTest = new SearchedFileSet();

        //    string folderPath = @"C:\Users\JUNJI\Desktop\tools";
        //    string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

        //    foreach (string file in files)
        //    {
        //        // Debug.WriteLine(Path.GetFileName(file));
        //        fileListTest.Items.Add(new FileInfo(file));
        //    }
        //    //Debug.WriteLine("FileListForTest count: " + fileListTest.Items.Count);
        //    return fileListTest;
        //}

        //internal void AddPriority(string fullPathName, int value)
        //{
        //    Items.FirstOrDefault(x => x.FullPathName == fullPathName)?.AddPriority(value);
        //}
    }
}
