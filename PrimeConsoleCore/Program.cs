using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.Numerics;
using System.Runtime.InteropServices;

namespace PrimeConsoleCore
{
    public class Program
    {
        /* Variables */
        private static EventWaitHandle[] waithandle = new AutoResetEvent[Environment.ProcessorCount];
        // Numbers
        public static BigInteger primefromprimestats = 1;
        public static BigInteger primeoverall = 0;
        public static BigInteger prime = 0;
        public static int threads = Environment.ProcessorCount;
        // Text
        public static string user;
        public static string pass;
        public static string token;
        public static string version = "0.9.5";
        // Bools
        public static bool visible = true;
        public static bool listediting = false;
        public static bool stop = false;
        public static bool setnewprimenumber = false;

        /* Dictionarys */
        public static Dictionary<string, string> config = new Dictionary<string, string>();
        public static Dictionary<string, string> userconfig = new Dictionary<string, string>();

        /* Threads */
        public static Thread buttonthread = new Thread(new ThreadStart(button));

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

        /* Main*/
        public static void Main()
        {
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
            start();
            Parallel.Invoke(() =>
                 {
                     if(config["prime_found"] == "false")
                     {
                         Console.WriteLine(" >> Calculating started.");
                     }
                     calculate();
                 },
                 () =>
                 {
                     buttonthread.Start();
                 }
                );
        }
        public static void sendprimesperbutton()
        {
            Dictionary<string, string> sendprimes_values = new Dictionary<string, string>();
            try
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("username",Program.user);
                values.Add("token", Program.token);
                values.Add("prime", Convert.ToString(prime));
                sendprimes_values = Login.connect(Program.config["link"] + "prime/client_sendprimes.php", values);
            }
            catch (Exception e)
            {
                Console.WriteLine(Convert.ToString(e));
                Console.ReadLine();
            }
            if (sendprimes_values["send"] == "true")
            {
                Program.token = sendprimes_values["token"];
                Console.WriteLine(" >> Primes were send to the server.");
            }
            else if (sendprimes_values["send"] == "false")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" >> Primes were not send to the server! (1)");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.WriteLine(" >> Primes were not send to the server! (2)");
            }
        }
        public static void stopcalculation ()
        {
            stop = true;
            for(int i = 0; i < Environment.ProcessorCount; i++)
            {
                waithandle[i].Reset();
            }
            Console.WriteLine("");
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║  You stopped the calculation. Press a button to resume the calculation.  ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("");
            Console.WriteLine(" >> Number of all found primes: " + primeoverall);
            Console.WriteLine(" >> Current prime: " + prime);
            sendprimesperbutton();
            Console.ReadLine();
            stop = false;
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                if(i != 0)
                {
                    System.Threading.Thread.Sleep(100 * i);
                }
                waithandle[i].Set();
            }
            if (config["prime_found"] == "false")
            {
                Console.WriteLine(" >> Calculating continued.");
            }
        }
        public static void calculate ()
        {
            List<BigInteger> givennumbers = new List<BigInteger>();
            BigInteger newnumber = primefromprimestats + 1;
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                givennumbers.Add(newnumber);
                newnumber++;
            }
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                waithandle[i] = new AutoResetEvent(true);
            }
            try
            {
                Parallel.For(0, Environment.ProcessorCount,
                    once =>
                    {
                        while (true)
                        {
                            if (stop == true)
                            {
                                waithandle[once].WaitOne();
                            }
                            BigInteger testnumber = givennumbers.ElementAt(0);
                                while (listediting == true)
                                {
                                    System.Threading.Thread.Sleep(1);
                                }
                            listediting = true;
                            givennumbers.Remove(givennumbers.ElementAt(0));
                            givennumbers.Add(givennumbers.ElementAt(Environment.ProcessorCount - 2) + 1);
                            listediting = false;
                            bool checking = checknumber(testnumber, once);
                            if (checking == true)
                            {
                                primeoverall++;
                                if (prime < testnumber)
                                {
                                    while (setnewprimenumber == true)
                                    {
                                        System.Threading.Thread.Sleep(1);
                                    }
                                    setnewprimenumber = true;
                                    prime = testnumber;
                                    setnewprimenumber = false;
                                }
                                if (visible == true && config["prime_found"] == "true")
                                {
                                    Console.WriteLine(" >> You found a new Primenumber: " + testnumber);
                                    if(config["sound"] == "true")
                                    {
                                        Console.Beep(5000, 1);
                                    }
                                }
                            }
                        }
                    }
                    );
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
        public static bool checknumber (BigInteger number, int thread)
        {
            BigInteger divisor = 2;
            bool calculating = true;
            while (calculating == true)
            {
                if(stop == true)
                {
                    waithandle[thread].WaitOne();
                }
                if (number % divisor == 0)
                {
                    if (number == divisor)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    divisor++;
                }
                if (number == 1)
                {
                    number++;
                    divisor = 2;
                }
            }
            return false;
        }
        public static void button()
        {
            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.P:
                        Console.Write("\b \b");
                        stopcalculation();
                        break;
                    case ConsoleKey.S:
                        visible = false;
                        settings();
                        visible = true;
                        break;
                    case ConsoleKey.H:
                        visible = false;
                        help();
                        visible = true;
                        break;
                    case ConsoleKey.W:
                        sendprimesperbutton();
                        break;
                    case ConsoleKey.N:
                        if (config["prime_found"] == "false")
                        {
                            Console.WriteLine(" >> New calculated prime: " + prime);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private static void truefalse(string name, string setting)
        {
            Console.Write("\n");
            Console.WriteLine(" >> Your editing \""+name+"\": Currently "+config[setting]+"");
            Console.WriteLine(" >> Type \"true\" or \"false\"");
            Console.Write(" >> ");
            switch (Console.ReadLine())
            {
                case "true":
                case "True":
                case "t":
                    config[setting] = "true";
                    break;
                case "false":
                case "False":
                case "f":
                    config[setting] = "false";
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" >> Only \"true\" or \"false\"!");
                    Console.ResetColor();
                    Console.ReadLine();
                    break;
            }
        }
        public static void settings()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║                       PrimeConsoleCore :: Settings                       ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║                     What do you would like to change?                    ║");
                Console.WriteLine("  ║             (type the relevant number to change the setting)             ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║          activate/deactivate sounds                   : press 1          ║");
                Console.WriteLine("  ║          activate/deactivate \"Prime found!!\" message: press 2            ║");
                Console.WriteLine("  ║          activate/deactivate welcome message          : press 3          ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
                Console.Write(" >> ");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        truefalse("sounds", "sound");
                        break;
                    case ConsoleKey.D2:
                        truefalse("prime found message", "prime_found");
                        break;
                    case ConsoleKey.D3:
                        truefalse("enable welcome message", "welcome_message");
                        break;
                    default:
                        break;
                }
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(config, Formatting.Indented));
                Console.WriteLine(" >> Do you would like to change other settings? (yes / no)");
                switch (Console.ReadLine())
                {
                    case "yes":
                    case "Ja":
                        break;
                    case "no":
                    case "nein":
                        Console.Clear();
                        return;
                    default:
                        Console.Clear();
                        return;
                }
                Console.Clear();
            }
        }
        public static void help()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                         PrimeConsoleCore :: Help                         ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║          Press the relevant button to start the relevant option          ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                   Stop calculating           : press P                   ║");
            Console.WriteLine("  ║                   Show help                  : press H                   ║");
            Console.WriteLine("  ║                   Change settings            : press S                   ║");
            Console.WriteLine("  ║                   Send primes to PrimeStats  : press W                   ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                       Go back by pressing a button                       ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.ReadLine();
            Console.Clear();
        }
        public static void start()
        {
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║                       PrimeConsoleCore :: Welcome!                       ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║                   Stop calculating           : press P                   ║");
                Console.WriteLine("  ║                   Show help                  : press H                   ║");
                Console.WriteLine("  ║                   Change settings            : press S                   ║");
                Console.WriteLine("  ║                   Send primes to PrimeStats  : press W                   ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║                      Your calculting with "+threads+" threads.                     ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║                    Start calculation by pressing Enter                   ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("");
                Console.WriteLine("  Highest calculated prime from PrimeStats: " + primefromprimestats + "");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.S:
                        settings();
                        break;
                    case ConsoleKey.H:
                        help();
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
