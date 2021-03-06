﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Numerics;
using System.Net;

namespace PrimeConsoleCore
{
    public class Login
    {
        public static string fail;
        public static void login()
        {
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ║                 PrimeConsoleCore :: Login into PrimeStats                ║");
                Console.WriteLine("  ║                                                                          ║");
                Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("");
                if (Program.config["username"] != "" && fail != "")
                {
                    Program.user = Program.config["username"];
                }
                if (fail != null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(fail + "\n");
                    fail = "";
                    Console.ResetColor();
                }
                if(fail == null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("   To use PrimeConsoleCore you have to login into PrimeStats.\n");
                    Console.ResetColor();
                }
                fail = "";
                Console.Write("   Please type your username: ");
                System.Windows.Forms.SendKeys.SendWait(Program.user);
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
                if (Program.user == "")
                {
                    fail = "   Please enter a username. \n";
                    Program.pass = "";
                    Console.Clear();
                }
                if (Program.pass == "")
                {
                    fail = fail + "   Please enter a password. \n";
                    Console.Clear();
                }
                if (fail == "")
                {
                    Dictionary<string, string> login_values = new Dictionary<string, string>();
                    try
                    {
                        Dictionary<string, string> values = new Dictionary<string, string>();
                        values.Add("username", Program.user);
                        values.Add("password", Program.pass);
                        login_values = connect(Program.config["link"] + "prime/client_login.php", values);
                    }
                    catch (System.Net.WebException)
                    {
                        fail = fail + "   No connection to " + Program.config["link"] + "! Check your internet settings or try again later.";
                        Program.pass = "";
                        Console.Clear();
                    }
                    catch (Exception e)
                    {
                        fail = Convert.ToString(e);
                        Program.pass = "";
                        Console.Clear();
                    }
                    if (login_values.ContainsKey("error"))
                    {
                        if (login_values["error"] == "0")
                        {
                            fail = fail + "   Unknown Error. (0)";
                        }
                        if (login_values["error"] == "1")
                        {
                            fail = fail + "   The username \"" + Program.user + "\" do not exists. (1)";
                        }
                        if (login_values["error"] == "2")
                        {
                            fail = fail + "   Your password is wrong. (2)";
                        }
                        if (login_values["error"] == "3")
                        {
                            fail = fail + "   You did not fill username and password.";
                        }
                        Program.pass = "";
                        Console.Clear();
                    }
                    if (fail == "")
                    {
                        if (((login_values["check"] == "true" && login_values["online"] == "false") || (login_values["check"] == "true" && login_values["online"] == "true")))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write("\n\n   You're logged in! Login on ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("http://stephchan.wf4.eu/prime/ ");
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write("and activate \n   the session. Then wait a few seconds...");
                            Console.ResetColor();
                            bool check = false;
                            while (true)
                            {
                                System.Threading.Thread.Sleep(3000);
                                try
                                {
                                    Dictionary<string, string> values = new Dictionary<string, string>();
                                    values.Add("username", Program.user);
                                    values.Add("password", Program.pass);
                                    login_values = connect(Program.config["link"] + "prime/client_login.php", values);
                                    if (login_values["online"] == "true")
                                    {
                                        Program.userconfig = login_values;
                                        check = true;
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                            if (check == true)
                            {
                                if (Program.config["username"] == "")
                                {
                                    Program.config["username"] = Program.user;
                                    System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.ParsePath(@"%localappdata%\\PCC\config.json")), JsonConvert.SerializeObject(Program.config, Formatting.Indented));
                                }
                                Program.primefromprimestats = BigInteger.Parse(Program.userconfig["prime"]);
                                Program.token = Program.userconfig["token"];
                                Console.Clear();
                                return;
                                // Login success
                            }
                            else
                            {
                                // Fail when checking false
                            }
                        }
                        else
                        {
                            // Fail after connecting
                        }
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    // Fail after wrong user creditials
                }
            }
        }
        public static Dictionary<string, string> connect(string url, Dictionary<string, string> values)
            {
                try
                {
                    string result;
                    using (var client = new WebClient())
                    {
                        var linkvalues = new NameValueCollection();
                        foreach (KeyValuePair<string, string> pair in values)
                        {
                            linkvalues[pair.Key] = pair.Value;
                        }
                        result = Encoding.Default.GetString(client.UploadValues(url, linkvalues));
                        result = result.Replace("&quot;", "\"");
                    }
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                }
                catch(Exception e)
                {
                Console.WriteLine(Convert.ToString(e));
                return null;
                }         
            }
    }
}
