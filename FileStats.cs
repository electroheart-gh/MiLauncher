using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MiLauncher
{
    internal class FileStats
    {
        // JsonSerializer requires 'set' or 'private set' with [JsonInclude] 
        [JsonInclude]
        public string FullPathName { get; private set; }
        [JsonInclude]
        public string FileName { get; private set; }
        [JsonInclude]
        public DateTime UpdateTime { get; private set; }
        [JsonInclude]
        public int Priority { get; set; }
        [JsonInclude]
        public DateTime ExecTime { get; set; }

        public FileStats()
        {
        }
        public FileStats(string pathName, DateTime? updateTime = null, int? priority = null, DateTime? execTime = null)
        {
            FullPathName = pathName;
            FileName = Path.GetFileName(pathName);
            UpdateTime = updateTime ?? File.GetLastWriteTime(pathName);
            Priority = priority ?? 0;
            ExecTime = execTime ?? default;
        }

        internal object SortValue(SortKeyOption key)
        {
            return key switch {
                SortKeyOption.FullPathName => FullPathName,
                SortKeyOption.UpdateTime => UpdateTime,
                SortKeyOption.ExecTime=> ExecTime,
                _ => Priority,
            };
        }
    }

    public enum SortKeyOption
    {
        Priority,
        ExecTime,
        UpdateTime,
        FullPathName,
    }
}
