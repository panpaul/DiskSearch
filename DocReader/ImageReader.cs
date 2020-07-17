using System.IO;
using ImageReader;

namespace DocReader
{
    internal class ImageReader : IReader
    {
        private static readonly ImageTag Image = new ImageTag();
        private readonly FileInfo _file;

        public ImageReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            return Image.ClassifySingleImage(_file.FullName);
        }
    }
}