using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MyShopServerMain.core.shop;

namespace MyShopServerMain.core.wrappers
{
    static class DataForWrappers
    {
        internal static readonly Shop Shop;
        internal static string[] TerminalCommand;
        internal static readonly Account AdminAccount;
        internal const string AdminPassword = "qwerty";
        internal static Encoding encoding = new UTF8Encoding();

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
