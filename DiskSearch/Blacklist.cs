using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DiskSearch
{
    internal class Blacklist
    {
        private readonly List<string> _list;

        public Blacklist(string blacklistPath)
        {
            try
            {
                string jsonString;
                if (File.Exists(blacklistPath))
                    jsonString = File.ReadAllText(blacklistPath);
                else
                    jsonString =
                        File.ReadAllText(
                            Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory ?? ".",
                                "blacklist.json"
                            )
                        );

                _list = JsonSerializer.Deserialize<List<string>>(jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _list = new List<string>();
            }
        }

        public bool Judge(string filename)
        {
            filename = filename.ToLower();
            return _list.Any(filename.Contains);
        }
    }
}