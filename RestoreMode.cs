using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLauncher
{
    /// <summary>
    /// Stores information and status to control Restore mode, 
    /// which is a short-life mode to restore the snapshot of MainForm and ListForm
    /// </summary>
    internal class RestoreMode
    {
        public RestoreMode(string text, int index, SortKeyOption sortKey, List<FileStats> items)
        {
            SavedText = text;
            SavedIndex = index;
            SavedSortKey = sortKey;
            SavedItems = items;
            Status = ModeStatus.Prepared;
        }

        public ModeStatus Status { get; internal set; }

        //private CrawlMode(string path, string cmdBoxText, int index, SortKeyOption sortKey, List<FileStats> items)
        internal SortKeyOption SavedSortKey { get; }
        internal string SavedText { get; }
        internal List<FileStats> SavedItems { get; }
        internal int SavedIndex { get; }

    }
}
