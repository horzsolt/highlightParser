using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace comsoleApp
{
    public class Program
    {
        static string filePath = "g:\\My Clippings.txt";
        static string outputDirectory = "g:\\kindleoutput";
        static List<string> words = new List<string> {"table", "figure"};

        public static void Main(string[] args)
        {
            IList<Highlight> list = new List<Highlight>();
            using (StreamReader reader = File.OpenText(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string titleLine = reader.ReadLine();
                    string[] _location = reader.ReadLine().Split('|')[0].Split('-');// - Your Highlight on Location 2190-2192 | Added on Sunday, September 13, 2015 4:38:32 PM

                    reader.ReadLine(); //always empty
                    string selectedTextLine = reader.ReadLine();

                    reader.ReadLine(); // separator line

                    Highlight hl = new Highlight();
                    hl.Title = titleLine;
                    hl.LocationStart = new string(_location[0].Where(c => char.IsDigit(c)).ToArray());
                    hl.LocationEnd = _location[1];
                    hl.SelectedText = selectedTextLine;
                    hl.FileName = hl.Title.Replace(':', '_').Replace('.', '_');
                    if (hl.FileName.Length > 50)
                    {
                        hl.FileName = hl.FileName.Substring(0, 50);
                    }

                    list.Add(hl);
                }
            }

            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }
            Directory.CreateDirectory(outputDirectory);
            TextWriter writer = null;
            string prev = "";
            string current = "";
            int counter = 1;

            foreach (Highlight hl in list)
            {
                current = hl.Title;
                if (prev == "" || prev != current)
                {
                    if (writer != null)
                    {
                        writer.WriteLine("</html>");
                        writer.Flush();
                        writer.Dispose();
                    }

                    if (File.Exists(outputDirectory + "\\" + hl.FileName + ".html"))
                    {
                        writer = File.AppendText(outputDirectory + "\\" + hl.FileName + ".html");
                    }
                    else
                    {
                        writer = File.CreateText(outputDirectory + "\\" + hl.FileName + ".html");
                        writer.WriteLine("<html>");
                        writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                        writer.WriteLine("<h1>" + hl.Title + "</h1>");
                    }

                    prev = current;
                    ++counter;
                }
                
                if ((hl.SelectedText.ToLower().IndexOf("chapter") > -1) && (hl.SelectedText.ToLower().IndexOf("chapter") < 5))
                {
                    writer.WriteLine("<h2>" + hl.SelectedText + "</h2>");
                }
                else if (words.Any(w => hl.SelectedText.Contains(w)))
                {
                    writer.WriteLine("<hr>");
                    writer.WriteLine("<h2>Image placeholder</h2>");
                    writer.WriteLine("<hr>");
                }
                else if (hl.SelectedText.Length < 40)
                {
                    writer.WriteLine("<h3>" + hl.SelectedText + "</h3>");
                }
                else
                {
                    writer.WriteLine("<span>" + hl.SelectedText + "</span>");
                    byte[] asciiBytes = Encoding.UTF8.GetBytes(hl.SelectedText);
                    writer.WriteLine("<span></span>");
                    writer.WriteLine("<span></span>");
                }

            }

            if (writer != null)
            {
                writer.WriteLine("</html>");
                writer.Flush();
                writer.Dispose();
            }
        }
    }
}
