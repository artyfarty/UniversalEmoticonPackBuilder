using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Ionic.Zip;

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
                    case "gmail":
                        builders.Add(new GMailUserjs(build_dir, packinfo));
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

    // class that stores emoticon pack info
    class PackInfo {
        public string name;
        public string version;
        public string author;

        public string FullName {
            get {
                return String.Format("{0} {1} by {2}", name, version, author);
            }
        }

        public string Description {
            get {
                return FullName;
            }
        }
    }

    // Class for config json deserialization
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
