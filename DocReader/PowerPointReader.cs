using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;

namespace DocReader
{
    internal class PowerPointReader : IReader
    {
        private readonly FileInfo _file;

        public PowerPointReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            var sb = new StringBuilder();
            var presentation = PresentationDocument.Open(_file.FullName, false);
            var presentationPart = presentation.PresentationPart;
            foreach (var slidePart in presentationPart.SlideParts)
            {
                sb.Append(slidePart.Slide.InnerText);
                sb.Append(" ");
            }

            presentation.Close();
            return sb.ToString();
        }
    }
}