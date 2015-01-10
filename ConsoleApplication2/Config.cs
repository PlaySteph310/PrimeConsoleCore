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
            if (File.Exists(Environment.ExpandEnvironmentVariables(Program.ParsePath(@"%localappdata%\\PCC\config.json"))))
            {
                StreamReader configread = new StreamReader(Program.ParsePath(@"%localappdata%\\PCC\config.json"));
                Program.config = JsonConvert.DeserializeObject<Dictionary<string, string>>(configread.ReadToEnd());
                configread.Close();
                Program.config["version"] = Program.version;
                checkkeys();
            }
            else
            {
                Program.config.Add("sound", "true");
                Program.config.Add("prime_found", "true");
                Program.config.Add("welcome_message", "true");
                Program.config.Add("version", Program.version);
                if (Directory.Exists(Environment.ExpandEnvironmentVariables(Program.ParsePath(@"%localappdata%\\PCC\"))))
                {

                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\")));
                }
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(Program.config, Formatting.Indented));
            }
        }
        public static void checkkeys()
        {
            if (Program.config.ContainsKey("welcome_message"))
            {
            }
            else
            {
                Program.config.Add("welcome_message", "true");
            }
            if (Program.config.ContainsKey("sound"))
            {
            }
            else
            {
                Program.config.Add("sound", "true");
            }
            if (Program.config.ContainsKey("prime_found"))
            {
            }
            else
            {
                Program.config.Add("prime_found", "true");
            }
            if (Program.config.ContainsKey("version"))
            {
            }
            else
            {
                Program.config.Add("version", Program.version);
            }
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(Program.config, Formatting.Indented));
        }
    }
}
