using System.IO;
using MimeTypes;
using Sentry;

namespace DocReader
{
    internal interface IReader
    {
        string ReadAll();
    }

    public static class Doc
    {
        static Doc()
        {
            SentrySdk.Init("https://e9bae2c6285e48ea814087d78c9a40f1@sentry.io/4202655");
        }

        public static string Read(FileInfo file)
        {
            IReader reader;
            var fileType = MimeTypeMap.GetMimeType(file.Extension);
            if (fileType.StartsWith("text/"))
            {
                reader = new TextReader(file);
                return reader.ReadAll();
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.wordprocessingml"))
            {
                reader = new WordReader(file);
                return reader.ReadAll();
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.spreadsheetml"))
            {
                reader = new ExcelReader(file);
                return reader.ReadAll();
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.presentationml"))
            {
                reader = new PowerPointReader(file);
                return reader.ReadAll();
            }

            return file.Name.Replace(".", " ");
        }
    }
}