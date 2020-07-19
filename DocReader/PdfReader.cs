using System;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Sentry;

namespace DocReader
{
    internal class PDFReader : IReader
    {
        private readonly FileInfo _file;

        public PDFReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            try
            {
                var reader = new PdfReader(_file);
                var doc = new PdfDocument(reader);

                var sb = new StringBuilder();
                for (var page = 1; page <= doc.GetNumberOfPages(); page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    var content = PdfTextExtractor.GetTextFromPage(doc.GetPage(page), strategy);
                    sb.Append(content);
                }

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