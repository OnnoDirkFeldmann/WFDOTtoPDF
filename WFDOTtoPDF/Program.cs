using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace WFDOTtoPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            Console.WriteLine("1=html 2=html (full) 3=dic 4=Tools 5=? 6=Falsche Zuordnungen");
            line = Console.ReadLine();
            switch (line)
            {
                case "1":
                    string html;
                    html = Tohtml(false);
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\out.html", html);
                    break;
                case "2":
                    string htmlFull;
                    htmlFull = Tohtml(true);
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\outfull.html", htmlFull);
                    break;
                case "3":
                    string dic;
                    dic = Todic();
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\frs.dic", dic, Encoding.Unicode);
                    break;
                case "4":
                    dublicatesWithoutIndex();
                    break;
                case "5":
                    ExcelToSqlite(@"C:\Users\Neronno\Desktop\convert\export.xlsm", @"C:\Users\Neronno\Desktop\convert\export.db");
                    break;
                case "6":
                    ShowWrongReference();
                    break;
            }

        }

        private static void ShowWrongReference()
        {
            SQLiteConnection connection = new SQLiteConnection();
            connection.ConnectionString = @"Data Source=C:\Users\Neronno\source\repos\WFDOTtoPDF\WFDOTtoPDF\WFDOT.db";
            connection.Open();
            string sqlCom = "SELECT * FROM WB WHERE Wortart = 'Phrase' AND Zuordnung != '-'";
            SQLiteCommand scdCommand = new SQLiteCommand(sqlCom, connection);
            SQLiteDataReader reader = scdCommand.ExecuteReader();
            var wordList = new Dictionary<long, string[]>();
            while (reader.Read())
            {
                wordList.Add((long)reader["ID"], new string[] { (string)reader["Ostfriesisch"], (string)reader["Zuordnung"] });
            }
            foreach (KeyValuePair<long, string[]> word in wordList)
            {
                var splitted = word.Value[1].Split('=');
                string sqlCom2 = "SELECT COUNT(*) FROM WB WHERE Ostfriesisch = @ofrs";
                SQLiteParameter ofrsprep = new SQLiteParameter("@ofrs");
                SQLiteParameter index = new SQLiteParameter("@index");
                ofrsprep.Value = splitted[0];
                if (splitted.Length == 2)
                {
                    sqlCom2 = "SELECT COUNT(*) FROM WB WHERE Ostfriesisch = @ofrs AND \"Index\" = @index";
                    index.Value = splitted[1];
                }
                SQLiteCommand scdCommand2 = new SQLiteCommand(sqlCom2, connection);
                scdCommand2.Parameters.Add(ofrsprep);
                scdCommand2.Parameters.Add(index);
                scdCommand2.Prepare();
                SQLiteDataReader reader2 = scdCommand2.ExecuteReader();
                while (reader2.Read())
                {
                    var count = reader2.GetInt64(0);
                    if (count != 1)
                    {
                        Console.WriteLine(word.Key + word.Value[0]);
                    }
                }
            }

            Console.WriteLine("---Done");
            Console.Read();
            reader.Close();
            connection.Close();
        }

        private static void dublicatesWithoutIndex()
        {
            SQLiteConnection connection = new SQLiteConnection();
            connection.ConnectionString = @"Data Source=C:\Users\Neronno\source\repos\WFDOTtoPDF\WFDOTtoPDF\WFDOT.db";
            connection.Open();
            string sqlCom = "SELECT * FROM WB WHERE INDEX = '-'";
            SQLiteCommand scdCommand = new SQLiteCommand(sqlCom, connection);
            SQLiteDataReader reader = scdCommand.ExecuteReader();
            var wordList = new List<string[]>();
            while (reader.Read())
            {
                wordList.Add(new string[] { (string)reader["Ostfriesisch"], (string)reader["Index"] });
            }

            Console.Read();
            reader.Close();
            connection.Close();
        }

        public static void Topdf(string html)
        {

        }
        public static string Tohtml(bool fullversion)
        {
            SQLiteConnection connection = new SQLiteConnection();
            connection.ConnectionString = @"Data Source=C:\Users\Neronno\source\repos\WFDOTtoPDF\WFDOTtoPDF\WFDOT.db";
            connection.Open();

            string sqlCom = "SELECT * FROM WB WHERE Wortart != 'Phrase'";
            SQLiteCommand scdCommand = new SQLiteCommand(sqlCom, connection);
            SQLiteDataReader reader = scdCommand.ExecuteReader();

            //Ostfriesische Begriffe holen
            string ostfriesisch;
            string standardform;
            string konjugation;
            string index;
            string deutsch;
            string artikel;
            string plural;
            string temp;
            string wortart;
            string genus;
            string komparation;
            string nebenformen;
            string writestring;
            string kommentar;
            string autorKommentar;
            string rezept;
            string interferenz;
            writestring = "";
            List<string> list = new List<string>();
            List<string> ostfriesischewoerter = new List<string>();
            List<string> indexewoerter = new List<string>();
            List<string> list2 = new List<string>();
            while (reader.Read())
            {
                ostfriesisch = (string)reader["Ostfriesisch"];
                ostfriesischewoerter.Add(ostfriesisch);
                ostfriesisch = "<span style=\"font-family:Verdana; font-size:12pt\"><b>" + ostfriesisch + "</b></span>";
                deutsch = (string)reader["Deutsch"];
                standardform = (string)reader["Standardform"];
                artikel = (string)reader["Artikel"];
                plural = (string)reader["Plural"];
                wortart = (string)reader["Wortart"];
                genus = (string)reader["Genus"];
                nebenformen = (string)reader["Nebenformen"];
                komparation = (string)reader["Komparation"];
                konjugation = (string)reader["Konjugation"];
                kommentar = (string)reader["Kommentar"];
                autorKommentar = (string)reader["Autorkommentar"];
                rezept = (string)reader["Rezept"];
                interferenz = (string)reader["Interferenz"];

                temp = ostfriesisch;

                string wortartstring = "Wortartfehler";
                if (wortart != "-")
                {
                    switch (wortart)
                    {
                        case "Abkürzung":
                            wortartstring = "-";
                            break;
                        case "Adjektiv":
                            wortartstring = "-";
                            break;
                        case "Adverb":
                            wortartstring = "-";
                            break;
                        case "Artikel":
                            wortartstring = "-";
                            break;
                        case "Ausruf":
                            wortartstring = "-";
                            break;
                        case "Flexionsform":
                            wortartstring = "-";
                            break;
                        case "Interrogativpronomen":
                            wortartstring = "-";
                            break;
                        case "Konjunktion":
                            wortartstring = "-";
                            break;
                        case "Nachsilbe":
                            wortartstring = "-";
                            break;
                        case "Name":
                            wortartstring = "-";
                            break;
                        case "Numeral":
                            wortartstring = "-";
                            break;
                        case "Ortsname":
                            wortartstring = "-";
                            break;
                        case "Partikel":
                            wortartstring = "-";
                            break;
                        case "Partizip":
                            wortartstring = "-";
                            break;
                        case "Phrase":
                            wortartstring = "-";
                            break;
                        case "Pronomen":
                            wortartstring = "-";
                            break;
                        case "Pronominaladverb":
                            wortartstring = "-";
                            break;
                        case "Substantiv":
                            wortartstring = "-";
                            break;
                        case "Verb":
                            wortartstring = "-";
                            break;
                        case "Vorsilbe":
                            wortartstring = "-";
                            break;
                        case "Zwischensilbe":
                            wortartstring = "-";
                            break;
                    }
                }

                

                if (artikel != "-" || genus != "-" || plural != "-" || (wortart != "-" && wortartstring != "-"))
                {
                    temp += "<span style=\"font-family:Verdana; font-size:12pt\"> [</span>";
                }

                if (wortartstring != "-")
                {
                    temp += "<span style=\"font-family:Verdana; font-size:12pt\">, " + wortartstring + "</span>";
                }
                

                if (artikel != "-")
                {
                    temp += "<span style=\"font-family:Verdana; font-size:12pt\">" + artikel + "</span>";
                }

                if (genus != "-")
                {
                    string genusstring = "Genusfehler";
                    switch (genus)
                    {
                        case "m":
                            genusstring = "m.";
                            break;
                        case "f":
                            genusstring = "f.";
                            break;
                        case "n":
                            genusstring = "n.";
                            break;
                    }
                    temp += "<span style=\"font-family:Verdana; font-size:12pt\">, " + genusstring + "</span>";
                }

                if (plural != "-")
                {
                    temp += "<span style=\"font-family:Verdana; font-size:12pt\">, " + plural + "</span>";
                }

                if (artikel != "-" || genus != "-" || plural != "-" || (wortart != "-" && wortartstring != "-"))
                {
                    temp += "<span style=\"font-family:Verdana; font-size:12pt\">]</span>";
                }

                temp += "<i><span style=\"font-family:Verdana; font-size:12pt\"> " + deutsch + "<br/></span></i>";
                if (nebenformen != "-")
                {
                    temp += "<span style=\"font-family:Verdana; font-size:11pt\">[NF: " + nebenformen + "]<br/></span>";
                }
                if (standardform != "-")
                {
                    temp += "<span style=\"font-family:Verdana; font-size:11pt\">[" + standardform + "]<br/></span>";
                }

                if (fullversion)
                {
                    if (komparation != "-")
                    {
                        temp += "<span style=\"font-family:Verdana; font-size:11pt\">" + komparation + "<br/></span>";
                    }
                    if (konjugation != "-")
                    {
                        temp += "<span style=\"font-family:Verdana; font-size:11pt\">" + konjugation + "<br/></span>";
                    }
                    if (kommentar != "-")
                    {
                        temp += "<div style=\"border:1px solid black;\"><span style=\"font-family:Verdana; font-size:11pt\">" + kommentar + "<br/></span></div>";
                    }
                    if (autorKommentar != "-")
                    {
                        temp += "<div style=\"border:1px solid black;\"><span style=\"font-family:Verdana; font-size:11pt\">" + autorKommentar + "<br/></span></div>";
                    }
                    if (rezept != "-")
                    {
                        temp += "<div style=\"border:1px solid black;\"><span style=\"font-family:Verdana; font-size:11pt\">" + rezept + "<br/></span></div>";
                    }
                    if (interferenz != "-")
                    {
                        temp += "<div style=\"border:1px solid black;\"><span style=\"font-family:Verdana; font-size:11pt\">" + interferenz + "<br/></span></div>";
                    }
                }

                index = (string)reader["Index"];
                indexewoerter.Add(index);
                list.Add(temp);
            }
            reader.Close();
            // Phrasen anreichern
            for (int i = 0; i < ostfriesischewoerter.Count; i++)
            {
                string final;
                final = list[i];
                SQLiteParameter ofrsprep = new SQLiteParameter("@ofrs");
                string sqlCom2 = "SELECT * FROM WB WHERE Wortart = 'Phrase' AND Zuordnung = @ofrs";
                SQLiteCommand scdCommand2 = new SQLiteCommand(sqlCom2, connection);
                if (indexewoerter[i] == "-")
                {
                    ofrsprep.Value = ostfriesischewoerter[i];
                }
                else
                {
                    ofrsprep.Value = ostfriesischewoerter[i] + "=" + indexewoerter[i];
                }
                scdCommand2.Parameters.Add(ofrsprep);
                scdCommand2.Prepare();
                SQLiteDataReader reader2 = scdCommand2.ExecuteReader();
                while (reader2.Read())
                {
                    ostfriesisch = (string)reader2["Ostfriesisch"];
                    deutsch = (string)reader2["Deutsch"];
                    deutsch = "<i>" + deutsch + "</i>";
                    temp = "<span style=\"font-family:Verdana; font-size:11pt\">" + ostfriesisch + " " + deutsch + "<br/></span>";
                    final = final + temp;
                }
                writestring = writestring + final;
                reader2.Close();
            }
            //Phrasen ohne Zuordnung
            string addition = "<span style=\"font-family:Verdana; font-size:11pt\"><br/><br/>Unzugeordnete Phrasen:<br/><br/><br/></span>";
            string sqlCom3 = "SELECT * FROM WB WHERE Wortart = 'Phrase' AND Zuordnung = '-'";
            SQLiteCommand scdCommand3 = new SQLiteCommand(sqlCom3, connection);
            scdCommand3.Prepare();
            SQLiteDataReader reader3 = scdCommand3.ExecuteReader();
            while (reader3.Read())
            {
                ostfriesisch = (string)reader3["Ostfriesisch"];
                deutsch = (string)reader3["Deutsch"];
                deutsch = "<i>" + deutsch + "</i>";
                temp = "<span style=\"font-family:Verdana; font-size:11pt\">" + ostfriesisch + " " + deutsch + "<br/></span>";
                addition = addition + temp;
            }
            reader3.Close();
            writestring = writestring + addition;
            writestring = "<!DOCTYPE html><html><body>" + writestring + "</body></html>";
            connection.Close();
            return writestring;
        }
        public static string Todic()
        {
            string dictstring;
            dictstring = "";
            SQLiteConnection connection = new SQLiteConnection();
            connection.ConnectionString = @"Data Source=C:\Users\Neronno\source\repos\WFDOTtoPDF\WFDOTtoPDF\WFDOT.db";
            connection.Open();
            string sqlCom = "SELECT * FROM WB";
            SQLiteCommand scdCommand = new SQLiteCommand(sqlCom, connection);
            SQLiteDataReader reader = scdCommand.ExecuteReader();
            List<string> dicw = new List<string>();
            while (reader.Read())
            {
                string ofrs;
                ofrs = (string)reader["Ostfriesisch"];
                ofrs = ofrs.Replace("!", " ");
                ofrs = ofrs.Replace("?", " ");
                ofrs = ofrs.Replace("\"", " ");
                ofrs = ofrs.Replace(" - ", " ");
                ofrs = ofrs.Replace("\n", " ");
                ofrs = ofrs.Replace("\r", " ");
                dicw.AddRange(ofrs.Split(" "));
                string plural;
                plural = (string)reader["Plural"];
                if (!plural.Equals("-"))
                {
                    plural = plural.Replace(",", " ");
                    plural = plural.Replace("\n", " ");
                    plural = plural.Replace("\r", " ");
                    dicw.AddRange(plural.Split(" "));
                }
                string komparation;
                komparation = (string)reader["Komparation"];
                if (!komparation.Equals("-"))
                {
                    komparation = komparation.Replace("stark<br/>Positiv: ", " ");
                    komparation = komparation.Replace("schwach<br/>Positiv: ", " ");
                    komparation = komparation.Replace("<br/>Komparativ (prädikativ): ", " ");
                    komparation = komparation.Replace("<br/>Komparativ (attributiv): ", " ");
                    komparation = komparation.Replace("<br/>Superlativ : ", " ");
                    komparation = komparation.Replace("<br/>Attributiv (bestimmter Artikel/unbestimmter Artikel m./f.): ", " ");
                    komparation = komparation.Replace("<br/>Attributiv (unbestimmter Artikel n.): ", " ");
                    komparation = komparation.Replace("<br/>Elativ: ", " ");
                    komparation = komparation.Replace("<br/>", " ");
                    komparation = komparation.Replace("/", " ");
                    komparation = komparation.Replace("\n", " ");
                    komparation = komparation.Replace("\r", " ");
                    dicw.AddRange(komparation.Split(" "));
                }
                string konjugation;
                konjugation = (string)reader["Konjugation"];
                if (!konjugation.Equals("-"))
                {
                    konjugation = konjugation.Replace("stark<br/>Inf.: ", " ");
                    konjugation = konjugation.Replace("schwach<br/>Inf.: ", " ");
                    konjugation = konjugation.Replace("stark (Praet.-Praes.)<br/>Inf.: ", " ");
                    konjugation = konjugation.Replace("<br/>Imp: ", " ");
                    konjugation = konjugation.Replace("<br/>Präs.: ", " ");
                    konjugation = konjugation.Replace("<br/>Prät.: ", " ");
                    konjugation = konjugation.Replace("<br/>Part.I: ", " ");
                    konjugation = konjugation.Replace("<br/>Part.II: ", " ");
                    konjugation = konjugation.Replace("<br/>", " ");
                    konjugation = konjugation.Replace(",", " ");
                    konjugation = konjugation.Replace("/", " ");
                    konjugation = konjugation.Replace("\n", " ");
                    konjugation = konjugation.Replace("\r", " ");
                    dicw.AddRange(konjugation.Split(" "));
                }
            }
            dicw = dicw.Distinct().ToList();
            dicw.Sort();
            for (int i = 0; i < dicw.Count; i++)
            {
                dictstring = dictstring + dicw[i] + "\r\n";
            }

            //alles Trimmen
            return dictstring;
        }

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
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            scdCommand = new SQLiteCommand("CREATE VIRTUAL TABLE WBFTS USING FTS4(ID, Ostfriesisch, Deutsch, Artikel, Wortart, Plural, Genus, Komparation, Konjugation, Nebenformen, Standardform, tokenize=unicode61);", connection);
            scdCommand.ExecuteNonQuery();
            scdCommand = new SQLiteCommand("Insert INTO WBFTS (ID, Ostfriesisch, Deutsch, Artikel, Wortart, Plural, Genus, Komparation, Konjugation, Nebenformen, Standardform) SELECT ID, Ostfriesisch, Deutsch, Artikel, Wortart, Plural, Genus, Komparation, Konjugation, Nebenformen, Standardform FROM WB; ", connection);
            scdCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
}

