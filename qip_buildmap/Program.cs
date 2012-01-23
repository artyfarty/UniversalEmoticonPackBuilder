using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace qip_buildmap
{
    class Program
    {
        static void Main(string[] args)
        {
            var defineFile = new StreamReader("_define.ini", Encoding.GetEncoding("windows-1251"));
            var map = new StreamWriter("map.txt", false, Encoding.UTF8);
            map.AutoFlush = true;
            var dir = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).Where(f => f.EndsWith(".gif") || f.EndsWith(".jpg"));

            foreach (var smile in dir) {
                var defline = defineFile.ReadLine();
                var smile_fname = smile.Split('\\').Last();
                map.WriteLine("{0}:{1}", smile_fname, defline);
                //map.Flush();
            }
        }
    }
}
