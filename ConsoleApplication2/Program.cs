using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Security.Permissions;
using System.Numerics;
using System.Security.Cryptography;
using System.Net.Http;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PrimeConsoleCore
{
    public class Program
    {
        /* Variables */
        // Numbers
        public static BigInteger zahl = 1;
        //public static BigInteger zähler = 2;
        public static BigInteger anzahl = 0;
        public static BigInteger primzahl = 1;
        public static int threads = 0;
        // Text
        public static string user = "";
        public static string pass = "";
        public static string token = "";
        public static string resultToken = "";
        public static string version = "0.0.8.1";
        // Bools
        public static bool visible = true;

        /* Dictionarys */
        public static Dictionary<string, string> config = new Dictionary<string, string>();
        public static Dictionary<string, string> userconfig = new Dictionary<string, string>();

        /* Threads */
        public static Thread pause = new Thread(new ThreadStart(stoppen));
        //public static Thread berechnungthread = new Thread(new ThreadStart(berechnung));
        public static Thread tastethread = new Thread(new ThreadStart(taste));
        public static Thread send = new Thread(new ThreadStart(SendPrimes));
        public static Thread[] bthreads = new Thread[Environment.ProcessorCount];

        /* %directorypath% by Jonathan */
        public static string ParsePath(string path) //function by dr4yyee
        {
            var newPath = new StringBuilder();
            var folders = path.Split(Path.DirectorySeparatorChar);
            foreach (var folder in folders) newPath.Append((Regex.IsMatch(folder, "%.+%")) ? Environment.GetEnvironmentVariable(Regex.Match(folder, "(?:%)(.+)(?:%)").Groups[1].Value) : folder).Append((folders[folders.Length - 1] == folder) ? string.Empty : new string(Path.DirectorySeparatorChar, 1));
            return newPath.ToString();
        }

        /* Beepsound */
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int Frequenz, int Dauer);
        public static void SendPrimes()
        {
            while (true)
            {
                Dictionary<string, string> sendprimes = new Dictionary<string, string>();
                string result = "";
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["username"] = Program.user;
                    values["password"] = Program.pass;
                    values["token"] = Program.token;
                    values["prime"] = Convert.ToString(Program.zahl);
                    result = Encoding.Default.GetString(client.UploadValues("http://prime.steph.ml/sendprime.php", values)); ;
                }
                result = result.Replace("&quot;", "\"");
                sendprimes = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                if (sendprimes["send"] == "true")
                {
                    Console.WriteLine(" >> Primes were send to the server.");
                }
                else if (sendprimes["send"] == "false")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" >> Primes were not send to the server!");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.WriteLine(" >> Primes were not send to the server!");
                }
                System.Threading.Thread.Sleep(120000);
            }

        }

        /* Main*/
        public static void Main()
        {
            Console.SetWindowPosition(0, 0);
            Config.checkfiles();
            Console.Title = "PrimeConsoleCore - Version "+ config["version"];
            if (config["welcome_message"] == "true")
            {
                Console.WriteLine("\n\n\n\n\n\n\n");
                Console.WriteLine("                   _    _      _                          _ ");
                Thread.Sleep(100);
                Console.WriteLine("                  | |  | |    | |                        | |");
                Thread.Sleep(100);
                Console.WriteLine("                  | |  | | ___| | ___ ___  _ __ ___   ___| |");
                Thread.Sleep(100);
                Console.WriteLine("                  | |/\\| |/ _ \\ |/ __/ _ \\| '_ ` _ \\ / _ \\ |");
                Thread.Sleep(100);
                Console.WriteLine("                  \\  /\\  /  __/ | (_| (_) | | | | | |  __/_|");
                Thread.Sleep(100);
                Console.WriteLine("                   \\/  \\/ \\___|_|\\___\\___/|_| |_| |_|\\___(_)");
                Thread.Sleep(1000);
                Console.Clear();
            }
            Login.login();
            token = userconfig["token"];
            threads = Environment.ProcessorCount;
            for (int i = 0; i < threads; i++ )
            {
                bthreads[i] = new Thread(new ThreadStart(berechnung));
            }
            Parallel.Invoke(() =>
                 {
                     //berechnungthread.Start();
                     for (int i = 0; i < threads; i++)
                     {
                         bthreads[i].Start();
                     }
                 },
                 () =>
                 {
                     tastethread.Start();
                 },
                 () =>
                 {
                     send.Start();
                 }
                );
            //pause.Start();
        }
        public static void stoppen ()
        {
            for (int i = 0; i < threads; i++)
            {
                bthreads[i].Suspend();
            }
            Console.WriteLine("");
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║  You stopped the calculation. Press a button to resume the calculation.  ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("");
            Console.WriteLine(" >> Number of founded primes: "+anzahl);
            Console.WriteLine(" >> Current prime: " + zahl);
            Console.ReadLine();
            for (int i = 0; i < threads; i++)
            {
                bthreads[i].Resume();
            }
            taste();
        }
        public static void berechnung ()
        {
            //BigInteger zahl = 1;
            BigInteger zähler = 2;
            while (true)
            {
                if (zahl % zähler == 0 && zahl != 1)
                {
                    if (zahl == zähler)
                    {
                        anzahl++;
                        if(config["prime_found"] == "true" && visible == true)
                        {
                            Console.WriteLine(" >> Prime found!! (" + anzahl + "): " + zahl);
                            if (config["sound"] == "true")
                            {
                                //Beep(1000, 500);
                            }
                        }
                        primzahl++;
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
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.P:
                    Console.Write("\b \b");
                    stoppen();
                    break;
                case ConsoleKey.E:
                    visible = false;
                    einstellungen();
                    visible = true;
                    taste();
                    break;
                case ConsoleKey.H:
                    visible = false;
                    hilfe();
                    visible = true;
                    taste();
                    break;
                default:
                    taste();
                    break;
            }
        }
        public static void einstellungen()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                       PrimeConsoleCore :: Settings                       ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                        What do you want to change?                       ║");
            Console.WriteLine("  ║             (type the relevant number to change the setting)             ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║          activate/deactivate sounds                   : press 1          ║");
            Console.WriteLine("  ║          activate/deactivate \"Prime found!!\" message:   press 2          ║");
            Console.WriteLine("  ║          activate/deactivate welcome message          : press 3          ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.Write(" >> ");
            var einstellung = "";
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    Console.Write("\n");
                    Console.WriteLine("Type \"True\" or \"False\"");
                    einstellung = Console.ReadLine();
                    if (einstellung == "true" || einstellung == "True")
                    {
                        config["sound"] = "true";
                    }
                    else if (einstellung == "false" || einstellung == "False")
                    {
                        config["sound"] = "false";
                    }
                    else
                    {
                        Console.WriteLine("Only \"True\" or \"False\"!");
                        Console.ReadLine();
                        break;
                    }
                    break;
                case ConsoleKey.D2:
                    Console.Write("\n");
                    Console.WriteLine("Type \"True\" or \"False\"");
                    einstellung = Console.ReadLine();
                    if (einstellung == "true" || einstellung == "True")
                    {
                        config["prime_found"] = "true";
                    }
                    else if (einstellung == "false" || einstellung == "False")
                    {
                        config["prime_found"] = "false";
                    }
                    else
                    {
                        Console.WriteLine("Only \"True\" or \"False\"!");
                        Console.ReadLine();
                        break;
                    }
                    break;
                case ConsoleKey.D3:
                    Console.Write("\n");
                    Console.WriteLine("Type \"True\" or \"False\"");
                    einstellung = Console.ReadLine();
                    if (einstellung == "true" || einstellung == "True")
                    {
                        config["welcome_message"] = "true";
                    }
                    else if (einstellung == "false" || einstellung == "False")
                    {
                        config["welcome_message"] = "false";
                    }
                    else
                    {
                        Console.WriteLine("Only \"True\" or \"False\"!");
                        Console.ReadLine();
                        break;
                    }
                    break;
                default:
                    break;
            }
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(config, Formatting.Indented));
            Console.WriteLine("Do you want to change other settings? (yes / no)");
            switch(Console.ReadLine())
            {
                case "yes":
                    einstellungen();
                    break;
                case "Ja":
                    einstellungen();
                    break;
                case "no":
                    break;
                case "nein":
                    break;
                default:
                    break;
            }
            Console.Clear();
        }
        public static void hilfe()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                         PrimeConsoleCore :: Help                         ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║          Press the relevant button to start the relevant option          ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                        Stop calculating : press P                        ║");
            Console.WriteLine("  ║                        Show help        : press H                        ║");
            Console.WriteLine("  ║                        Show settings    : press E                        ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                       Go back by pressing a button                       ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.ReadLine();
            Console.Clear();
        }
        public static void start()
        {
            Console.WriteLine("");
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                       PrimeConsoleCore :: Welcome!                       ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                        Stop calculating: press P                         ║");
            Console.WriteLine("  ║                        Show help:        press H                         ║");
            Console.WriteLine("  ║                        Show settings:    press E                         ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                  Start calculation by pressing a button                  ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.E:
                    einstellungen();
                    start();
                    break;
                case ConsoleKey.H:
                    hilfe();
                    start();
                    break;
                default:
                    break;
            }
        }
        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
