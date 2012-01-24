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
            Console.WriteLine("UniversalEmoticonPackBuilder console utility");
            Console.WriteLine("© by artyfarty 2012");
            Console.WriteLine();
            Console.WriteLine("Usage: ");
            Console.WriteLine("uepackbuild [<path_to_config.json>]");
            Console.WriteLine();
            Console.WriteLine("uepackbuild will also look for config.json in current dir");
            Console.WriteLine("All pack files should be in the same dir as config.json");

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
                UniversalEmoticonPackBuilder.BuildPacks(Config);

                Console.WriteLine("Build finished.");
            }
            else {
                Console.WriteLine("Cannot open config file. Press any key to exit...");
                Console.ReadLine();
            }
        }
    }
}
