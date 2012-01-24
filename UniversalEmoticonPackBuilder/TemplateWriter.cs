using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace QIPSmileBuilder
{
    class TemplateWriter
    {
        private string outfile;
        private Dictionary<string, string> replaces;
        private string tpl_file;
        public string data;

        public TemplateWriter(string name, string outfile, PackInfo pack)
        {
            this.tpl_file = name;
            this.outfile = outfile;

            replaces = new Dictionary<string, string>
            {
                {"__PACKNAME__", pack.name},
                {"__PACKAUTHOR__", pack.author},
                {"__PACKVERSION__", pack.version}
            };
        }

        public void AddReplacement(string key, string value) {
            replaces.Add(key, value);
        }

        public void Write(string s) {
            data += s;
        }

        public void WriteLine(string l)
        {
            Write(String.Format("{0}\n", l));
        }

        public void Flush() {
            var tpl = new StreamReader(String.Format("{0}/templates/{1}", System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), tpl_file));
            var o = new StreamWriter(outfile);
            o.AutoFlush = true;

            string l;
            while ((l = tpl.ReadLine()) != null)
            {
                foreach (var kv in replaces)
                    l = l.Replace(kv.Key, kv.Value);
                l = l.Replace("__PACKDATA__", data);
                o.WriteLine(l);
            }

            tpl.Close();
            o.Flush();
            o.Close();
        }
    }
}
