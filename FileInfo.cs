using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MiLauncher
{
    internal class FileInfo
    {
        // JsonSerializer requires 'set' or 'private set' with [JsonInclude] 
        [JsonInclude]
        public string FullPathName { get; private set; }

        [JsonInclude]
        public string FileName { get; private set; }

        [JsonInclude]
        public int Priority { get; set; }

        public FileInfo()
        {
        }
        public FileInfo(string pathName)
        {
            FullPathName = pathName;
            FileName = Path.GetFileName(pathName);
        }
        public FileInfo(string pathName, int priority)
        {
            FullPathName = pathName;
            FileName = Path.GetFileName(pathName);
            Priority = priority;
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
    }
}
