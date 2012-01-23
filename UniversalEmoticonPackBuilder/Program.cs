using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace QIPSmileBuilder
{
    class Program
    {
        public static CMDArgs Config;

        static void Main(string[] args)
        {
            // Default config
            Config = new CMDArgs()
            {
                version = "1.0.0",
                author = "VA",
                name = "Untitled",
                map = "",
                builders = new List<string> {
                    "qip"
                }
            };

            // Load config json
            string cfg_file = args.Length > 0 ? args[0] : "config.json";

            if (File.Exists(cfg_file))
            {
                Config =
                (CMDArgs)
                new DataContractJsonSerializer(typeof(CMDArgs))
                    .ReadObject(
                        new StreamReader(cfg_file, Encoding.UTF8).BaseStream
                     );
                Directory.SetCurrentDirectory(new FileInfo(cfg_file).DirectoryName);
            }

            // Initialize
            var packinfo = new PackInfo() { name = Config.name, version = Config.version, author = Config.author };

            var build_dir = "build/";// +Directory.CreateDirectory("build/build_" + (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).Name + "/";
            

            // Cleanup and create build dir
            try
            {
                Directory.CreateDirectory(build_dir).Delete(true);
                Directory.CreateDirectory(build_dir);
            }
            catch (IOException)
            {

            }

            // Load builders
            List<SmileConfig> builders =  new List<SmileConfig>();
            foreach (var b in Config.builders)
                switch (b) {
                    case "qip":
                        builders.Add(new QipSmileConfig(build_dir, packinfo));
                        break;
                    case "pidgin":
                        builders.Add(new PidginSmileConfig(build_dir, packinfo));
                        break;
                    case "adium":
                        builders.Add(new AdiumSmileConfig(build_dir, packinfo));
                        break;
                    case "miranda":
                        builders.Add(new MirandaSmileConfig(build_dir, packinfo));
                        break;
                    case "wim":
                        builders.Add(new WIMSkin(build_dir, packinfo));
                        break;
                }


            // Load pack
            Dictionary<string, string> pack = new Dictionary<string, string>();

            if (Config.map.Length > 0)
            {
                string line;
                var map = new StreamReader(Config.map);
                while ((line = map.ReadLine()) != null)
                {
                    if (line.IndexOf('#') == 0) continue;
                    var spl = line.Split(new[] { ':' }, 2);
                    if (spl.Length < 2) continue;

                    var sname = spl[0];
                    var scode = spl[1];
                    pack.Add(sname, scode);
                }
            }
            else
                throw new Exception("No map defined");// pack = Config.Pack;

            // Build!
            foreach (var s in pack)
                builders.ForEach(c => c.CopySmiley(s.Key, s.Value.Split(',')));

            builders.ForEach(c => c.Finish());
        }
    }

    abstract class SmileConfig
    {
        protected string path;
        protected PackInfo pack;
        protected string client;
        
        public SmileConfig(PackInfo pack) {
            this.pack = pack;
        }

        public void CopySmiley(string orig_file, string[] equivs)
        {
            File.Copy(orig_file, path + RenameFile(orig_file), true);
            WriteEntry(RenameFile(orig_file), equivs);
        }

        public void Finish() {
            EndFile();
        }

        protected abstract void WriteEntry(string file, string[] equivs);
        protected abstract void EndFile();
        protected virtual string RenameFile(string fname)
        {
            return fname;
        }
    }

    class QipSmileConfig : SmileConfig {
        private StreamWriter defineFile;
        private int counter = 0;

        public QipSmileConfig(string path, PackInfo pack) : base(pack)
        {
            client = "QIP";

            this.path = path + String.Format("{0} {1} by {2} for {3}/", pack.name, pack.version, pack.author, client);
            Directory.CreateDirectory(this.path);

            defineFile = new StreamWriter(this.path + "_define.ini", false, Encoding.GetEncoding("windows-1251"));
            defineFile.AutoFlush = true;
        }

        protected override void WriteEntry(string file, string[] equivs)
        {
            defineFile.WriteLine(String.Join(",", equivs));
            counter++;
        }

        protected override void EndFile()
        {
            defineFile.Flush();
            var w = new StreamWriter(this.path + "_define_vis.ini", false, Encoding.GetEncoding("windows-1251"));
            w.Write("1-{0}", counter + 1);
            w.Flush();
        }

        protected override string RenameFile(string fname)
        {
            var ext = fname.Split('.').Last();
            return toBase26(counter, 2) + "." + ext;
        }

        static string toBase26(int x, int digits)
        {
            string base26Characters = "abcdefghijklmnopqrstuvwxyz";

            char[] result = new char[digits];
            for (int i = digits - 1; i >= 0; --i)
            {
                result[i] = base26Characters[x % base26Characters.Length];
                x /= base26Characters.Length;
            }
            return new string(result);
        }
    }

    class PidginSmileConfig : SmileConfig
    {
        private StreamWriter pidginTheme;

        public PidginSmileConfig(string path, PackInfo pack)
            : base(pack)
        {
            client = "Pidgin";

            this.path = path + String.Format("{0} {1} by {2} for {3}/{0}/", pack.name, pack.version, pack.author, client);
            Directory.CreateDirectory(this.path);

            pidginTheme = new StreamWriter(this.path + "theme", false, Encoding.GetEncoding("windows-1251"));
            pidginTheme.AutoFlush = true;
            pidginTheme.WriteLine("Name={0}", pack.name);
            pidginTheme.WriteLine("Description={0} {1} by {2}", pack.name, pack.version, pack.author);
            pidginTheme.WriteLine("Icon=ff.gif");
            pidginTheme.WriteLine("Author={0}", pack.author);
            pidginTheme.WriteLine("[default]");
        }

        protected override void WriteEntry(string file, string[] equivs)
        {
            pidginTheme.WriteLine("{0}\t{1}", file, String.Join("\t", equivs));
        }

        protected override void EndFile()
        {
            pidginTheme.Flush();
        }
    }

    class AdiumSmileConfig : SmileConfig {
        private XmlTextWriter plistWriter;

        public AdiumSmileConfig(string path, PackInfo pack)
            : base(pack)
        {
            client = "Adium";

            this.path = path + String.Format("{0} {1} by {2} for {3}/{0}.AdiumEmoticonSet/", pack.name, pack.version, pack.author, client);
            Directory.CreateDirectory(this.path);

            plistWriter = new XmlTextWriter(this.path + "Emoticons.plist", Encoding.UTF8); 

            plistWriter.Formatting = Formatting.Indented;
            plistWriter.WriteStartDocument();
            plistWriter.WriteDocType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);

            plistWriter.WriteStartElement("plist");
            plistWriter.WriteAttributeString("version", "1.0");
            plistWriter.WriteStartElement("dict");
            plistWriter.WriteElementString("key", "AdiumSetVersion");
            plistWriter.WriteElementString("integer", "1");
            plistWriter.WriteElementString("key", "Emoticons");
            plistWriter.WriteStartElement("dict");
        }

        protected override void WriteEntry(string file, string[] equivs)
        {
            plistWriter.WriteElementString("key", file);
            plistWriter.WriteStartElement("dict");
            plistWriter.WriteElementString("key", "Equivalents");
            plistWriter.WriteStartElement("array");
            foreach (var eq in equivs)
                plistWriter.WriteElementString("string", eq);
            plistWriter.WriteEndElement();
            plistWriter.WriteElementString("key", "Name");
            plistWriter.WriteElementString("string", equivs[0]);
            plistWriter.WriteEndElement();
        }

        protected override void EndFile()
        {
            plistWriter.WriteEndElement();
            plistWriter.WriteEndElement();
            plistWriter.WriteEndElement();
            plistWriter.WriteEndDocument();
            plistWriter.Flush();
        }
    }

    class MirandaSmileConfig : SmileConfig
    {
        private StreamWriter mirandaTheme;

        public MirandaSmileConfig(string path, PackInfo pack)
            : base(pack)
        {
            client = "Miranda";

            this.path = path + String.Format("{0} {1} by {2} for Miranda/Animated/", pack.name, pack.version, pack.author);
            Directory.CreateDirectory(this.path);

            mirandaTheme = new StreamWriter(this.path + pack.name + ".msl", false, Encoding.GetEncoding("utf-8"));
            mirandaTheme.AutoFlush = true;
            mirandaTheme.WriteLine("Name\t=\t{0}", pack.name);
            mirandaTheme.WriteLine("Author\t=\t{0}", pack.author);
            mirandaTheme.WriteLine("Date\t=\t{0}", DateTime.Now);
            mirandaTheme.WriteLine("Version\t=\t{0}", pack.version);
        }

        protected override void WriteEntry(string file, string[] equivs)
        {
            mirandaTheme.WriteLine("Smiley\t= \"{0}\", 0, \"{1}\"", file, String.Join("\", \"", equivs));
        }

        protected override void EndFile()
        {
            mirandaTheme.Flush();
        }
    }

    class WIMSkin : SmileConfig
    {
        private StreamWriter wimSkin;

        public WIMSkin(string path, PackInfo pack)
            : base(pack)
        {
            client = "WIM";

            this.path = path + String.Format("{0} {1} by {2} for WIM/PlurkSmilies/", pack.name, pack.version, pack.author);
            Directory.CreateDirectory(this.path);

            wimSkin = new StreamWriter(this.path + pack.name + ".lua", false, Encoding.GetEncoding("utf-8"));
            wimSkin.AutoFlush = true;
            wimSkin.WriteLine("WIM_ClassicSkin.emoticons.definitions = {");

            this.path += "Emoticons/";
            Directory.CreateDirectory(this.path);
        }

        protected override void WriteEntry(string file, string[] equivs)
        {
            wimSkin.WriteLine("[\"{0}\"] = \"Interface\\\\AddOns\\\\PlurkSmilies\\\\Emoticons\\\\{1}\",", equivs[0], file);
        }

        protected override void EndFile()
        {
            wimSkin.WriteLine("};");
            wimSkin.Flush();
        }
    }


    class PackInfo {
        public string name;
        public string version;
        public string author;
    }

    [DataContract]
    class CMDArgs {
        [DataMember]
        public string name;
        [DataMember]
        public string version;
        [DataMember]
        public string author;
        [DataMember]
        public string map;
        [DataMember]
        public List<string> builders;
    }
}
