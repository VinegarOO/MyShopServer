using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using MyShopServerMain.core.shop;
using MyShopServerMain.core.wrappers.server;
using static MyShopServerMain.core.wrappers.DataForWrappers;

namespace MyShopServerMain.core.wrappers.console
{
    public static class UserInterface
    {
        private static readonly Thread Server = new Thread(server.Server.WaitingConnection);
        private static readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>
        {
            {"start", Start },
            {"exit", Exit },
            {"manage", Manage },
            {"stop", Stop },
            {"help", Help }
        };

        public static void Menu()
        {
            while (true)
            {
                Console.WriteLine("Waiting for command");
                Console.Write("Admin >> ");
                TerminalCommand = Console.ReadLine().Split();

                if (Commands.ContainsKey(TerminalCommand[0]))
                {
                    Commands[TerminalCommand[0]]();
                }
                else
                {
                    Console.WriteLine("Not a command");
                }
            }
        }

        private static void Start()
        {
            Server.Start();
            // Initialise all server Threads
        }

        private static void Manage()
        {
            if (TerminalCommand.Length < 3) // checking for arguments
            {
                Console.WriteLine("wrong arguments");
                return;
            }

            switch (TerminalCommand[3]) // checking type of action
            {
                case "add":
                {
                    Add();
                    break;
                }

                case "edit":
                {
                    Edit();
                    break;
                }

                case "inspect":
                {
                    Inspect();
                    break;
                }

                default:
                {
                    Console.WriteLine("i can't do it");
                    return;
                }
            }
        }

        private static void Add()
        {
            switch (TerminalCommand[1])
            {
                case "account":
                {
                    string password; // getting password
                    while (true)
                    {
                        Console.WriteLine("print new password");
                        password = Console.ReadLine();
                        Console.WriteLine("confirm password");
                        if (Console.ReadLine() == password)
                        {
                            break;
                        }
                        Console.WriteLine("passwords isn't similar");
                    }

                    Account account = new Account(TerminalCommand[2], password); // creating account

                    try // adding account
                    {
                        DataForWrappers.Shop.AddAccount(account);
                    }
                    catch (Exception e) // all mistakes throwing to user console 
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    break;
                }

                case "goods":
                {
                    string picturePath; // getting picture
                    while (true)
                    {
                        Console.WriteLine("print path to the picture");
                        picturePath = Console.ReadLine();
                        if (File.Exists(picturePath))
                        {
                            break;
                        }

                        Console.WriteLine("wrong path");
                    }

                    Console.WriteLine("print goods description"); // getting description
                    string about = Console.ReadLine();

                    decimal price; // getting price
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine("print price");
                            price = Convert.ToDecimal(Console.ReadLine());
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("not a number");
                        }
                    }

                    string[] tags; // getting price
                    while (true)
                    {
                        try
                        {
                            Console.WriteLine(
                                "add tags" + Environment.NewLine +
                                "you can add no tags" + Environment.NewLine +
                                "tags must be spited by spaces"
                            );
                            tags = Console.ReadLine().Split();
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("wrong input");
                        }
                    }

                    ShopLot shopLot = new ShopLot(TerminalCommand[2], picturePath, about, price, tags);

                    try // adding shop lot
                    {
                        DataForWrappers.Shop.AddShopLot(shopLot);
                    }
                    catch (Exception e) // all mistakes throwing to user console
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    break;
                }

                default:
                {
                    Console.WriteLine("wrong argument");
                    return;
                }
            }

            Console.WriteLine("Item added");
        }

        private static void Edit()
        {
            switch (TerminalCommand[1])
            {
                case "account":
                {
                    Account account; // loading account
                    try
                    {
                        account = DataForWrappers.Shop.GetAccount(TerminalCommand[2]);
                    }
                    catch (Exception e) // all mistakes throwing to user console
                    {
                        Console.WriteLine(e.GetType());
                        return;
                    }
                    if (account == null) // checking existing of account
                    {
                        Console.WriteLine("account doesn't exists");
                        return;
                    }

                    Console.WriteLine("What parameter do you want to change");
                    Console.WriteLine("You can change:" + Environment.NewLine +
                                      "Password" + Environment.NewLine +
                                      "AccessRights" + Environment.NewLine + 
                                      "State of account - Money");
                    switch (Console.ReadLine())
                    {
                        case "Password":
                        {
                            string newPassword; // getting new password
                            while (true)
                            {
                                Console.WriteLine("print new password");
                                newPassword = Console.ReadLine();
                                Console.WriteLine("confirm new password");
                                if (Console.ReadLine() == newPassword)
                                {
                                    break;
                                }
                                Console.WriteLine("passwords isn't similar");
                            }

                            Console.WriteLine("print admin password"); // getting admin password
                            var aPassword = Console.ReadLine();

                            try  // changing password
                            {
                                account.ChangePassword(newPassword, AdminAccount, aPassword);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return;
                            }
                            break;
                        }

                        case "AccessRights":
                        {
                            AccessRights accessRights;
                            Console.WriteLine(
                                "To change AccessRights" + Environment.NewLine +
                                "User" + Environment.NewLine +
                                "VipUser" + Environment.NewLine +
                                "Moder" + Environment.NewLine +
                                "Admin"
                            );

                            switch (Console.ReadLine()) // getting access rights
                            {
                                case "User":
                                {
                                    accessRights = AccessRights.User;
                                    break;
                                }

                                case "VipUser":
                                {
                                    accessRights = AccessRights.VipUser;
                                    break;
                                }

                                case "Moder":
                                {
                                    accessRights = AccessRights.Moder;
                                    break;
                                }

                                case "Admin":
                                {
                                    accessRights = AccessRights.Admin;
                                    break;
                                }

                                default:
                                {
                                    Console.WriteLine("wrong access rights");
                                    return;
                                }
                            }

                            try // changing access rights
                            {
                                Console.WriteLine("print admin password");
                                string aPassword = Console.ReadLine();
                                account.ChangeAccessRights(accessRights, AdminAccount, aPassword);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return;
                            }
                            break;
                        }

                        case "Money":
                        {
                            decimal sum; // getting sum, converting to decimal
                            while (true)
                            {
                                try
                                {
                                    Console.WriteLine("print sum");
                                    sum = Convert.ToDecimal(Console.ReadLine());
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("not a number");
                                }
                            }

                            Console.WriteLine("print admin password");
                            string aPassword = Console.ReadLine();

                            if (sum > 0) // refill/withdraw
                            {
                                try
                                {
                                    account.Refill(sum, AdminAccount, aPassword);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    return;
                                }
                            }
                            else
                            {
                                try
                                {
                                    account.Withdraw(sum, AdminAccount, aPassword);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    return;
                                }
                            }

                            break;
                        }

                        default:
                        {
                            Console.WriteLine("there is no option");
                            return;
                        }
                    }

                    try // updating data
                    {
                        DataForWrappers.Shop.UpdateAccount(account);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }

                    break;
                }

                case "goods":
                {
                    ShopLot shopLot; // loading shop lot
                    try
                    {
                        shopLot = DataForWrappers.Shop.GetShopLot(TerminalCommand[2]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    if (shopLot == null) // checking for existing
                    {
                        Console.WriteLine("Item doesn't exists");
                        return;
                    }
                    
                    Console.WriteLine("What parameter do you want to change");
                    Console.WriteLine("You can change:" + Environment.NewLine +
                                      "Picture" + Environment.NewLine +
                                      "Price" + Environment.NewLine +
                                      "Description - About");
                    switch (Console.ReadLine())
                    {
                        case "Picture":
                        {
                            string picturePath; // getting new picture
                            Console.WriteLine("Chose new picture");
                            picturePath = Console.ReadLine();
                            try
                            {
                                shopLot.EditPicture(picturePath);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return;
                            }
                            break;
                        }

                        case "Price":
                        {
                            decimal price; // getting price and convert to decimal
                            Console.WriteLine("Print new price");
                            while (true)
                            {
                                try
                                {
                                    Console.WriteLine("print sum");
                                    price = Convert.ToDecimal(Console.ReadLine());
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("not a number");
                                }
                            }

                            try // changing price
                            {
                                shopLot.EditPrice(price);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return;
                            }
                            break;
                        }

                        case "About":
                        {
                            string about;
                            Console.WriteLine("Print new description");
                            about = Console.ReadLine();
                            try // changing about
                            {
                                shopLot.EditAbout(about);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return;
                            }
                            break;
                        }
                    }

                    try // updating data
                    {
                        DataForWrappers.Shop.UpdateShopLot(shopLot);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }

                    break;
                }

                default:
                {
                    Console.WriteLine("wrong argument");
                    return;
                }
            }

            Console.WriteLine("operation is done");
        }

        private static void Inspect()
        {
            switch (TerminalCommand[1])
            {
                case "account":
                {
                    if (TerminalCommand[2] == null) // if no currently account show all
                    {
                        List<string> accs = DataForWrappers.Shop.GetAccounts();
                        foreach (var acc in accs)
                        {
                            Console.WriteLine(acc);
                        }
                        return;
                    }

                    Account account;
                    try // loading account
                    {
                        account = DataForWrappers.Shop.GetAccount(TerminalCommand[2]);
                    }
                    catch (Exception e) 
                    {
                        Console.WriteLine(e.GetType());
                        return;
                    }
                    if (account == null) // checking for existing
                    {
                        Console.WriteLine("account doesn't exists");
                        return;
                    }

                    Console.WriteLine(account.ToString()); // show to user
                    break;
                }

                case "goods":
                {
                    if (TerminalCommand[2] == null) // if no currently goods show all
                    {
                        List<ShopLot> lots = DataForWrappers.Shop.GetShopLots();
                        foreach (var lot in lots)
                        {
                            Console.WriteLine(lot + Environment.NewLine);
                        }
                        return;
                    }

                    ShopLot shopLot;
                    try // loading shop lot
                    {
                        shopLot = DataForWrappers.Shop.GetShopLot(TerminalCommand[2]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    if (shopLot == null) // checking for existing
                    {
                        Console.WriteLine("Item doesn't exists");
                        return;
                    }

                    Console.WriteLine(shopLot.ToString());
                    
                    break;
                }

                default:
                {
                    Console.WriteLine("wrong argument");
                    return;
                }
            }
        }

        private static void Stop()
        {
            Server.Abort();
            Console.WriteLine("Stopping server");
        }

        private static void Exit()
        {
            while (true)
            {
                Console.WriteLine("are you sure (yes/no)");
                switch (Console.ReadLine())
                {
                    case "yes":
                    {
                        Stop();
                        // saving shop
                        using (FileStream fs = new FileStream($"{DataForWrappers.Shop.Name}.shop", FileMode.Create))
                        {
                            DataForWrappers.Shop.SaveShop(fs);
                        }

                        Environment.Exit(0);
                        break;
                    }

                    case "no":
                    {
                        return;
                    }
                }
            }
        }

        private static void Help()
        {
            List<string> help = new List<string> { "start", "manage", "stop", "exit" };

            Console.WriteLine();

            if (TerminalCommand.Length == 1)
            {
                foreach (var h in help)
                {
                    Console.WriteLine(h);
                }

                Console.WriteLine("you can get additional option by adding arguments to command");
            }

            else
            {
                switch (TerminalCommand[1])
                {
                    case "start":
                    {
                        Console.WriteLine("will start server");
                        break;
                    }

                    case "manage":
                    {
                        Console.WriteLine(
                            "first argument - type of item:" + Environment.NewLine +
                            "to choose account print - account" + Environment.NewLine +
                            "to choose goods print - goods" + Environment.NewLine +
                            "--- --- --- --- --- --- ---" + Environment.NewLine +
                            "second argument - name of item" + Environment.NewLine +
                            "--- --- --- --- --- --- ---" + Environment.NewLine +
                            "third argument - type of action:" + Environment.NewLine +
                            "to add new item print - add" + Environment.NewLine +
                            "to edit item print - edit" + Environment.NewLine +
                            "to see item print - inspect"
                        );
                        break;
                    }

                    case "stop":
                    {
                        Console.WriteLine("will stop server");
                        break;
                    }

                    case "exit":
                    {
                        Console.WriteLine("exit has no arguments");
                        break;
                    }

                    default:
                    {
                        Console.WriteLine("wrong argument");
                        break;
                    }
                }
            }

            Console.WriteLine();
        }
    }
}