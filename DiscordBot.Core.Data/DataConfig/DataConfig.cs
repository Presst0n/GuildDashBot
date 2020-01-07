using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot.Core.Data.Config
{
    public class DataConfig
    {
        private const string configFolder = "DataConfig/Files"; // For local usage: "../DiscordBot.Core.Data/DataConfig/Files"
        private const string configFile = "settings.json";
        public static Config dataConfig;

        static DataConfig()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            if (!File.Exists($"{configFolder}/{configFile}"))
            {
                dataConfig = new Config();
                string json = JsonConvert.SerializeObject(dataConfig, Formatting.Indented);
                File.WriteAllText($"{configFolder}/{configFile}", json);
            }
            else
            {
                string json = File.ReadAllText($"{configFolder}/{configFile}");
                dataConfig = JsonConvert.DeserializeObject<Config>(json);
            }
        }

        public struct Config
        {
            public string connectionString;
        }

    }
}

