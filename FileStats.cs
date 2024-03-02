using KaoriYa.Migemo;
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
        public FileStats(string pathName, DateTime? updateTime = null, int priority = 0, DateTime? execTime = null)
        {
            FullPathName = pathName;
            FileName = Path.GetFileName(pathName);
            UpdateTime = updateTime ?? File.GetLastWriteTime(pathName);
            Priority = priority;
            ExecTime = execTime ?? default;
        }

        public bool IsMatchAllPatterns(IEnumerable<string> patterns)
        {
            // TODO: consider to use LINQ with MatchCondition class
            foreach (var pattern in patterns)
            {
                if (!(pattern[..1] switch
                {
                    "-" => !IsMatchPattern(FullPathName, pattern[1..]),
                    "!" => !IsMatchPattern(FileName, pattern[1..]),
                    "\\" => IsMatchPattern(FullPathName, pattern[1..]),
                    _ => IsMatchPattern(FileName, pattern),
                }))
                {
                    return false;
                }
            }
            return true;

            bool IsMatchPattern(string name, string pattern)
            {
                // Simple search
                if (pattern.Length < Program.appSettings.MinMigemoLength)
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
                        if (Regex.IsMatch(name, pattern.ToString(), RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch (ArgumentException)
                    {
                        if (name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        internal void AddPriority(int value)
        {
            Priority += value;
        }

        internal object SortValue(SortKeyOption key)
        {
            return key switch
            {
                SortKeyOption.Priority => Priority,
                SortKeyOption.FullPathName => FullPathName,
                SortKeyOption.UpdateTime => UpdateTime,
                _ => Priority,
            };
        }
    }

    public enum SortKeyOption
    {
        Priority,
        FullPathName,
        UpdateTime,
    }
}
