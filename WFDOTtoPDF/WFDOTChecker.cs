﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace WFDOTtoPDF
{
    public class WFDOTChecker
    {
        public static void ShowWrongReference()
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

        public static void DublicatesWithoutIndex()
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
    }
}