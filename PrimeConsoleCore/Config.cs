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
                checkkeys();
            }
            else
            {
                Program.config.Add("sound", "true");
                Program.config.Add("prime_found", "true");
                Program.config.Add("welcome_message", "true");
                Program.config.Add("version", Program.version);
                Program.config.Add("link", "http://stephchan.w4f.eu/");
                Program.config.Add("username", "");
                if (!Directory.Exists(Environment.ExpandEnvironmentVariables(Program.ParsePath(@"%localappdata%\\PCC\"))))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\")));
                }
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(Program.config, Formatting.Indented));
            }
        }
        public static void checkkeys()
        {
            if (!Program.config.ContainsKey("welcome_message"))
            {
                Program.config.Add("welcome_message", "true");
            }
            if (!Program.config.ContainsKey("sound"))
            {
                Program.config.Add("sound", "true");
            }
            if (!Program.config.ContainsKey("prime_found"))
            {
                Program.config.Add("prime_found", "true");
            }
            if (!Program.config.ContainsKey("link"))
            {
                Program.config.Add("link", "http://stephchan.w4f.eu/");
            }
            if (!Program.config.ContainsKey("version"))
            {
                Program.config.Add("version", Program.version);
            }
            if (!Program.config.ContainsKey("username"))
            {
                Program.config.Add("username", "");
            }
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(Program.config, Formatting.Indented));
        }
    }
}
