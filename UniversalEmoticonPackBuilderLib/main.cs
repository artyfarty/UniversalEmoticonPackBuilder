using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using UniversalEmoticonPackBuilderLib.Builders;

namespace UniversalEmoticonPackBuilderLib
{
    public static class UniversalEmoticonPackBuilder {
        public static void BuildPacks(PackInfo packinfo, string build_dir, List<string> requested_builders, Dictionary<string, string> map)
        {
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
            List<SmilePackBuilder> builders = new List<SmilePackBuilder>();
            foreach (var b in requested_builders)
                switch (b)
                {
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

            // Build!
            foreach (var s in map)
                builders.ForEach(c => c.CopySmiley(s.Key, s.Value.Split(',')));

            builders.ForEach(c => c.Finish());
        }

        public static void BuildPacks(PackInfo packinfo, string build_dir, List<string> requested_builders, string mapfile) {
            // Load pack
            Dictionary<string, string> map = new Dictionary<string, string>();

            if (mapfile.Length > 0)
            {
                string line;
                var mapStream = new StreamReader(mapfile);
                while ((line = mapStream.ReadLine()) != null)
                {
                    if (line.IndexOf('#') == 0) continue;
                    var spl = line.Split(new[] { ':' }, 2);
                    if (spl.Length < 2) continue;

                    var sname = spl[0];
                    var scode = spl[1];
                    map.Add(sname, scode);
                }
            }
            else
                throw new Exception("No map defined");

            BuildPacks(
                packinfo,
                build_dir,
                requested_builders,
                map
            );
        }

        public static void BuildPacks(BuilderConfig Config)
        {
            BuildPacks(
                new PackInfo() { name = Config.name, version = Config.version, author = Config.author },
                "build/",
                Config.builders,
                Config.map
            );
        }
    }

    [DataContract]
    public class BuilderConfig
    {
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

    // class that stores emoticon pack info
    public class PackInfo
    {
        public string name;
        public string version;
        public string author;

        public string FullName
        {
            get
            {
                return String.Format("{0} {1} by {2}", name, version, author);
            }
        }

        public string Description
        {
            get
            {
                return FullName;
            }
        }
    }
}
