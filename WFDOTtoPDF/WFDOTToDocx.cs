using Novacode;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace WFDOTtoPDF
{
    public class WFDOTToDocx
    {
        public static void ToDocx(bool fullversion, string fileName, bool testMode)
        {
            var doc = DocX.Create(fileName);

            var wordFormat = new Formatting();
            wordFormat.FontFamily = new Font("Verdana");
            wordFormat.Size = 12D;

            var wordFormatBold = new Formatting();
            wordFormatBold.FontFamily = new Font("Verdana");
            wordFormatBold.Size = 12D;
            wordFormatBold.Bold = true;

            var textFormat = new Formatting();
            textFormat.FontFamily = new Font("Verdana");
            textFormat.Size = 11D;

            var textFormatItalic = new Formatting();
            textFormatItalic.FontFamily = new Font("Verdana");
            textFormatItalic.Size = 11D;
            textFormatItalic.Italic = true;

            SQLiteConnection connection = new SQLiteConnection();
            connection.ConnectionString = @"Data Source=C:\Users\Neronno\source\repos\WFDOTtoPDF\WFDOTtoPDF\WFDOT.db";
            connection.Open();

            var sql1 = testMode ? "SELECT * FROM WB WHERE Wortart != 'Phrase' AND Ostfriesisch Like 'a%'" : "SELECT * FROM WB WHERE Wortart != 'Phrase'";
            string sqlCom = sql1;
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
            string wortart;
            string genus;
            string komparation;
            string nebenformen;
            string kommentar;
            string autorKommentar;
            string rezept;
            string interferenz;
            List<string> list = new List<string>();
            List<string> ostfriesischewoerter = new List<string>();
            List<string> indexewoerter = new List<string>();
            List<string> list2 = new List<string>();
            var i = 0;
            while (reader.Read())
            {
                var paragraph = doc.InsertParagraph("", false, wordFormat);
                ostfriesisch = (string)reader["Ostfriesisch"];
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
                index = (string)reader["Index"];

                if (i%100 == 0)
                {
                    Console.WriteLine(ostfriesisch);
                }
                i++;

                paragraph.InsertText(ostfriesisch, false, wordFormatBold);

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
                    paragraph.InsertText(" [", false, wordFormat);
                }

                if (artikel != "-")
                {
                    paragraph.InsertText(artikel, false, wordFormat);
                }

                if (wortartstring != "-")
                {
                    if (artikel != "-") paragraph.InsertText(", ", false, wordFormat);
                    paragraph.InsertText(wortartstring, false, wordFormat);
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
                    if (artikel != "-" || wortartstring != "-") paragraph.InsertText(", ", false, wordFormat);
                    paragraph.InsertText(genusstring, false, wordFormat);
                }

                if (plural != "-")
                {
                    if (artikel != "-" || wortartstring != "-" || plural != "-") paragraph.InsertText(", ", false, wordFormat);
                    paragraph.InsertText(plural, false, wordFormat);
                }

                if (artikel != "-" || genus != "-" || plural != "-" || (wortart != "-" && wortartstring != "-"))
                {
                    paragraph.InsertText("]", false, wordFormat);
                }

                paragraph.InsertText(" ", false, wordFormat);
                paragraph.InsertText(deutsch, false, wordFormat);

                if (nebenformen != "-")
                {
                    paragraph.InsertText(Environment.NewLine, false, wordFormat);
                    paragraph.InsertText($"[NF: {nebenformen}]", false, wordFormat);
                }
                if (standardform != "-")
                {
                    paragraph.InsertText(Environment.NewLine, false, wordFormat);
                    paragraph.InsertText($"[{standardform}]", false, wordFormat);
                }

                if (fullversion)
                {
                    if (komparation != "-")
                    {
                        komparation = komparation.Substring(0, komparation.Length - 5);
                        komparation = komparation.Replace("<br/>", "; ");
                        var table = doc.AddTable(1, 1);
                        table.Rows[0].Cells[0].Paragraphs.First().Append(komparation);
                        table.AutoFit = AutoFit.Window;
                        doc.InsertTable(table);
                    }
                    if (konjugation != "-")
                    {
                        konjugation = konjugation.Substring(0, konjugation.Length - 5);
                        konjugation = konjugation.Replace("<br/>", "; ");
                        var table = doc.AddTable(1, 1);
                        table.Rows[0].Cells[0].Paragraphs.First().Append(konjugation);
                        table.AutoFit = AutoFit.Window;
                        doc.InsertTable(table);
                    }
                    if (kommentar != "-")
                    {
                        var table = doc.AddTable(1, 1);
                        table.Rows[0].Cells[0].Paragraphs.First().Append(kommentar);
                        table.AutoFit = AutoFit.Window;
                        doc.InsertTable(table);
                    }
                    if (autorKommentar != "-")
                    {
                        var table = doc.AddTable(1, 1);
                        table.Rows[0].Cells[0].Paragraphs.First().Append(autorKommentar);
                        table.AutoFit = AutoFit.Window;
                        doc.InsertTable(table);
                    }
                    if (rezept != "-")
                    {
                        rezept = rezept.Replace("<br/>", " ");
                        var table = doc.AddTable(1, 1);
                        table.Rows[0].Cells[0].Paragraphs.First().Append(rezept);
                        table.AutoFit = AutoFit.Window;
                        doc.InsertTable(table);
                    }
                    if (interferenz != "-")
                    {
                        var table = doc.AddTable(1, 1);
                        table.Rows[0].Cells[0].Paragraphs.First().Append(interferenz);
                        table.AutoFit = AutoFit.Window;
                        doc.InsertTable(table);
                    }
                }
                // Phrasen anreichern
                SQLiteParameter ofrsprep = new SQLiteParameter("@ofrs");
                string sqlCom2 = "SELECT * FROM WB WHERE Wortart = 'Phrase' AND Zuordnung = @ofrs";
                SQLiteCommand scdCommand2 = new SQLiteCommand(sqlCom2, connection);
                if (index == "-")
                {
                    ofrsprep.Value = ostfriesisch;
                }
                else
                {
                    ofrsprep.Value = ostfriesisch + "=" + index;
                }
                scdCommand2.Parameters.Add(ofrsprep);
                scdCommand2.Prepare();
                SQLiteDataReader reader2 = scdCommand2.ExecuteReader();
                while (reader2.Read())
                {
                    var phraseParagraph = doc.InsertParagraph("", false, textFormat);
                    var ostfriesischPhrase = (string)reader2["Ostfriesisch"];
                    var deutschPhrase = (string)reader2["Deutsch"];
                    phraseParagraph.InsertText(ostfriesischPhrase, false, textFormat);
                    phraseParagraph.InsertText(" ", false, textFormat);
                    phraseParagraph.InsertText(deutschPhrase, false, textFormatItalic);
                }
                reader2.Close();
            }
            reader.Close();

            //Phrasen ohne Zuordnung
            doc.InsertParagraph(Environment.NewLine, false, textFormat);
            doc.InsertParagraph("Unzugeordnete Phrasen:", false, wordFormat);
            doc.InsertParagraph(Environment.NewLine, false, textFormat);
            string sqlCom3 = "SELECT * FROM WB WHERE Wortart = 'Phrase' AND Zuordnung = '-'";
            SQLiteCommand scdCommand3 = new SQLiteCommand(sqlCom3, connection);
            scdCommand3.Prepare();
            SQLiteDataReader reader3 = scdCommand3.ExecuteReader();
            while (reader3.Read())
            {
                var phraseParagraph = doc.InsertParagraph("", false, textFormat);
                var ostfriesischPhrase = (string)reader3["Ostfriesisch"];
                var deutschPhrase = (string)reader3["Deutsch"];
                phraseParagraph.InsertText(ostfriesischPhrase, false, textFormat);
                phraseParagraph.InsertText(" ", false, textFormat);
                phraseParagraph.InsertText(deutschPhrase, false, textFormatItalic);
            }
            reader3.Close();

            connection.Close();
            doc.Save();
        }
    }
}
