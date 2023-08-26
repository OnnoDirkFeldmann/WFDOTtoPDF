using System;
using System.IO;
using System.Text;

namespace WFDOTtoPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string line;
            Console.WriteLine("1=html 2=html (full) 3=docx 4=docx (full) 5=dic 6=Duplikate ohne Index 7=Falsche Zuordnungen");
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
                    WFDOTToDocx.ToDocx(false, @"C:\Users\Neronno\Desktop\gen", false);
                    break;
                case "4":
                    WFDOTToDocx.ToDocx(true, @"C:\Users\Neronno\Desktop\gen", false);
                    break;
                case "5":
                    string dic;
                    dic = WFDOTToDic.Todic();
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\frs.dic", dic, Encoding.Unicode);
                    break;
                case "6":
                    WFDOTChecker.DublicatesWithoutIndex();
                    break;
                case "7":
                    WFDOTChecker.ShowWrongReference();
                    break;
            }
        }
    }
}

