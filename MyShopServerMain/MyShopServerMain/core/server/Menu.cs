using System;
using System.Collections.Generic;
using MyShopServerMain.core.shop;

namespace MyShopServerMain.core.server
{
    static class Menu
    {
        delegate void CommandDelegate(string[] command, Shop shop);

        internal static bool ServerMenu(string[] command, Shop shop)
        {
            Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>
            {
                {"LogIn", LogIn },
                {"SignIn", SignIn },
                {"LogOut", LogOut },
                {"ChangePassword", ChangePassword },
                {"GetShopLot", GetShopLot },
                {"GetShopLotsList", GetShopLotsList },
                {"GetShopLots", GetShopLots },
                {"Refill", Refill },
                {"Buy", Buy }
            };

            if (commands.ContainsKey(command[0]))
            {
                commands[command[0]](command, shop);
                return true;
            }
            else
            {
                return false;
            }
        }


        private static void LogIn(string[] command, Shop shop)
        {

        }

        private static void SignIn(string[] command, Shop shop)
        {

        }

        private static void LogOut(string[] command, Shop shop)
        {

        }

        private static void ChangePassword(string[] command, Shop shop)
        {

        }

        private static void GetShopLot(string[] command, Shop shop)
        {

        }

        private static void GetShopLotsList(string[] command, Shop shop)
        {

        }

        private static void GetShopLots(string[] command, Shop shop)
        {

        }

        private static void Refill(string[] command, Shop shop)
        {

        }

        private static void Buy(string[] command, Shop shop)
        {

        }
    }
}