using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
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
                    DataForWrappers.SenderPort), request.Request);
            
                Dictionary<string, MyDelegate> commands = new Dictionary<string, MyDelegate>
                {
                    {"SignIn", SignIn },
                    {"ChangePassword", ChangePassword },
                    {"GetShopLot", GetShopLot },
                    {"GetShopLotsList", GetShopLotsList },
                    //{"GetShopLots", GetShopLots },
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
                    Server.Send(request.Client, new byte[]{0});
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
                Server.Send(request.Client, new byte[]{0});
                return;
            }

            if (!account.Verify(request.Command[3])) // checking password
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            Account newAccount = new Account(request.Command[1], request.Command[2]);

            try // add account to shop
            {
                DataForWrappers.Shop.AddAccount(newAccount);
                Server.Send(request.Client, new byte[] { 1 });
            }
            catch (ArgumentException e) // problem with naming
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
            catch // other problems
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
        }

        private static void ChangePassword(RequestHolder request) // [1]new_password [2]password [3]my_account_name
        {
            Account account = DataForWrappers.Shop.GetAccount(request.Command[3]); // loading an account
            if (account == null)
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            try
            {
                account.ChangePassword(request.Command[1], request.Command[2]);
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
                Server.Send(request.Client, new byte[] { 1 });
            }
            catch (MemberAccessException e) // problem with changing password
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
            catch (ArgumentException e) // problem with applying changes
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
            catch // other problems
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
        }

        private static void GetShopLot(RequestHolder request) // [1]name
        {
            ShopLot result = DataForWrappers.Shop.GetShopLot(request.Command[1]); // loading shoplot
            if (result == null)
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            Server.Send(request.Client, result.Save());
        }

        private static void GetShopLotsList(RequestHolder request) // [1]void
        {
            var result = new ShopLib.ListOfGoods();

            foreach (var lot in DataForWrappers.Shop.GetShopLots()) // filling list
            { 
                var t = new ThumbGoods(lot.Name, lot.Picture);
                result.Goods.Add(t);
            }
            Server.Send(request.Client, result.Save());
        }

        /*private static void GetShopLots(RequestHolder request) // [1]-[infinity]tags
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
        }*/

        private static void Refill(RequestHolder request) // [1]sum [2]my_account_name [3]verify
        {
            if (request.Command[3] != "true") // checking verify
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            Account account = DataForWrappers.Shop.GetAccount(request.Command[2]); // loading account
            if (account == null)
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            long sum; // checking sum for being decimal
            try
            {
                sum = Convert.ToInt64(request.Command[1]);
            }
            catch
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            account.Refill(sum, DataForWrappers.AdminAccount, DataForWrappers.AdminPassword);
            try
            {
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
                Server.Send(request.Client, new byte[]{1});
            }
            catch (ArgumentException) // problem when applying
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
            catch // other problem
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
        }

        private static void Buy(RequestHolder request) // [1]item [2]password [3]my_account_name
        {
            ShopLot lot = DataForWrappers.Shop.GetShopLot(request.Command[1]); // loading shoplot
            if (lot == null)
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            Account account = DataForWrappers.Shop.GetAccount(request.Command[3]); // loading account
            if (account == null)
            {
                Server.Send(request.Client, new byte[] { 0 });
                return;
            }

            try
            {
                account.Withdraw(lot.Price, request.Command[2]);
                DataForWrappers.Shop.UpdateAccount(account); // applying changes
                Console.WriteLine($"{request.Command[3]} have bought {lot.Name} for {lot.Price}");
                Server.Send(request.Client, new byte[] { 1 });
            }
            catch (ArgumentOutOfRangeException e) // problem with withdraw
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
            catch (ArgumentException e) // problem with applying
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
            catch (MemberAccessException e) // problem with withdraw
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
            catch // other problem
            {
                Server.Send(request.Client, new byte[] { 0 });
            }
        }
    }
}
