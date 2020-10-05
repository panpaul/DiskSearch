using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Utils
{
    public class Blacklist
    {
        private readonly string _blacklistPath;

        public Blacklist(string blacklistPath)
        {
            try
            {
                if (File.Exists(blacklistPath))
                    _blacklistPath = blacklistPath;
                else
                    _blacklistPath =
                        Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory ?? ".",
                            "blacklist.json"
                        );
                var jsonString = File.ReadAllText(_blacklistPath);
                List = JsonSerializer.Deserialize<List<string>>(jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                List = new List<string>();
            }
        }

        public List<string> List { get; set; }

        public bool Judge(string filename)
        {
            filename = filename.ToLower();
            return List.Any(filename.Contains);
        }

        public void Save()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(List);
                File.WriteAllText(_blacklistPath, jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Update()
        {
            var jsonString = File.ReadAllText(_blacklistPath);
            List = JsonSerializer.Deserialize<List<string>>(jsonString);
        }
    }
}