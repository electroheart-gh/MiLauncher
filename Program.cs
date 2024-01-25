﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiLauncher
{
    internal static class Program
    {
        // Global variable for configuration
        static string configFilePath = "myConfig.json";
        static public AppSettings appSettings = new AppSettings();


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Read configuration
            var appSettings = SettingManager.LoadSettings<AppSettings>(configFilePath);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
