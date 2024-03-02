using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiLauncher
{
    public class DirectorySearch
    {
        public static IEnumerable<string> EnumerateAllFileSystemEntries(string path)
        {
            return EnumerateAllFileSystemEntries(path, "*");
        }
        public static IEnumerable<string> EnumerateAllFileSystemEntries(string path, string searchPattern)
        {
            var files = Enumerable.Empty<string>();
            try
            {
                //files = System.IO.Directory.EnumerateFiles(path, searchPattern);
                files = Directory.EnumerateFileSystemEntries(path, searchPattern);
            }
            catch (UnauthorizedAccessException)
            {
            }
            try
            {
                files = Directory.EnumerateDirectories(path)
                    .Aggregate(files, (a, v) => a.Union(EnumerateAllFileSystemEntries(v, searchPattern)));
            }
            catch (UnauthorizedAccessException)
            {
            }
            return files;
        }

        // EnumerateAllFileSystemInfos created temporarily, which is not tested
        public static IEnumerable<FileSystemInfo> EnumerateAllFileSystemInfos(string path)
        {
            return EnumerateAllFileSystemInfos(path, "*");
        }
        private static IEnumerable<FileSystemInfo> EnumerateAllFileSystemInfos(string path, string searchPattern)
        {
            var files = Enumerable.Empty<FileSystemInfo>();
            try
            {
                files = new DirectoryInfo(path).EnumerateFileSystemInfos(searchPattern);
            }
            catch (UnauthorizedAccessException)
            {
            }
            try
            {
                files = Directory.EnumerateDirectories(path)
                    .Aggregate(files, (a, v) => a.Union(EnumerateAllFileSystemInfos(v)));
            }
            catch (UnauthorizedAccessException)
            {
            }
            return files;

        }
    }
}
