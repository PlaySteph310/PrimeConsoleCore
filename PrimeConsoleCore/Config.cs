using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PrimeConsoleCore
{
    public class Config
    {
        public static void checkfiles()
        {
            var checkconfig = new Dictionary<string, string>();
            checkconfig.Add("sound", "true");
            checkconfig.Add("prime_found", "true");
            checkconfig.Add("welcome_message", "true");
            checkconfig.Add("version", Program.version);
            checkconfig.Add("link", "http://stephchan.w4f.eu/");
            checkconfig.Add("username", "");
            if (File.Exists(Environment.ExpandEnvironmentVariables(Program.ParsePath(@"%localappdata%\\PCC\config.json"))))
            {
                StreamReader configread = new StreamReader(Program.ParsePath(@"%localappdata%\\PCC\config.json"));
                Program.config = JsonConvert.DeserializeObject<Dictionary<string, string>>(configread.ReadToEnd());
                configread.Close();
                checkkeys(checkconfig);
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(Program.config, Formatting.Indented));
            }
            else
            {
                checkkeys(checkconfig);
                if (!Directory.Exists(Environment.ExpandEnvironmentVariables(Program.ParsePath(@"%localappdata%\\PCC\"))))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\")));
                }
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(Program.config, Formatting.Indented));
            }
        }
        public static void checkkeys(Dictionary<string, string> fill)
        {
            foreach(var item in fill)
            {
                if(!Program.config.ContainsKey(item.Key))
                {
                    Program.config.Add(item.Key, item.Value);
                }
            }
            if(Program.config["version"] != Program.version)
            {
                Program.config["version"] = Program.version;
            }
        }
    }
}
