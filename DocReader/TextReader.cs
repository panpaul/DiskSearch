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
            return File.ReadAllText(_file.FullName);
        }
    }
}