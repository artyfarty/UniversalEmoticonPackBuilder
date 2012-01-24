using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using UniversalEmoticonPackBuilderLib;
using UniversalEmoticonPackBuilderLib.Builders;

namespace ConsoleUniversalEmoticonPackBuilder
{
    class Program
    {
        public static BuilderConfig Config;

        static void Main(string[] args)
        {
            // Default config
            Config = new BuilderConfig()
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
                (BuilderConfig)
                new DataContractJsonSerializer(typeof(BuilderConfig))
                    .ReadObject(
                        new StreamReader(cfg_file, Encoding.UTF8).BaseStream
                     );
                Directory.SetCurrentDirectory(new FileInfo(cfg_file).DirectoryName);
            }

            BuildPacks(Config);
        }

        private static void BuildPacks(BuilderConfig Config)
        {
            throw new NotImplementedException();
        }
    }
}
