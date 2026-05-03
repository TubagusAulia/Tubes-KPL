using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ManajemenPerpus.Core.Helper
{
    public static class JsonHelper
    {
        public static string GetSharedDataPath(string fileName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? dir = new DirectoryInfo(baseDir);

            while (dir != null)
            {
                string sharedDataPath = Path.Combine(dir.FullName, "SharedData", "DataJson");
                if (Directory.Exists(sharedDataPath))
                {
                    return Path.Combine(sharedDataPath, fileName);
                }
                dir = dir.Parent;
            }

            // Fallback just in case
            string fallbackDir = Path.Combine(baseDir, "SharedData", "DataJson");
            if (!Directory.Exists(fallbackDir))
            {
                Directory.CreateDirectory(fallbackDir);
            }
            return Path.Combine(fallbackDir, fileName);
        }

        public static void WriteJson<T>(string filePath, List<T> data)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static List<T> ReadJson<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>(); // Return empty list instead of crashing on a fresh git clone
            }
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<T>>(jsonString, options) ?? new List<T>();
            }
            catch(JsonException ex)
            {
                throw new InvalidOperationException($"Error deserializing JSON from file: {filePath}", ex);
            }
        }
    }
}
