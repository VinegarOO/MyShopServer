using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading;
using MyShopServerMain.core.shop;
using MyShopServerMain.core.wrappers.DB;

namespace MyShopServerMain.core.wrappers.server
{
    internal static class RequestsProcessor
    {
        private delegate void MyDelegate(RequestHolder request);

        internal static void ProcessRequest()
        {
            while (true)
            {
                RequestHolder request;

                while (!DataForWrappers.Requests.TryDequeue(out request))
                {
                    Thread.Sleep(10);
                }
                
                request = new RequestHolder(new IPEndPoint(((IPEndPoint)request.Client).Address,
                    ((IPEndPoint)request.Client).Port + 1), request.Request);
            
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
            
                string tCommand = DataForWrappers.Encoding.GetString(request.Request);

                request.Command = tCommand.Split();

                if (commands.ContainsKey(request.Command[0])) // processing command[0]
                {
                    commands[request.Command[0]](request);
                }
                else
                {
                    Server.SendAnswer(request.Client, Server.CreateAnswer("Error", "Not a command."));
                }
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

            var account = DataForWrappers.Shop.GetAccount(request.Command[4]); // loading account
            if (account == null)
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error", "No account."));
                return;
            }

            if (!account.Verify(request.Command[3])) // checking password
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error", "Wrong password."));
                return;
            }

            Account newAccount = new Account(request.Command[1], request.Command[2]);

            try // add account to shop
            {
                DataForWrappers.Shop.AddAccount(newAccount);
                Server.SendAnswer(request.Client, Server.CreateAnswer("Complete"
                    , $"Congratulate you {account.Name}." + Environment.NewLine +
                      $"Account for {request.Command[1]} successfully created."));
            }
            catch (ArgumentException e) // problem with naming
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error", e.Message));
            }
            catch // other problems
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }

        private static void ChangePassword(RequestHolder request) // [1]new_password [2]password [3]my_account_name
        {
            Account account = DataForWrappers.Shop.GetAccount(request.Command[3]); // loading an account
            if (account == null)
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error", "404"));
                return;
            }

            try
            {
                account.ChangePassword(request.Command[1], request.Command[2]);
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
                Server.SendAnswer(request.Client, Server.CreateAnswer("Complete"
                    , $"Congratulate you {account.Name}." + Environment.NewLine +
                      "Password has changed."));
            }
            catch (MemberAccessException e) // problem with changing password
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error"
                    , e.Message));
            }
            catch (ArgumentException) // problem with applying changes
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error"
                    , "Something wrong with your account."
                    + Environment.NewLine +
                    "Please contact with our support."));
            }
            catch // other problems
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }

        private static void GetShopLot(RequestHolder request) // [1]name
        {
            ShopLot result = DataForWrappers.Shop.GetShopLot(request.Command[1]); // loading shoplot
            if (result == null)
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    "There is no similar goods."));
                return;
            }

            Server.SendAnswer(request.Client, Server.CreateAnswer("Complete",
                $"{result.Name} - {result.Price}" + Environment.NewLine +
                $"{result.About}"), result.Picture);
        }

        private static void GetShopLotsList(RequestHolder request) // [1]void
        {
            string list = string.Empty;
            List<Image> images = new List<Image>();

            foreach (var lot in DataForWrappers.Shop.GetShopLots()) // filling list
            {
                list += lot.Name;
                list += Environment.NewLine;
                Image temp = (lot.Image);
                images.Add(temp);
            }
            Server.SendAnswer(request.Client, Server.CreateAnswer("Complete",
                list), images);
        }

        private static void GetShopLots(RequestHolder request) // [1]-[infinity]tags
        {
            string result = string.Empty;
            List<Image> images = new List<Image>();

            string[] tags = new string[request.Command.Length - 1]; // getting tags

            foreach (ShopLot lot in DataForWrappers.Shop.GetShopLots(tags)) // refactor list of results
            {
                result += lot.Name;
                result += Environment.NewLine;
                Image temp = MyDb.GetData<Image>(lot.Name);
                images.Add(temp);
            }

            Server.SendAnswer(request.Client, Server.CreateAnswer("Complete",
                result), images);
        }

        private static void Refill(RequestHolder request) // [1]sum [2]my_account_name [3]verify
        {
            if (request.Command[3] != "true") // checking verify
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    "I can't do it."));
                return;
            }

            Account account = DataForWrappers.Shop.GetAccount(request.Command[2]); // loading account
            if (account == null)
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error", "404"));
                return;
            }

            decimal sum; // checking sum for being decimal
            try
            {
                sum = Convert.ToDecimal(request.Command[1]);
            }
            catch
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
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
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error"
                    , "Something wrong with your account."
                      + Environment.NewLine +
                      "Please contact with our support."));
            }
            catch // other problem
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }

        private static void Buy(RequestHolder request) // [1]item [2]password [3]my_account_name
        {
            ShopLot lot = DataForWrappers.Shop.GetShopLot(request.Command[1]); // loading shoplot
            if (lot == null)
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    "There is no similar goods."));
                return;
            }

            Account account = DataForWrappers.Shop.GetAccount(request.Command[3]); // loading account
            if (account == null)
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error", "404"));
                return;
            }

            try
            {
                account.Withdraw(lot.Price, request.Command[2]);
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
                Console.WriteLine($"{request.Command[3]} have bought {lot.Name} for {lot.Price}");
                Server.SendAnswer(request.Client, Server.CreateAnswer("Complete",
                    $"You have bought {lot.Name}"));
            }
            catch (ArgumentOutOfRangeException e) // problem with withdraw
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    e.Message));
            }
            catch (ArgumentException) // problem with applying
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error"
                    , "Something wrong with your account."
                      + Environment.NewLine +
                      "Please contact with our support."));
            }
            catch (MemberAccessException e) // problem with withdraw
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    e.Message));
            }
            catch // other problem
            {
                Server.SendAnswer(request.Client, Server.CreateAnswer("Error",
                    "Something goes wrong, try again."));
            }
        }
    }
}