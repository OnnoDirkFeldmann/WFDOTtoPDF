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
            string index;
            string deutsch;
            string temp;
            string writestring;
            writestring = "";
            List<string> list = new List<string>();
            List<string> ostfriesischewoerter = new List<string>();
            List<string> indexewoerter = new List<string>();
            List<string> list2 = new List<string>();
            while (reader.Read())
            {
                ostfriesisch = (string)reader["Ostfriesisch"];
                ostfriesischewoerter.Add(ostfriesisch);
                ostfriesisch = "<b>" + ostfriesisch + "</b>";
                deutsch = (string)reader["Deutsch"];
                temp = ostfriesisch + " " + deutsch + "<br/>";
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
                    temp = ostfriesisch + " " + deutsch + "<br/>";
                    final = final + temp;
                }
                writestring = writestring + final;
                reader2.Close();
            }

            connection.Close();
            return writestring;
        }
    }
}
