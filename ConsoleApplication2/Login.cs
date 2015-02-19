using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Net;

namespace PrimeConsoleCore
{
    public class Login
    {
        static string fail = "";
        public static void login()
        {
            Console.WriteLine("");
            Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ║                      PrimeConsoleCore :: Login-Menu                      ║");
            Console.WriteLine("  ║                                                                          ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("");
            if (fail != "")
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("   " + fail + "\n");
                Console.ResetColor();
            }
            Console.Write("   Please type your username: ");
            Program.user = Console.ReadLine();
            Console.Write("   Please type your password: ");
            ConsoleKeyInfo Key = Console.ReadKey(true);
            while (Key.Key != ConsoleKey.Enter)
            {
                if (Key.Key != ConsoleKey.Backspace)
                {
                    Program.pass += Key.KeyChar;
                    Key = Console.ReadKey(true);
                }
                else if (Key.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(Program.pass))
                    {
                        Program.pass = Program.pass.Substring
                        (0, Program.pass.Length - 1);
                    }
                    Key = Console.ReadKey(true);
                }
            }
            Program.pass = Program.sha256_hash(Program.pass);
            try
            {
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["username"] = Program.user;
                    values["password"] = Program.pass;
                    Program.resultToken = Encoding.Default.GetString(client.UploadValues("http://prime.steph.ml/login_token.php", values));
                }
                Program.resultToken = Program.resultToken.Replace("&quot;", "\"");
                Program.userconfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(Program.resultToken);
            }
            catch(System.Net.WebException)
            {
                fail = "No connection to http://prime.steph.ml/! Check your internet settings or\n   try again later.";
                Program.pass = "";
                Console.Clear();
                login();
            }
            if (Program.userconfig["token"] != "false")
            {
                Program.zahl = BigInteger.Parse(Program.userconfig["prime"]);
                Console.Clear();
                Program.start();
            }
            else
            {
                fail = "The username or password was incorrect!";
                Program.pass = "";
                Console.Clear();
                login();
            }
        }
    }
}
