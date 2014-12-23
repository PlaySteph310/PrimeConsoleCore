using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
//using System.Runtime.InteropServices;
//using System.Media.SoundPlayer;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Security.Permissions;

namespace PrimeConsoleCore
{
    class Program
    {
        static long zahl = 1;
        static long zähler = 2;
        static long anzahl = 0;
        static bool laufen = true;
        public static bool sound = true;
        public static bool visible = true;
        public static Dictionary<string, bool> config = new Dictionary<string, bool>();
        public static Dictionary<string, string> userconfig = new Dictionary<string, string>();
        static public string ParsePath(string path) //function by dr4yyee
        {
            var newPath = new StringBuilder();
            var folders = path.Split(Path.DirectorySeparatorChar);
            foreach (var folder in folders) newPath.Append((Regex.IsMatch(folder, "%.+%")) ? Environment.GetEnvironmentVariable(Regex.Match(folder, "(?:%)(.+)(?:%)").Groups[1].Value) : folder).Append((folders[folders.Length - 1] == folder) ? string.Empty : new string(Path.DirectorySeparatorChar, 1));
            return newPath.ToString();
        }
        static Thread pause = new Thread(new ThreadStart(stoppen));
        public static Thread berechnungthread = new Thread(new ThreadStart(berechnung));
        public static Thread tastethread = new Thread(new ThreadStart(taste));
        public static System.Media.SoundPlayer beep = new System.Media.SoundPlayer();
        public static void Main()
        {
            Console.Title = "PrimeConsoleCore - Version 0.0.4";
            if (File.Exists(Environment.ExpandEnvironmentVariables(ParsePath(@"%localappdata%\\PCC\config.json"))))
            {
                StreamReader configread = new StreamReader(ParsePath(@"%localappdata%\\PCC\config.json"));
                config = JsonConvert.DeserializeObject<Dictionary<string, bool>>(configread.ReadToEnd());
                configread.Close();
            }
            else
            {
                config.Add("sound", true);
                config.Add("prime_found", true);
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                if (Directory.Exists(Environment.ExpandEnvironmentVariables(ParsePath(@"%localappdata%\\PCC\"))))
                {

                }
                else
                {
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ParsePath(@"%localappdata%\\PCC\"));
                    Directory.CreateDirectory(filePath);
                }
                string filePath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ParsePath(@"%localappdata%\\PCC\config.json"));
                File.WriteAllText(filePath2, json);
            }
            Console.WriteLine("");
            Console.WriteLine("Sie können jederzeit mit der Taste \"S\" sich die Statistiken des Programmes angucken.\n");
            Console.WriteLine("Programm stoppen: P");
            Console.WriteLine("Hilfe anzeigen: H");
            Console.WriteLine("Einstellungen: E\n");
            Console.WriteLine("Drücken Sie eine beliebige Taste um mit der Berechnung zu beginnen.");
            WebClient userconfigdl = new WebClient();
            userconfigdl.DownloadFile("http://steph.cf/pcc/index.html", "userconfig.json");
            StreamReader userconfigread = new StreamReader("userconfig.json");
            userconfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(userconfigread.ReadToEnd());
            userconfigread.Close();
            File.Delete("userconfig.json");
            zahl = Convert.ToInt32(userconfig["anzahl"]);
            //zähler = ;
            beep.SoundLocation = "beep.wav";
            var auswahl = Console.ReadKey();
            switch (auswahl.Key)
            {
                case ConsoleKey.E:
                    einstellungen();
                    break;
            }

            Parallel.Invoke(() =>
                 {
                     berechnungthread.Start();
                 },
                 () =>
                 {
                     tastethread.Start();
                 }
                );
            //pause.Start();
        }
        public static void stoppen ()
        {
            berechnungthread.Suspend();
            Console.WriteLine("\nSie haben die Berechnung gestoppt! Drücken Sie eine Taste um die Berechnung fortzusetzen.");
            Console.WriteLine("Anzahl der entdeckten Primzahlen: "+anzahl);
            Console.ReadLine();
            berechnungthread.Resume();
            taste();
        }
        public static void berechnung ()
        {
            while (laufen == true)
            {
                if (zahl % zähler == 0 && zahl != 1)
                {
                    if (zahl == zähler)
                    {
                        if(config["prime_found"] == true)
                        {
                            anzahl++;
                            if (config["sound"] == true)
                            {
                                beep.Play();
                            }
                            Console.WriteLine("Primzahl entdeckt! (" + anzahl + "): " + zahl);
                        }
                        zahl++;
                        zähler = 2;
                    }
                    else
                    {
                        zahl++;
                        zähler = 2;
                    }
                }
                else
                {
                    zähler++;
                }
                if (zahl == 1)
                {
                    zahl++;
                    zähler = 2;
                }
            }
        }
        public static void taste()
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.P)
            {
                stoppen();
            }
            else if (key.Key == ConsoleKey.E)
            {
                berechnungthread.Suspend();
                einstellungen();
                berechnungthread.Resume();
                taste();
            }
            else if (key.Key == ConsoleKey.H)
            {
                //if (visible != false)
                //{
                //    visible = false;
                //}
                hilfe();
                taste();
                visible = true;
            }
            else
            {
                taste();
            }
        }
        public static void einstellungen()
        {
            Console.Clear();
            Console.WriteLine("Einstellungen:");
            Console.WriteLine("Was möchten Sie verändern?");
            Console.WriteLine("Tippen Sie die jeweilige Zahl für die jeweilige Einstellung.");
            Console.WriteLine("1 = Sound aktivieren/deaktivieren");
            Console.WriteLine("2 = \"Primzahl entdeckt\"-Meldung aktivieren/deaktiveren");
            Console.WriteLine("3 = Anzahl ab wann \"Primzahl entdeckt\" angezeigt werden soll");
            Console.Write(">> ");
            var key = Console.ReadKey();
            Console.WriteLine("\n");
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Tippen Sie \"True\" oder \"False\"");
                    config["sound"] = Convert.ToBoolean(Console.ReadLine());
                    //sound = Convert.ToBoolean(Console.ReadLine());
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("Tippen Sie \"True\" oder \"False\"");
                    config["prime_found"] = Convert.ToBoolean(Console.ReadLine());
                    //visible = Convert.ToBoolean(Console.ReadLine());
                    break;
                case ConsoleKey.D3:
                    Console.WriteLine("Geben Sie eine Anzahl an:");
                    break;
            }
            string json2 = JsonConvert.SerializeObject(config, Formatting.Indented);
            string filePath3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ParsePath(@"%localappdata%\\PCC\config.json"));
            File.WriteAllText(filePath3, json2);
            Console.WriteLine("Drücken Sie eine Taste um zurückzukehren.");
            Console.ReadLine();
            Console.Clear();
        }
        public static void hilfe()
        {
            Console.Clear();
            Console.WriteLine("Hilfe:");
            Console.WriteLine("Drücken Sie die jeweilige Taste auf ihrer Tastatur um die jeweiligen Befehle auszuführen.");
            Console.WriteLine("Einstellungen: E");
            Console.WriteLine("Berechnung stoppen/pausieren: P");
            Console.WriteLine("Hilfe anzeigen: H\n");
            Console.WriteLine("Drücken Sie eine Taste um zurückzukehren.");
            Console.ReadLine();
            Console.Clear();
        }
        /*public enum BeepType
        {
            SimpleBeep = -1,
            IconAsterisk = 0x00000040,
            IconExclamation = 0x00000030,
            IconHand = 0x00000010,
            IconQuestion = 0x00000020,
            Ok = 0x00000000,
        }
        [DllImport("user32.dll")]
        public static extern bool MessageBeep(BeepType beepType);
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int frequency, int duration);*/
    }
}
