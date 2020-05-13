using System;
using System.IO;
using System.Text.Json;

namespace DiskSearch.GUI
{
    internal class Config
    {
        public Config()
        {
        }

        public Config(string configPath)
        {
            try
            {
                string jsonString;
                if (File.Exists(configPath))
                    jsonString = File.ReadAllText(configPath);
                else
                    jsonString =
                        File.ReadAllText(
                            Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory ?? ".",
                                "config.json"
                            )
                        );

                var config = JsonSerializer.Deserialize<Config>(jsonString);
                SearchPath = config.SearchPath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string SearchPath { get; set; }
    }
}