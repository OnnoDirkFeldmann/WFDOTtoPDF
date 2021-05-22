using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace WFDOTtoPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection connection = new SQLiteConnection();
            connection.ConnectionString = @"Data Source=C:\Users\Neronno\source\repos\WFDOTtoPDF\WFDOTtoPDF\WFDOT.db";
            connection.Open();

            string sqlCom = "SELECT * FROM WB WHERE Wortart != 'Phrase'";
            SQLiteCommand scdCommand = new SQLiteCommand(sqlCom, connection);
            SQLiteDataReader reader = scdCommand.ExecuteReader();

            //Ostfriesische Begriffe holen
            string ostfriesisch;
            string deutsch; 
            string temp;
            string writestring;
            writestring = "";
            List<string> list = new List<string>();
            List<string> ofrs = new List<string>();
            List<string> list2 = new List<string>();
            while (reader.Read())
            {
                ostfriesisch = (string)reader["Ostfriesisch"];
                ofrs.Add(ostfriesisch);
                ostfriesisch = "<b>" + ostfriesisch + "</b>";
                deutsch = (string)reader["Deutsch"];
                temp = ostfriesisch + " " + deutsch + "<br/>";
                list.Add(temp);
            }
            reader.Close();
            // Phrasen anreichern
            for (int i = 0; i < ofrs.Count; i++)
            {
                string final;
                final = list[i];
                SQLiteParameter ofrsprep = new SQLiteParameter("@ofrs");
                string sqlCom2 = "SELECT * FROM WB WHERE Wortart = 'Phrase' AND Zuordnung = @ofrs";
                SQLiteCommand scdCommand2 = new SQLiteCommand(sqlCom2, connection);
                ofrsprep.Value = ofrs[i];
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
            File.WriteAllText(@"C:\Users\Neronno\Desktop\test.html", writestring);
        }
    }
}
