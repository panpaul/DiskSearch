using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Database;
using FileType;
using Lucene.Net.Documents;
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
            sb.Append(' ');
            sb.Append(PinyinHelper.GetPinyinInitials(str));
            return CleanUpSpaces(sb.ToString());
        }

        public static Document Read(FileInfo file)
        {
            IReader reader;
            var nameContent = file.Name;
            var doc = Engine.GenerateDocument(
                file.FullName,
                CleanUpSpaces(nameContent),
                CleanUpSpaces(GetPinyin(nameContent)),
                "unsupported");

            var fileType = FileTypeMap.GetType(file.Extension);
            string tag;
            switch (fileType)
            {
                case FileTypeMap.TypeCode.TypeText:
                    tag = "text";
                    reader = new TextReader(file);
                    break;
                case FileTypeMap.TypeCode.TypeDocx:
                    tag = "word";
                    reader = new WordReader(file);
                    break;
                case FileTypeMap.TypeCode.TypePptx:
                    tag = "powerpoint";
                    reader = new PowerPointReader(file);
                    break;
                case FileTypeMap.TypeCode.TypeXlsx:
                    tag = "excel";
                    reader = new ExcelReader(file);
                    break;
                case FileTypeMap.TypeCode.TypeImage:
                    tag = "image";
                    reader = new ImageReader(file);
                    break;
                case FileTypeMap.TypeCode.TypePdf:
                    tag = "pdf";
                    reader = new PDFReader(file);
                    break;
                case FileTypeMap.TypeCode.TypeUnsupported:
                    return doc;
                default: return doc;
            }

            // In Database\Engine.cs we defined that the max length is 80
            var paddingBlank = new string(' ', 80);
            var content = reader.ReadAll();
            var pinyin = GetPinyin(content);
            content = nameContent + paddingBlank + CleanUpSpaces(content);
            content = CleanUpSpaces(content);
            pinyin = CleanUpSpaces(pinyin);

            doc = Engine.GenerateDocument(file.FullName, content, pinyin, tag);
            return doc;
        }
    }
}