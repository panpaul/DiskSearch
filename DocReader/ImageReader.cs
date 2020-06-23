using System.IO;
using ImageReader;

namespace DocReader
{
    internal class ImageReader : IReader
    {
        private readonly FileInfo _file;
        private static readonly ImageTag _imageReader = new ImageTag();

        public ImageReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            return _imageReader.ClassifySingleImage(_file.FullName);
        }
    }
}