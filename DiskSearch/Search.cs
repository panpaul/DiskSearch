using System;
using Database;

namespace DiskSearch
{
    internal class Search
    {
        private readonly Engine _index;

        public Search(string path)
        {
            _index = new Engine(path);
        }

        public void Prompt()
        {
            while (true)
            {
                Console.Write("Search for What ? >");
                var word = Console.ReadLine();
                if (word == null || word.Equals("QUIT"))
                {
                    Close();
                    break;
                }

                Console.WriteLine("==== Searching for : " + word + " ====");
                var schemes = _index.Search(word);
                foreach (var scheme in schemes) Console.WriteLine(scheme.Path);
                Console.WriteLine("==== End Search ====");
            }
        }

        public void Close()
        {
            _index.Close();
        }
    }
}