using System;
using System.IO;

namespace DocReader
{
    internal class TextReader : IReader
    {
        private readonly FileInfo _file;

        public TextReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            try
            {
                return File.ReadAllText(_file.FullName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _file.Extension == "" ? _file.Name : _file.Name.Replace(_file.Extension, "");
            }
        }
    }
}