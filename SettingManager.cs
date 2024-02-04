﻿using System;
using System.IO;
using System.Text.Json;

namespace MiLauncher
{
    // Used to save AppSettings and FileList object in the specified files
    internal class SettingManager
    {
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
