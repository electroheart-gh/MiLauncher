using KaoriYa.Migemo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MiLauncher
{
    internal class FileInfo
    {
        // If no 'set', JsonSerializer not working
        public string FullPathName { get; set; }
        public string FileName { get; set; }

        //static readonly Migemo migemo = new("./Dict/migemo-dict");


        public FileInfo()
        {
        }
        public FileInfo(string pathName)
        {
            FullPathName = pathName;
            FileName = Path.GetFileName(pathName);
        }


        public bool IsMatchAllPatterns(IEnumerable<string> patterns)
        {
            // TODO: consider to use linq
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
                if (pattern.Length < Program.appSettings.MigemoMinLength)
                {
                    if (name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                // Migemo search
                else
                {
                    // Migemo migemo = new("./Dict/migemo-dict");
                    // var regex = migemo.GetRegex(pattern);
                    try
                    {
                        //var regex = new Migemo("./Dict/migemo-dict").GetRegex(pattern);

                        // Debug.WriteLine("\"" + regex.ToString() + "\"");  //生成された正規表現をコンソールに出力
                        if (Regex.IsMatch(name, pattern.ToString(), RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                    }
                    catch (ArgumentException)
                    {
                        //if (name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) > 0)
                        if (name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

    }

}
