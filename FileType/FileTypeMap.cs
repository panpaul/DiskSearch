using System;
using System.Collections.Generic;

namespace FileType
{
    public static class FileTypeMap
    {
        public enum TypeCode
        {
            TypeText,
            TypeDocx,
            TypePptx,
            TypeXlsx,
            TypeImage,
            TypeUnsupported
        }

        private static readonly Lazy<IDictionary<string, TypeCode>> Mappings =
            new Lazy<IDictionary<string, TypeCode>>(BuildMappings);

        private static IDictionary<string, TypeCode> BuildMappings()
        {
            var source = new Dictionary<string, TypeCode>(StringComparer.OrdinalIgnoreCase)
            {
                {".txt", TypeCode.TypeText},
                {".md", TypeCode.TypeText},
                {".html", TypeCode.TypeText},
                {".htm", TypeCode.TypeText},
                {".js", TypeCode.TypeText},
                {".c", TypeCode.TypeText},
                {".cpp", TypeCode.TypeText},
                {".h", TypeCode.TypeText},
                {".hpp", TypeCode.TypeText},
                {".vue", TypeCode.TypeText},
                {".cs", TypeCode.TypeText},
                {".cu", TypeCode.TypeText},
                {".go", TypeCode.TypeText},
                {".v", TypeCode.TypeText},
                // DOCx
                {".docx", TypeCode.TypeDocx},
                {".docm", TypeCode.TypeDocx},
                {".dotx", TypeCode.TypeDocx},
                {".dotm", TypeCode.TypeDocx},
                // PPTx
                {".pptx", TypeCode.TypePptx},
                {".pptm", TypeCode.TypePptx},
                {".potx", TypeCode.TypePptx},
                {".potm", TypeCode.TypePptx},
                {".ppsx", TypeCode.TypePptx},
                {".ppsm", TypeCode.TypePptx},
                // XLSx
                {".xlsx", TypeCode.TypeXlsx},
                {".xlsm", TypeCode.TypeXlsx},
                {".xlsb", TypeCode.TypeXlsx},
                {".xltx", TypeCode.TypeXlsx},
                {".xltm", TypeCode.TypeXlsx},
                // Image
                {".jpg", TypeCode.TypeImage},
                {".jpeg", TypeCode.TypeImage},
                {".png", TypeCode.TypeImage},
                {".gif", TypeCode.TypeImage},
                {".bmp", TypeCode.TypeImage}
            };
            return source;
        }

        public static TypeCode GetType(string extension)
        {
            extension = extension.ToLower();
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            if (!extension.StartsWith("."))
                extension = "." + extension;
            return !Mappings.Value.TryGetValue(extension, out var type) ? TypeCode.TypeUnsupported : type;
        }
    }
}