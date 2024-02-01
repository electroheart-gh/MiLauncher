using System;
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
        static public AppSettings appSettings;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Read configuration
            appSettings = SettingManager.LoadSettings<AppSettings>(configFilePath);
            if (appSettings == null)
            {
                appSettings = new AppSettings();
                SettingManager.SaveSettings(appSettings, configFilePath);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
