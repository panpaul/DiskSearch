using System;
using System.IO;

namespace DocReader
{
    interface IReader
    {
        string ReadAll();
    }
    public class Doc
    {
        public static string Read(FileInfo file)
        {
            IReader reader;
            switch (file.Extension)
            {
                case ".txt":
                    reader=new TextReader(file);
                    return reader.ReadAll();
                default:
                    return file.Name;
            }
        }
    }
}
