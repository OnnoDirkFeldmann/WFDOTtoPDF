using Microsoft.Office.Interop.Excel;
using System;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace WFDOTtoPDF
{
    internal class ExcelToWFDOT
    {
        public static void ExcelToSqlite(string excelPath, string sqlitepath)
        {
            Application xlApp = new Application();
            Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;
            SQLiteConnection connection = new SQLiteConnection();
            connection.ConnectionString = @"Data Source=" + sqlitepath;
            connection.Open();
            SQLiteCommand scdCommand = new SQLiteCommand("DELETE FROM WB;", connection);
            scdCommand.ExecuteNonQuery();
            //scdCommand = new SQLiteCommand("DROP TABLE WBFTS;", connection);
            //scdCommand.ExecuteNonQuery();

            //Erste Zeile sind die Captions
            var insertFront = "INSERT INTO WB (";
            for (int j = 2; j <= 50; j++)
            {
                var cellValue = (string)(xlWorksheet.Cells[1, j] as Microsoft.Office.Interop.Excel.Range).Value;
                insertFront += $"[{cellValue}], ";
            }
            insertFront = insertFront.Substring(0, insertFront.Length - 2);
            insertFront += ") ";

            for (int i = 2; i <= xlRange.Rows.Count; i++)
            {
                string insert = insertFront + "VALUES (";
                for (int j = 2; j <= 50; j++)
                {
                    var cell = xlWorksheet.Cells[i, j] as Microsoft.Office.Interop.Excel.Range;
                    var cellValue = "";
                    if (cell != null)
                    {
                        cellValue = (xlWorksheet.Cells[i, j] as Microsoft.Office.Interop.Excel.Range).Value.ToString();
                        if (j == 3 || j == 11 || j == 12 || j == 13)
                        {
                            if (cellValue.Length >= 2)
                            {
                                var leading = cellValue.Substring(0, 2);
                                //Quote hinzufügen
                                var match = Regex.Match(leading, ". ");
                                if (match.Success)
                                {
                                    cellValue = "'" + cellValue;
                                }
                            }

                            if (cellValue.Equals("k") || cellValue.Equals("n") || cellValue.Equals("s") || cellValue.Equals("t"))
                            {
                                cellValue = "'" + cellValue;
                            }
                        }

                    }
                    cellValue = cellValue.Replace("'", "''");
                    insert += $"'{cellValue}', ";
                }
                insert = insert.Substring(0, insert.Length - 2);
                insert += ");";
                scdCommand = new SQLiteCommand(insert, connection);
                scdCommand.ExecuteNonQuery();
                insert = "";
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            Marshal.ReleaseComObject(xlRange);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            Marshal.ReleaseComObject(xlWorksheet);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
            xlWorkbook.Close();
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            Marshal.ReleaseComObject(xlWorkbook);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
            xlApp.Quit();
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
            Marshal.ReleaseComObject(xlApp);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
            scdCommand = new SQLiteCommand("CREATE VIRTUAL TABLE WBFTS USING FTS4(ID, Ostfriesisch, Deutsch, Artikel, Wortart, Plural, Genus, Komparation, Konjugation, Nebenformen, Standardform, tokenize=unicode61);", connection);
            scdCommand.ExecuteNonQuery();
            scdCommand = new SQLiteCommand("Insert INTO WBFTS (ID, Ostfriesisch, Deutsch, Artikel, Wortart, Plural, Genus, Komparation, Konjugation, Nebenformen, Standardform) SELECT ID, Ostfriesisch, Deutsch, Artikel, Wortart, Plural, Genus, Komparation, Konjugation, Nebenformen, Standardform FROM WB; ", connection);
            scdCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
}
