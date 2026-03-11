using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MAB_Mailer
{
    public class ExcelService
    {
        public void CreateMasterTemplate(string filePath, int extraColumnCount)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AnaListe");

                worksheet.Cell(1, 1).Value = "Ad";
                worksheet.Cell(1, 2).Value = "Soyad";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Grup";

                for (int i = 0; i < extraColumnCount; i++)
                {
                    worksheet.Cell(1, 5 + i).Value = $"Veri {i + 1}";
                }

                var totalCols = 4 + extraColumnCount;
                var rngTable = worksheet.Range(1, 1, 1, totalCols);
                var table = rngTable.CreateTable("MAB_AnaTablo");

                table.Theme = XLTableTheme.TableStyleMedium2;

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(filePath);
            }
        }

        public List<Recipient> LoadRecipientsFromExcel(string filePath)
        {
            var list = new List<Recipient>();
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        try
                        {
                            var r = new Recipient();
                            r.Ad = row.Cell(1).GetValue<string>();
                            r.Soyad = row.Cell(2).GetValue<string>();
                            r.Email = row.Cell(3).GetValue<string>();

                            if (row.CellCount() >= 4)
                                r.GroupName = row.Cell(4).GetValue<string>();

                            if (!string.IsNullOrWhiteSpace(r.Email))
                            {
                                r.DataFields = new Dictionary<string, string>();
                                int lastCol = row.LastCellUsed().Address.ColumnNumber;

                                for (int i = 5; i <= lastCol; i++)
                                {
                                    string header = worksheet.Cell(1, i).GetValue<string>();
                                    string val = row.Cell(i).GetValue<string>();

                                    if (!string.IsNullOrEmpty(header) && !string.IsNullOrEmpty(val))
                                    {
                                        r.DataFields[header] = val;
                                    }
                                }
                                list.Add(r);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception) { throw; }
            return list;
        }
    }
}