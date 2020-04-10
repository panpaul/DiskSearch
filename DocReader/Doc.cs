using System.IO;
using System.Text;
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
        private static string AddPinyin(string str) {
            var sb = new StringBuilder();
            sb.Append(str);
            sb.Append(" ");
            sb.Append(TinyPinyin.PinyinHelper.GetPinyin(str));
            sb.Append(" ");
            sb.Append(TinyPinyin.PinyinHelper.GetPinyinInitials(str));
            var result = sb.ToString();
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");
            return result;
        }
        public static string Read(FileInfo file)
        {
            IReader reader;
            var fileType = MimeTypeMap.GetMimeType(file.Extension);
            if (fileType.StartsWith("text/"))
            {
                reader = new TextReader(file);
                return AddPinyin(reader.ReadAll());
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.wordprocessingml"))
            {
                reader = new WordReader(file);
                return AddPinyin(reader.ReadAll());
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.spreadsheetml"))
            {
                reader = new ExcelReader(file);
                return AddPinyin(reader.ReadAll());
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.presentationml"))
            {
                reader = new PowerPointReader(file);
                return AddPinyin(reader.ReadAll());
            }

            return AddPinyin(file.Name.Replace(".", " "));
        }
    }
}