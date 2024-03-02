using System;
using System.IO;
using System.Text.Json;

namespace MiLauncher
{
    // Used to save AppSettings and FileList object in the specified files
    internal class SettingManager
    {
        private static readonly JsonSerializerOptions s_writeOptions = new() {
            WriteIndented = true
        };

        public static void SaveSettings<T>(T settingsObject, string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(settingsObject, s_writeOptions));
        }

        public static T LoadSettings<T>(string path)
        {
            try {
                return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
            }
            catch (Exception ex) {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                return default;
            }
        }
    }
}
