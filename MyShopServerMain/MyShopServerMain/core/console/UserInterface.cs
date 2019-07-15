using MyShopServerMain.core.shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyShopServerMain.core.console
{
    public static class UserInterface
    {
        delegate void CommandDelegate(string[] command, Shop shop, Account adminAccount);

        public static void Menu()
        {
            Account console = new Account("Admin", "qwerty", AccessRights.Admin);
            while (true)
            {
                Console.WriteLine("enter password");
                Console.Write(">> ");
                string passwd = Console.ReadLine();
                if (console.Verify(passwd))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("wrong password");
                }
            }

            Shop shop = null; // load or create shop

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
                                shop = (Shop) bf.Deserialize(fs);
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
                                shop = new Shop(Console.ReadLine());

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

            Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>
            {
                {"start", Start },
                {"exit", Exit },
                {"manage", Manage },
                {"stop", Stop },
                {"help", Help }
            };
            while (true)
            {
                Console.WriteLine("Waiting for command");
                Console.Write("Admin >> ");
                string[] command = Console.ReadLine().Split();

                if (commands.ContainsKey(command[0]))
                {
                    commands[command[0]](command, shop, console);
                }
                else
                {
                    Console.WriteLine("Not a command");
                }
            }
        }

        private static void Start(string[] command, Shop shop, Account adminAccount)
        {
            Console.WriteLine("Starting server");
        }

        private static void Manage(string[] command, Shop shop, Account adminAccount)
        {
            if (command.Length < 3)
            {
                Console.WriteLine("wrong arguments");
                return;
            }

            switch (command[3])
            {
                case "add":
                {
                    Add(command, shop);
                    break;
                }

                case "edit":
                {
                    Edit(command, shop, adminAccount);
                    break;
                }

                case "inspect":
                {
                    Inspect(command, shop);
                    break;
                }

                default:
                {
                    Console.WriteLine("i can't do it");
                    return;
                }
            }
        }

        private static void Add(string[] command, Shop shop)
        {
            switch (command[1])
            {
                case "account":
                {
                    string password;
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

                    Account account = new Account(command[2], password);

                    try
                    {
                        shop.AddAccount(account);
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
                    string picturePath;
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

                    Console.WriteLine("print goods description");
                    string about = Console.ReadLine();

                    decimal price;
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

                    string[] tags;
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

                    ShopLot shopLot = new ShopLot(command[2], picturePath, about, price, tags);

                    try
                    {
                        shop.AddShopLot(shopLot);
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

            Console.WriteLine("Item added");
        }

        private static void Edit(string[] command, Shop shop, Account adminAccount)
        {
            switch (command[1])
            {
                case "account":
                {
                    Account account;
                    try
                    {
                        account = shop.GetAccount(command[2]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.GetType());
                        return;
                    }
                    if (account == null)
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
                            string newPassword;
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

                            Console.WriteLine("print admin password");
                            var aPassword = Console.ReadLine();

                            try
                            {
                                account.ChangePassword(newPassword, adminAccount, aPassword);
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

                            switch (Console.ReadLine())
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

                            try
                            {
                                Console.WriteLine("print admin password");
                                string aPassword = Console.ReadLine();
                                account.ChangeAccessRights(accessRights, adminAccount, aPassword);
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
                            decimal sum;
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

                            if (sum > 0)
                            {
                                try
                                {
                                    account.Refill(sum, adminAccount, aPassword);
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
                                    account.Withdraw(sum, adminAccount, aPassword);
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

                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fs = new FileStream($"{account.Name}.acc", FileMode.Create))
                    {
                        bf.Serialize(fs, account);
                    }

                    break;
                }

                case "goods":
                {
                    ShopLot shopLot;
                    try
                    {
                        shopLot = shop.GetShopLot(command[2]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    if (shopLot == null)
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
                            string picturePath;
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
                            decimal price;
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

                            try
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
                            try
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

                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fs = new FileStream($"{shopLot.Name}.safer", FileMode.Create))
                    {
                        bf.Serialize(fs, shopLot);
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

        private static void Inspect(string[] command, Shop shop)
        {
            switch (command[1])
            {
                case "account":
                {
                    if (command[2] == null)
                    {
                        List<string> accs = shop.GetAccounts();
                        foreach (var acc in accs)
                        {
                            Console.WriteLine(acc);
                        }
                        return;
                    }

                    Account account;
                    try
                    {
                        account = shop.GetAccount(command[2]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.GetType());
                        return;
                    }
                    if (account == null)
                    {
                        Console.WriteLine("account doesn't exists");
                        return;
                    }

                    Console.WriteLine(account.ToString());
                    break;
                }

                case "goods":
                {
                    if (command[2] == null)
                    {
                        List<string> lots = shop.GetShopLotsList();
                        foreach (var lot in lots)
                        {
                            Console.WriteLine(lot);
                        }
                        return;
                    }

                    ShopLot shopLot;
                    try
                    {
                        shopLot = shop.GetShopLot(command[2]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                    if (shopLot == null)
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

        private static void Stop(string[] command, Shop shop, Account adminAccount)
        {
            Console.WriteLine("Stopping server");
        }

        private static void Exit(string[] command, Shop shop, Account adminAccount)
        {
            while (true)
            {
                Console.WriteLine("are you sure (yes/no)");
                switch (Console.ReadLine())
                {
                    case "yes":
                    {
                        Stop(new[] { "", "" }, shop, adminAccount);
                        using (FileStream fs = new FileStream($"{shop.name}.shop", FileMode.Create))
                        {
                            shop.SaveShop(fs);
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

        private static void Help(string[] command, Shop shop, Account adminAccount)
        {
            List<string> help = new List<string> { "start", "manage", "stop", "exit" };

            Console.WriteLine();

            if (command.Length == 1)
            {
                foreach (var h in help)
                {
                    Console.WriteLine(h);
                }

                Console.WriteLine("you can get additional option by adding arguments to command");
            }

            else
            {
                switch (command[1])
                {
                    case "start":
                    {
                        Console.WriteLine(
                            "" + Environment.NewLine +
                            ""
                        );
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
                        Console.WriteLine(
                            "" + Environment.NewLine +
                            ""
                        );
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