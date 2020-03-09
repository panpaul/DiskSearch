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

        

        private void Close()
        {
            _index.Close();
        }
    }
}