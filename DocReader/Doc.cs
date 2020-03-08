using System.IO;

namespace DocReader
{
    internal interface IReader
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
                    reader = new TextReader(file);
                    return reader.ReadAll();
                case ".docx":
                    reader = new WordReader(file);
                    return reader.ReadAll();
                default:
                    return file.Name.Replace(file.Extension,"");
            }
        }
    }
}