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
            Console.WriteLine("1=html 2=html (full) 3=docx 4=docx-full testpage 5=docx-full 6=dic 7=counterpart without index 8=wrong assignments");
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
                    WFDOTToDocx.ToDocx(true, @"C:\Users\Neronno\Desktop\gen", true);
                    break;
                case "5":
                    WFDOTToDocx.ToDocx(true, @"C:\Users\Neronno\Desktop\gen", false);
                    break;
                case "6":
                    string dic;
                    dic = WFDOTToDic.Todic();
                    File.WriteAllText(@"C:\Users\Neronno\Desktop\frs.dic", dic, Encoding.Unicode);
                    break;
                case "7":
                    WFDOTChecker.DublicatesWithoutIndex();
                    break;
                case "8":
                    WFDOTChecker.ShowWrongReference();
                    break;
            }
        }
    }
}

