using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace WFDOTtoPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            line = Console.ReadLine();
            switch (line)
            {
                case "1":
                    string html;
                    html = Tohtml();
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\test.html", html);
                    break;
                case "2":

                    break;
            }

        }
        public static void Topdf(string html)
        {
            
        }
            public static string Tohtml()
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
                komparation = (string)reader["Komparation"];
                nebenformen = (string)reader["Nebenformen"];
                konjugation = (string)reader["Konjugation"];
                kommentar = (string)reader["Kommentar"];
                temp = ostfriesisch;
                if (artikel != "-" || plural != "-")
                {
                    temp += "<span style=\"font-family:Verdana; font-size:12pt\"> [</span>";
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
                if (artikel != "-" || plural != "-")
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
                /*
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
                */
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
                    ofrsprep.Value = ostfriesischewoerter[i] + "#" + indexewoerter[i];
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
            writestring = "<!DOCTYPE html><html><body>" + writestring + "</body></html>";
            connection.Close();
            return writestring;
        }
    }
}
