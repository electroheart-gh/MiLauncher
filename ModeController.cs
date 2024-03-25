﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MiLauncher
{
    /// <summary>
    /// Provides functionality to integrates control of <see cref="CrawlMode"/>,
    /// <see cref="RestoreMode"/> and normal mode 
    /// </summary>
    internal class ModeController
    {
        // Mode and Status
        // CrawlMode: Null, Defective, Active
        // RestoreMode: Null, Defective, Prepared, Active
        private CrawlMode crawlMode;
        private RestoreMode restoreMode;

        internal ModeController()
        {
        }

        //
        // Methods for Crawl mode
        //
        internal bool Crawl(string path)
        {
            // Used to start crawl
            crawlMode = CrawlMode.Crawl(path);
            return crawlMode.Status == ModeStatus.Active;
        }
        internal bool CrawlUp(string itemPath)
        {
            // Returns false when no change needed
            // If Crawl mode active, use CrawlPath instead of itemPath
            if (IsCrawlMode()) {
                CrawlMode crawlResult = crawlMode.CrawlUp();
                crawlMode = (crawlResult?.Status == ModeStatus.Active) ? crawlResult : crawlMode;
                return crawlResult is not null;
            }
            else {
                crawlMode = CrawlMode.Crawl(Path.GetDirectoryName(itemPath));
                return crawlMode is not null;
            }
        }
        internal bool CrawlDown(string itemPath)
        {
            CrawlMode crawlResult = CrawlMode.Crawl(itemPath);
            crawlMode = (crawlResult?.Status == ModeStatus.Active) ? crawlResult : crawlMode;
            return crawlMode is not null;
        }
        internal bool IsCrawlMode()
        {
            return crawlMode?.Status == ModeStatus.Active;
        }
        internal string GetCrawlCaption()
        {
            return crawlMode.Caption;
        }
        internal HashSet<FileStats> GetCrawlFileSet()
        {
            return crawlMode?.CrawlFileSet;
        }
        internal void ExitCrawl()
        {
            // TODO: consider to dispose instance
            crawlMode = null;
        }

        //
        // Methods for Restore mode
        //
        internal bool IsRestoreMode()
        {
            return restoreMode?.Status == ModeStatus.Active;
        }
        internal bool IsRestorePrepared()
        {
            return restoreMode?.Status == ModeStatus.Prepared || restoreMode?.Status == ModeStatus.Active;
        }
        internal void PrepareRestore(string text, int index, SortKeyOption sortKey, List<FileStats> items)
        {
            restoreMode = new RestoreMode(text, index, sortKey, items);
        }
        internal void ActivateRestore()
        {
            restoreMode.Status = ModeStatus.Active;
        }
        internal SortKeyOption RestoreSortKey()
        {
            return restoreMode.SavedSortKey;
        }
        internal List<FileStats> RestoreItems()
        {
            return restoreMode.SavedItems;
        }
        internal string RestoreCmdBoxText()
        {
            return restoreMode.SavedText;
        }
        internal int RestoreIndex()
        {
            return restoreMode.SavedIndex;
        }
        internal void ExitRestore()
        {
            // TODO: consider to dispose instance
            restoreMode = null;
        }

        //
        // Methods for Plain mode
        //
        internal bool IsPlain()
        {
            return !IsCrawlMode() && !IsRestoreMode();
        }
    }
    internal enum ModeStatus
    {
        Defective,
        Prepared,
        Active,
    }
}
