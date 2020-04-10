using System;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sentry;

namespace DocReader
{
    internal class ExcelReader : IReader
    {
        private readonly FileInfo _file;

        public ExcelReader(FileInfo file)
        {
            _file = file;
        }

        public string ReadAll()
        {
            try
            {
                var sb = new StringBuilder();
                var ss = SpreadsheetDocument.Open(_file.FullName, false);

                var wbPart = ss.WorkbookPart;
                var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                foreach (var workSheetPart in wbPart.WorksheetParts)
                {
                    var reader = OpenXmlReader.Create(workSheetPart);
                    while (reader.Read())
                    {
                        if (reader.ElementType == typeof(Worksheet)) reader.ReadFirstChild();
                        if (reader.ElementType != typeof(Row)) continue;

                        var row = (Row) reader.LoadCurrentElement();
                        foreach (var cell in row.Elements<Cell>())
                        {
                            if (cell.CellReference == null || !cell.CellReference.HasValue || cell.DataType == null)
                                continue;
                            var value = cell.InnerText;
                            switch (cell.DataType.Value)
                            {
                                case CellValues.SharedString:
                                    if (stringTable != null)
                                        value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                                    break;

                                case CellValues.Boolean:
                                    value = value switch
                                    {
                                        "0" => "FALSE",
                                        _ => "TRUE"
                                    };
                                    break;
                                case CellValues.Error:
                                    continue;
                            }

                            sb.Append(value);
                            sb.Append(" ");
                        }
                    }

                    reader.Close();
                }

                ss.Close();
                return sb.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SentrySdk.CaptureException(e);
                return _file.Extension == "" ? _file.Name : _file.Name.Replace(_file.Extension, "");
            }
        }
    }
}