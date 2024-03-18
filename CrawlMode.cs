using System.Collections.Generic;
using System.IO;

namespace MiLauncher
{
    internal class CrawlMode
    {
        //
        // Properties
        //
        private string _crawlPath;

        internal string CrawlPath
        {
            get { return _crawlPath; }
            set {
                _crawlPath = value;
                CrawlFileSet = FileSet.SearchFilesInPath(value)?.ImportPriorityAndExecTime(SavedItems);
                Caption = string.Format("Crawling in: {0}", value);
            }
        }
        internal HashSet<FileStats> CrawlFileSet { get; private set; }
        internal string Caption { get; private set; }

        internal string SavedCmdBoxText { get; }
        internal int SavedIndex { get; }
        internal SortKeyOption SavedSortKey { get; }
        internal List<FileStats> SavedItems { get; }

        private CrawlMode(string path, string cmdBoxText, int index, SortKeyOption sortKey, List<FileStats> items)
        {
            SavedCmdBoxText = cmdBoxText;
            SavedIndex = index;
            SavedSortKey = sortKey;
            SavedItems = items;

            // To use SavedItems, set CrawPath at last
            CrawlPath = path;
        }
        internal bool IsValid()
        {
            return CrawlFileSet is not null;
        }

        internal static CrawlMode Crawl(string path, string cmdBoxText, int index, SortKeyOption sortKey, List<FileStats> items)
        {
            if (path is null) return null;

            var newCrawlMode = new CrawlMode(path, cmdBoxText, index, sortKey, items);
            return newCrawlMode.IsValid() ? newCrawlMode : null;

        }

        //internal void Crawl(string targetPath = null)
        //{
        //    targetPath ??= Path.GetDirectoryName(_crawlPath);

        //    HashSet<FileStats> result = FileSet.SearchFilesInPath(targetPath);
        //    if (result is null) return;

        //    CrawledFileSet = result;
        //    _crawlPath = targetPath;
        //}

        internal CrawlMode CrawlUp()
        {
            var newPath = Path.GetDirectoryName(CrawlPath);
            if (newPath == null) return null;

            var newCrawlMode = new CrawlMode(newPath, SavedCmdBoxText, SavedIndex, SavedSortKey, SavedItems);
            return newCrawlMode.IsValid() ? newCrawlMode : null;
        }

    }
}
