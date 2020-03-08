using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocReader
{
    class PowerPointReader : IReader
    {
        private readonly FileInfo _file;

        public PowerPointReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            return "";
        }
    }
}
