using System;
using System.Collections.Generic;
using MyShopServerMain.core.shop;

namespace MyShopServerMain.core.wrappers.server
{
    internal static class RequestsProcessor
    {
        private delegate void MyDelegate(RequestHolder request);

        internal static void ProcessRequest(Object t_request)
        {
            Dictionary<string, MyDelegate> commands = new Dictionary<string, MyDelegate>
                {
                    {"SignIn", SignIn },
                    {"ChangePassword", ChangePassword },
                    {"GetShopLot", GetShopLot },
                    {"GetShopLotsList", GetShopLotsList },
                    {"GetShopLots", GetShopLots },
                    {"Refill", Refill },
                    {"Buy", Buy }
                };

            RequestHolder request = t_request as RequestHolder;
            string t_command = DataForWrappers.encoding.GetString(request.request);

            request.command = t_command.Split();

            if (commands.ContainsKey(request.command[0])) // processing command[0]
            {
                commands[request.command[0]](request);
            }
            else
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error", "Not a command."));
            }
        }



        private static void SignIn(RequestHolder request) // [1]name [2]new_password [3]password [4]my_account_name
        {
            // try{} catch{} faster?
            /*if (DataForWrappers.Shop.GetAccounts().Contains(_command[1]))
            {
                Server.SendAnswer(IpAddress, Server.CreateAnswer("Error", "Name is busy"));
                return;
            }*/

            var account = DataForWrappers.Shop.GetAccount(request.command[4]); // loading account
            if (account == null)
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error", "No account."));
                return;
            }

            if (!account.Verify(request.command[3])) // checking password
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error", "Wrong password."));
                return;
            }

            Account newAccount = new Account(request.command[1], request.command[2]);

            try // add account to shop
            {
                DataForWrappers.Shop.AddAccount(newAccount);
                Server.SendAnswer(request.client, Server.CreateAnswer("Complete"
                    , $"Congratulate you {account.Name}." + Environment.NewLine +
                      $"Account for {request.command[1]} successfully created."));
            }
            catch (ArgumentException e) // problem with naming
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error", e.Message));
            }
            catch // other problems
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }

        private static void ChangePassword(RequestHolder request) // [1]new_password [2]password [3]my_account_name
        {
            Account account = DataForWrappers.Shop.GetAccount(request.command[3]); // loading an account
            if (account == null)
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error", "404"));
                return;
            }

            try
            {
                account.ChangePassword(request.command[1], request.command[2]);
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
                Server.SendAnswer(request.client, Server.CreateAnswer("Complete"
                    , $"Congratulate you {account.Name}." + Environment.NewLine +
                      "Password has changed."));
            }
            catch (MemberAccessException e) // problem with changing password
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error"
                    , e.Message));
            }
            catch (ArgumentException) // problem with applying changes
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error"
                    , "Something wrong with your account."
                    + Environment.NewLine +
                    "Please contact with our support."));
            }
            catch // other problems
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }

        private static void GetShopLot(RequestHolder request) // [1]name
        {
            ShopLot result = DataForWrappers.Shop.GetShopLot(request.command[1]); // loading shoplot
            if (result == null)
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "There is no similar goods."));
                return;
            }

            Server.SendAnswer(request.client, Server.CreateAnswer("Complete",
                $"{result.Name} - {result.Price}" + Environment.NewLine +
                $"{result.About}"), result.Picture);
        }

        private static void GetShopLotsList(RequestHolder request) // [1]void
        {
            string list = string.Empty;
            List<string> images = new List<string>();

            foreach (var lot in DataForWrappers.Shop.GetShopLots()) // filling list
            {
                list += lot.Name;
                list += Environment.NewLine;
                images.Add("_" + lot.Picture);
            }
            Server.SendAnswer(request.client, Server.CreateAnswer("Complete",
                list), images);
        }

        private static void GetShopLots(RequestHolder request) // [1]-[infinity]tags
        {
            string result = string.Empty;
            List<string> images = new List<string>();

            string[] tags = new string[request.command.Length - 1]; // getting tags

            foreach (var temp in DataForWrappers.Shop.GetShopLots(tags)) // refactor list of results
            {
                result += temp.Name;
                result += Environment.NewLine;
                images.Add("_" + temp.Picture);
            }

            Server.SendAnswer(request.client, Server.CreateAnswer("Complete",
                result), images);
        }

        private static void Refill(RequestHolder request) // [1]sum [2]my_account_name [3]verify
        {
            if (request.command[3] != "true") // checking verify
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "I can't do it."));
                return;
            }

            Account account = DataForWrappers.Shop.GetAccount(request.command[2]); // loading account
            if (account == null)
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error", "404"));
                return;
            }

            decimal sum; // checking sum for being decimal
            try
            {
                sum = Convert.ToDecimal(request.command[1]);
            }
            catch
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "Not a number."));
                return;
            }

            account.Refill(sum, DataForWrappers.AdminAccount, DataForWrappers.AdminPassword);
            try
            {
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
            }
            catch (ArgumentException) // problem when applying
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error"
                    , "Something wrong with your account."
                      + Environment.NewLine +
                      "Please contact with our support."));
            }
            catch // other problem
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }

        private static void Buy(RequestHolder request) // [1]item [2]password [3]my_account_name
        {
            ShopLot lot = DataForWrappers.Shop.GetShopLot(request.command[1]); // loading shoplot
            if (lot == null)
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "There is no similar goods."));
                return;
            }

            Account account = DataForWrappers.Shop.GetAccount(request.command[3]); // loading account
            if (account == null)
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error", "404"));
                return;
            }

            try
            {
                account.Withdraw(lot.Price, request.command[2]);
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
                Console.WriteLine($"{request.command[3]} have bought {lot.Name} for {lot.Price}");
                Server.SendAnswer(request.client, Server.CreateAnswer("Complete",
                    $"You have bought {lot.Name}"));
            }
            catch (ArgumentOutOfRangeException e) // problem with withdraw
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    e.Message));
            }
            catch (ArgumentException) // problem with applying
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error"
                    , "Something wrong with your account."
                      + Environment.NewLine +
                      "Please contact with our support."));
            }
            catch (MemberAccessException e) // problem with withdraw
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    e.Message));
            }
            catch // other problem
            {
                Server.SendAnswer(request.client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }
    }
}