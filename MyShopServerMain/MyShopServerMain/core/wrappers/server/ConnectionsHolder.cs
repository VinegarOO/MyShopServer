using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using MyShopServerMain.core.shop;

namespace MyShopServerMain.core.wrappers.server
{
    internal static class ConnectionsHolder
    {
        private static readonly List<ConnectionHolder> _connectionHolders;

        private class ConnectionHolder
        {
            private Account _account;
            private Timer _deletingTimer;
            internal readonly IPAddress Client;
            private string[] _command;

            internal ConnectionHolder(Account tAccount, IPAddress ipAddress)
            {
                _account = tAccount;
                Client = ipAddress;
                _deletingTimer = new Timer(DeleteConnection, this,
                    dueTime: new TimeSpan(0, 30, 0),
                    period: Timeout.InfiniteTimeSpan);
            }


            internal void Menu(string[] nCommand)
            {
                Dictionary<string, Action> commands = new Dictionary<string, Action>
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

                _command = nCommand;

                if (commands.ContainsKey(nCommand[0])) // processing command[0]
                {
                    _deletingTimer.Change(
                        dueTime: new TimeSpan(0, 30, 0),
                        period: Timeout.InfiniteTimeSpan);

                    commands[_command[0]]();
                }
                else
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error", "Not a command."));
                }
            }



            private void SignIn() // [1]name [2]new_password [3]password
            {
                // try{} catch{} faster?
                /*if (DataForWrappers.Shop.GetAccounts().Contains(_command[1]))
                {
                    Server.SendAnswer(IpAddress, Server.CreateAnswer("Error", "Name is busy"));
                    return;
                }*/

                if (!_account.Verify(_command[3])) // checking password
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error", "Wrong password."));
                    return;
                }

                Account newAccount = new Account(_command[1], _command[2]);

                try // add account to shop
                {
                    DataForWrappers.Shop.AddAccount(newAccount);
                    Server.SendAnswer(Client, Server.CreateAnswer("Complete"
                        , $"Congratulate you {_account.Name}." + Environment.NewLine +
                          $"Account for {_command[1]} successfully created."));
                }
                catch (ArgumentException e) // problem with naming
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error", e.Message));
                }
                catch // other problems
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "Something goes wrong, try again."));
                }
            }

            private void LogOut() // [1]void
            {
                _deletingTimer.Dispose();
                DeleteConnection(this);
            }

            private void ChangePassword() // [1]new_password [2]password
            {
                Account account = DataForWrappers.Shop.GetAccount(_account.Name); // loading an account
                if (account == null)
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error", "404"));
                    return;
                }

                try
                {
                    account.ChangePassword(_command[1], _command[2]);
                    DataForWrappers.Shop.RemoveAccount(account); // applying changes
                    DataForWrappers.Shop.AddAccount(account); // applying changes
                    _account = account; // applying changes
                    Server.SendAnswer(Client, Server.CreateAnswer("Complete"
                        , $"Congratulate you {account.Name}." + Environment.NewLine +
                          "Password has changed."));
                }
                catch (MemberAccessException e) // problem with changing password
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error"
                        , e.Message));
                }
                catch (ArgumentException) // problem with applying changes
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error"
                        , "Something wrong with your account."
                        + Environment.NewLine +
                        "Please contact with our support."));
                }
                catch // other problems
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "Something goes wrong, try again."));
                }
            }

            private void GetShopLot() // [1]name
            {
                ShopLot result = DataForWrappers.Shop.GetShopLot(_command[1]); // loading shoplot
                if (result == null)
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "There is no similar goods."));
                    return;
                }

                Server.SendAnswer(Client, Server.CreateAnswer("Complete",
                    $"{result.Name} - {result.Price}" + Environment.NewLine +
                    $"{result.About}"), result.Picture);
            }

            private void GetShopLotsList() // [1]void
            {
                string list = string.Empty;
                foreach (var lot in DataForWrappers.Shop.GetShopLots()) // filling list
                {
                    list += lot;
                    list += Environment.NewLine;
                }
                Server.SendAnswer(Client, Server.CreateAnswer("Complete",
                    list));
            }

            private void GetShopLots() // [1]-[infinity]tags
            {
                string result = string.Empty;
                string[] tags = new string[_command.Length - 1]; // getting tags

                foreach (var temp in DataForWrappers.Shop.GetShopLots(tags)) // refactor list of results
                {
                    result += temp;
                    result += Environment.NewLine;
                }

                Server.SendAnswer(Client, Server.CreateAnswer("Complete",
                    result));
            }

            private void Refill() // [1]sum [2]verify
            {
                if (_command[2] != "true") // checking verify
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "I can't do it."));
                    return;
                }

                Account account = DataForWrappers.Shop.GetAccount(_account.Name); // loading account
                if (account == null)
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error", "404"));
                    return;
                }

                decimal sum; // checking sum for being decimal
                try
                {
                    sum = Convert.ToDecimal(_command[1]);
                }
                catch
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "Not a number."));
                    return;
                }

                account.Refill(sum, DataForWrappers.AdminAccount, DataForWrappers.AdminPassword);
                try 
                {
                    DataForWrappers.Shop.RemoveAccount(account); // applying changes
                    DataForWrappers.Shop.AddAccount(account); // applying changes
                    _account = account; // applying changes
                }
                catch (ArgumentException) // problem when applying
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error"
                        , "Something wrong with your account."
                          + Environment.NewLine +
                          "Please contact with our support."));
                }
                catch // other problem
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "Something goes wrong, try again."));
                }
            }

            private void Buy() // [1]item [2]password
            {
                ShopLot lot = DataForWrappers.Shop.GetShopLot(_command[1]); // loading shoplot
                if (lot == null)
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "There is no similar goods."));
                    return;
                }

                Account account = DataForWrappers.Shop.GetAccount(_account.Name); // loading account
                if (account == null)
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error", "404"));
                    return;
                }

                try
                {
                    account.Withdraw(lot.Price, _command[2]);
                    DataForWrappers.Shop.RemoveAccount(account); // applying changes
                    DataForWrappers.Shop.AddAccount(account); // applying changes
                    _account = account; // applying changes
                    Console.WriteLine($"{_account.Name} have bought {lot.Name} for {lot.Price}");
                    Server.SendAnswer(Client, Server.CreateAnswer("Complete",
                        $"You have bought {lot.Name}"));
                }
                catch (ArgumentOutOfRangeException e) // problem with withdraw
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        e.Message));
                }
                catch (ArgumentException) // problem with applying
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error"
                        , "Something wrong with your account."
                          + Environment.NewLine +
                          "Please contact with our support."));
                }
                catch (MemberAccessException e) // problem with withdraw
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        e.Message));
                }
                catch // other problem
                {
                    Server.SendAnswer(Client, Server.CreateAnswer("Error",
                        "Something goes wrong, try again."));
                }
            }
        }

        static ConnectionsHolder()
        {
            _connectionHolders = new List<ConnectionHolder>();

        }

        internal static void ProcessRequest(IPAddress client, string[] command) // starting processing request
        {
            foreach (var connection in _connectionHolders)
            {
                if (connection.Client.Equals(client))
                {
                    connection.Menu(command);
                }
            }
            throw new ArgumentException("connection not found");
        }

        private static void DeleteConnection(object state) // calling when timer elapse
        {
            _connectionHolders.Remove((ConnectionHolder)state);
        }

        internal static void AddConnection(Account tAccount, IPAddress ipAddress)
        {
            ConnectionHolder connectionHolder = new ConnectionHolder(tAccount, ipAddress);
            _connectionHolders.Add(connectionHolder);
        }
    }
}