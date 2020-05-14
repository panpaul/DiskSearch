using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MimeTypes;
using Sentry;
using Sentry.Protocol;
using TinyPinyin;

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
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new User
                {
                    Id = MachineCode.MachineCode.GetMachineCode()
                };
            });
        }

        private static string CleanUpSpaces(string str)
        {
            str = str.Replace("_", " ");
            str = Regex.Replace(str, @"\s+", " ");
            return str;
        }

        public static string GetPinyin(string str)
        {
            var sb = new StringBuilder();
            sb.Append(PinyinHelper.GetPinyin(str));
            sb.Append(" ");
            sb.Append(PinyinHelper.GetPinyinInitials(str));
            return CleanUpSpaces(sb.ToString());
        }

        public static string Read(FileInfo file)
        {
            IReader reader;
            var pathContent = file.FullName.Replace(".", " ").Replace("\\", " ").Replace("/", " ").Replace(":", " ") +
                              " ";
            var fileType = MimeTypeMap.GetMimeType(file.Extension);
            if (fileType.StartsWith("text/"))
            {
                reader = new TextReader(file);
                return CleanUpSpaces(pathContent + reader.ReadAll());
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.wordprocessingml"))
            {
                reader = new WordReader(file);
                return CleanUpSpaces(pathContent + reader.ReadAll());
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.spreadsheetml"))
            {
                reader = new ExcelReader(file);
                return CleanUpSpaces(pathContent + reader.ReadAll());
            }

            if (fileType.StartsWith("application/vnd.openxmlformats-officedocument.presentationml"))
            {
                reader = new PowerPointReader(file);
                return CleanUpSpaces(pathContent + reader.ReadAll());
            }

            return CleanUpSpaces(pathContent);
        }
    }
}