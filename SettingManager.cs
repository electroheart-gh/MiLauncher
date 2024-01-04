using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiLauncher
{
    internal class SettingManager
    {
        // TODO: Change place to define paths below
        //private string settingsFilePath = "mySettings.json"; // 設定ファイルのパス
        //private string fileListDataPath = "FileList.dat";

        public static void SaveSettings<T>(T settingsObject, string path)
        {
            string json = JsonSerializer.Serialize(settingsObject, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public static T LoadSettings<T>(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                return default;
            }
        }
    }
}
