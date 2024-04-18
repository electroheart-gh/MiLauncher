using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace MiLauncher
{
    /// <summary>
    /// Provides functionality and information to control Crawl mode, 
    /// which is a mode to search files in a specified directory.
    /// </summary>
    internal class CrawlMode
    {
        //
        // Properties
        //
        internal string CrawlPath { get; private set; }
        internal HashSet<FileStats> CrawlFileSet { get; set; }
        //internal HashSet<FileStats> CrawlFileSet { get; private set; }
        internal string Caption { get; private set; }
        public ModeStatus Status { get; private set; }

        //
        //Constructor
        //
        private CrawlMode(string path, HashSet<FileStats> sourceFileSet = null)
        {
            CrawlPath = path;
            CrawlFileSet = FileSet.SearchFilesInPath(path);
            if (sourceFileSet is not null)
                CrawlFileSet = CrawlFileSet?.ImportPriorityAndExecTime(sourceFileSet);
            
            Status = CrawlFileSet is null ? ModeStatus.Defective : ModeStatus.Active;
            Caption = path is null ? null : string.Format("Crawling in: {0}", path);
        }

        //
        // Methods
        //
        internal static CrawlMode Crawl(string path, HashSet<FileStats> sourceFileSet = null)
        {
            if (path is null) return null;

            var newCrawlMode = new CrawlMode(path, sourceFileSet);
            return newCrawlMode.Status == ModeStatus.Active ? newCrawlMode : null;
        }

        //internal CrawlMode CrawlUp(HashSet<FileStats> sourceFileSet = null)
        //{
        //    var upperPath = Path.GetDirectoryName(CrawlPath);
        //    return Crawl(upperPath, sourceFileSet);
        //}
    }
}
