using System;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;

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
            var wDoc = WordprocessingDocument.Open(_file.FullName, false);
            var body = wDoc.MainDocumentPart.Document.Body;
            var content = new StringBuilder();
            foreach (var element in body.Elements())
            {
                content.Append(element.InnerText);
            }
            wDoc.Close();
            return content.ToString();
        }
    }
}