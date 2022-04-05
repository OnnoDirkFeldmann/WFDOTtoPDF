using System;
using System.IO;
using System.Text;

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
                    html = WFDOTToHtml.Tohtml(false);
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\out.html", html);
                    break;
                case "2":
                    string htmlFull;
                    htmlFull = WFDOTToHtml.Tohtml(true);
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\outfull.html", htmlFull);
                    break;
                case "3":
                    string dic;
                    dic = WFDOTToDic.Todic();
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\frs.dic", dic, Encoding.Unicode);
                    break;
                case "4":
                    WFDOTChecker.DublicatesWithoutIndex();
                    break;
                case "5":
                    ExcelToWFDOT.ExcelToSqlite(@"C:\Users\Neronno\Desktop\convert\export.xlsm", @"C:\Users\Neronno\Desktop\convert\export.db");
                    break;
                case "6":
                    WFDOTChecker.ShowWrongReference();
                    break;
            }
        }
    }
}

