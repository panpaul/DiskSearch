using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocReader
{
    class ExcelReader : IReader
    {
        private readonly FileInfo _file;

        public ExcelReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            return "";
        }
    }
}
