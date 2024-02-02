using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLauncher
{
    internal class AppSettings
    {
        // TODO: Consider to use <Record> Type
        public List<string> TargetFolders{ get; set; }
        public int MigemoMinLength {  get; set; }
        public int MaxListLine {  get; set; }

        public AppSettings()
        {
            //var userProfilePath = Environment.GetEnvironmentVariable("UserProfile");
            //TargetFolders = new List<string>();
            TargetFolders = [Environment.GetFolderPath(Environment.SpecialFolder.Desktop),Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)];

            MigemoMinLength = 3;
            MaxListLine = 50;

            // TODO: configuration for Keymap 
            // TODO: configuration for specific application to open file, such as sakura
        }
    }
}
