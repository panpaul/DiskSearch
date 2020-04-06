using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using Sentry;

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
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                SentrySdk.CaptureException(e);
                return _file.Extension == "" ? _file.Name : _file.Name.Replace(_file.Extension, "");
            }
        }
    }
}