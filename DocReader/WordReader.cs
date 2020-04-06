using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using Sentry;

namespace DocReader
{
    internal class WordReader : IReader
    {
        private readonly FileInfo _file;

        public WordReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            try
            {
                var wDoc = WordprocessingDocument.Open(_file.FullName, false);
                var body = wDoc.MainDocumentPart.Document.Body;
                var content = new StringBuilder();
                foreach (var element in body.Elements()) content.Append(element.InnerText);
                wDoc.Close();
                return content.ToString();
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