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

        public Blacklist()
        {
            try
            {
                var jsonString = File.ReadAllText("./blacklist.json");
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
            return _list.Any(filename.Contains);
        }
    }
}