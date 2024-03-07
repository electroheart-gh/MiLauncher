using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MiLauncher
{
    internal static class FileSet
    {
        internal static List<FileStats> SelectWithCancellation
            (IEnumerable<FileStats> sourceFiles, IEnumerable<string> patterns, CancellationToken token)
        {
            try {
                return new List<FileStats>(
                    sourceFiles.AsParallel().WithCancellation(token)
                               .Where(x => IsMatchAllPatterns(x, patterns)));
            }
            catch (OperationCanceledException) {
                // Debug.WriteLine("cancel occurs Select");
                return [];
            }
        }

        private static bool IsMatchAllPatterns(FileStats fileStats, IEnumerable<string> patterns)
        {
            // TODO: consider to use LINQ with MatchCondition class
            foreach (var pattern in patterns) {
                if (!(pattern[..1] switch {
                    "-" => !IsMatchPattern(fileStats.FullPathName, pattern[1..]),
                    "!" => !IsMatchPattern(fileStats.FileName, pattern[1..]),
                    "\\" => IsMatchPattern(fileStats.FullPathName, pattern[1..]),
                    _ => IsMatchPattern(fileStats.FileName, pattern),
                })) {
                    return false;
                }
            }
            return true;

            static bool IsMatchPattern(string name, string pattern)
            {
                // Simple search
                if (pattern.Length < Program.appSettings.MinMigemoLength) {
                    return name.Contains(pattern, StringComparison.OrdinalIgnoreCase);
                }
                // Migemo search
                else {
                    try {
                        return Regex.IsMatch(name, pattern.ToString(), RegexOptions.IgnoreCase);
                    }
                    catch (ArgumentException) {
                        return name.Contains(pattern, StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
        }

        internal static HashSet<FileStats> SearchFiles()
        {
            List<string> searchPaths = Program.appSettings.TargetFolders;
            return new HashSet<FileStats>(
                searchPaths.SelectMany(
                    x => DirectorySearch.EnumerateAllFileSystemEntries(x).Select(fn => new FileStats(fn))));
        }

        internal static HashSet<FileStats> CopyPriorityAndExecTime(IEnumerable<FileStats> sourceFiles,
                                                                   IEnumerable<FileStats> targetFiles)
        {
            return new HashSet<FileStats>(
                targetFiles.GroupJoin(sourceFiles,
                                      x => x.FullPathName,
                                      y => y.FullPathName,
                                      (x, y) => new FileStats(x.FullPathName,
                                                              x.UpdateTime,
                                                              y.FirstOrDefault()?.Priority,
                                                              y.FirstOrDefault()?.ExecTime)));
        }
    }
}
