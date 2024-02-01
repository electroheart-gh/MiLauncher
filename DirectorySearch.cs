using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLauncher
{
    public class DirectorySearch
    {
        public static IEnumerable<string> EnumerateAllFiles(string path)
        {
            return EnumerateAllFiles(path, "*");
        }
        public static IEnumerable<string> EnumerateAllFiles(string path, string searchPattern)
        {
            var files = Enumerable.Empty<string>();
            try
            {
                //files = System.IO.Directory.EnumerateFiles(path, searchPattern);
                files = System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern);
            }
            catch (System.UnauthorizedAccessException)
            {
            }
            try
            {
                files = System.IO.Directory.EnumerateDirectories(path)
                    .Aggregate(files, (a, v) => a.Union(EnumerateAllFiles(v, searchPattern)));
            }
            catch (System.UnauthorizedAccessException)
            {
            }
            return files;
        }
    }
}
