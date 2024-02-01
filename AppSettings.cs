﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLauncher
{
    internal class AppSettings
    {
        public List<string> TargetFolders{ get; set; }
        public int MigemoMinLength {  get; set; }
        public int MaxListLine {  get; set; }

        //// Configuration class to modify display of lblTaskName
        //// If Pattern match, replace it with Substitution and set the Forecolor
        //public List<NameModifier> NameModifiers { get; set; }
        //public class NameModifier
        //{
        //    public string Pattern { get; set; }
        //    public string Substitution { get; set; }
        //    public string ForeColor { get; set; }

        //    public NameModifier()
        //    {
        //        Pattern = string.Empty;
        //        Substitution = string.Empty;
        //        ForeColor = string.Empty;
        //    }
        //}

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
