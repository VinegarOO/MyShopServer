using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShopServerMain.core.shop;
using ShopServerMain.core.wrappers.server;

namespace ShopServerMain.core.wrappers
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
        internal const string ServerIpAddres = "127.0.0.1";
        internal const int SenderPort = 703;
        internal const int ServerPort = 708;
        internal const int MaxSizeOfShopLot = 1048576;

        static DataForWrappers()
        {
            AdminAccount = new Account("Admin", AdminPassword, AccessRights.Admin);
            /*while (true)
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
            }*/



            DataForWrappers.Shop = new Shop(); // load or create shop

        }
    }
}
