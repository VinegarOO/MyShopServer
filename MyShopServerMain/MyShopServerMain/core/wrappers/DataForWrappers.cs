using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyShopServerMain.core.shop;
using MyShopServerMain.core.wrappers.server;

namespace MyShopServerMain.core.wrappers
{
    class DataForWrappers
    {
        internal static readonly Shop Shop;
        internal static string[] TerminalCommand;
        internal static readonly Account AdminAccount;
        internal const string AdminPassword = "qwerty";
        internal static readonly Encoding Encoding = new UTF8Encoding();
        internal static ConcurrentQueue<RequestHolder> Requests = new ConcurrentQueue<RequestHolder>();
        internal static ConcurrentQueue<AnswerHolder> Answers = new ConcurrentQueue<AnswerHolder>();
        internal static List<Thread> Threads = new List<Thread>();

        static DataForWrappers()
        {
            AdminAccount = new Account("Admin", AdminPassword, AccessRights.Admin);
            while (true)
            {
                Console.WriteLine("enter password");
                Console.Write(">> ");
                string passwd = Console.ReadLine();
                if (AdminAccount.Verify(passwd))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("wrong password");
                }
            }



            DataForWrappers.Shop = null; // load or create shop

            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Do you want to load shop (yes/no)");
                switch (Console.ReadLine())
                {
                    case "yes":
                        {
                            Console.WriteLine("Print path to the shop");
                            try
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                using (FileStream fs = new FileStream($"{Console.ReadLine()}.shop", FileMode.Open))
                                {
                                    DataForWrappers.Shop = (Shop)bf.Deserialize(fs);
                                }

                                flag = false;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            break;
                        }

                    case "no":
                        {
                            Console.WriteLine("Print name of new shop");
                            while (true)
                            {
                                try
                                {
                                    DataForWrappers.Shop = new Shop(Console.ReadLine());

                                    flag = false;
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }

                            break;
                        }

                    default:
                        {
                            Console.WriteLine("You must choose");
                            break;
                        }
                }
            }
        }
    }
}
