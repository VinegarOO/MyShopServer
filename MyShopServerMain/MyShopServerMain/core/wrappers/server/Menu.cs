using System.Collections.Generic;
using MyShopServerMain.core.shop;
using static MyShopServerMain.core.wrappers.DataForWrappers;

namespace MyShopServerMain.core.wrappers.server
{
    static class Menu
    {
        delegate void CommandDelegate(string[] command);

        internal static bool ServerMenu(string[] command)
        {
            Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>
            {
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
                commands[command[0]](command);
                return true;
            }
            else
            {
                return false;
            }
        }



        private static void SignIn(string[] command)
        {

        }

        private static void LogOut(string[] command)
        {

        }

        private static void ChangePassword(string[] command)
        {

        }

        private static void GetShopLot(string[] command)
        {

        }

        private static void GetShopLotsList(string[] command)
        {

        }

        private static void GetShopLots(string[] command)
        {

        }

        private static void Refill(string[] command)
        {

        }

        private static void Buy(string[] command)
        {

        }
    }
}