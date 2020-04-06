using System;
using System.IO;
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
            switch (file.Extension)
            {
                case ".txt":
                    reader = new TextReader(file);
                    return reader.ReadAll();
                case ".docx":
                    reader = new WordReader(file);
                    return reader.ReadAll();
                case ".xlsx":
                    reader = new ExcelReader(file);
                    return reader.ReadAll();
                case ".pptx":
                    reader = new PowerPointReader(file);
                    return reader.ReadAll();
                default:
                    return file.Extension == "" ? file.Name : file.Name.Replace(file.Extension, "");
            }
        }
    }
}