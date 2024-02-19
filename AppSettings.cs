using System;
using System.Collections.Generic;

namespace MiLauncher
{
    internal class AppSettings
    {
        // TODO: Consider to use <Record> Type
        public List<string> TargetFolders { get; set; }
        public int MinMigemoLength { get; set; }
        public int MaxListLine { get; set; }

        public AppSettings()
        {
            //var userProfilePath = Environment.GetEnvironmentVariable("UserProfile");
            //TargetFolders = new List<string>();
            TargetFolders = [
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)                ];

            MinMigemoLength = 3;
            MaxListLine = 50;

            // TODO: configuration for Keymap 
            // TODO: configuration for specific application to open file, such as sakura
        }
    }
}
