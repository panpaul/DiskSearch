using System;
using System.IO;
using System.Text.Json;

namespace Utils
{
    public class Config
    {
        private readonly string _configPath;

        public Config()
        {
        }

        public Config(string configPath)
        {
            try
            {
                if (File.Exists(configPath))
                    _configPath = configPath;
                else
                    _configPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory ?? ".",
                        "config.json"
                    );
                Read();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string SearchPath { get; set; }

        public void Save()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(this);
                File.WriteAllText(_configPath, jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Read()
        {
            var jsonString = File.ReadAllText(_configPath);
            var config = JsonSerializer.Deserialize<Config>(jsonString);
            SearchPath = config.SearchPath;
        }
    }
}