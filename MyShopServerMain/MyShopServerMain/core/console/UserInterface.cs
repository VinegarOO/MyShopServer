using System;
using System.Collections.Generic;
using MyShopServerMain.core.shop;

namespace MyShopServerMain.core.console
{
    public static class UserInterface
    {
        delegate void CommandDelegate(string[] command);

        public static void Menu()
        {
            Account console = new Account("Admin", "qwerty", AccessRights.Admin);
            while (true)
            {
                Console.WriteLine("enter password");
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
            while (true)
            {
                Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>
                {
                    {"start", Start },
                    {"exit", Exit },
                    {"add", Add },
                    {"edit", Edit },
                    {"inspect", Inspect },
                    {"stop", Stop },
                    {"help", Help }
                };
                Console.WriteLine("Waiting for command" + Environment.NewLine);
                string[] command = Console.ReadLine().Split();

                if (commands.ContainsKey(command[0]))
                {
                    commands[command[0]](command);
                }
                else
                {
                    Console.WriteLine("Not a command");
                }
            }
        }

        private static void Start(string[] command)
        {
            Console.WriteLine("Starting server");
        }

        private static void Exit(string[] command)
        {
            Console.WriteLine("Closing console");
        }

        private static void Add(string[] command)
        {
            Console.WriteLine("Adding new lot to the shop");
        }

        private static void Edit(string[] command)
        {
            Console.WriteLine("Changing a lot into the shop");
        }

        private static void Inspect(string[] command)
        {
            Console.WriteLine("It is info");
        }

        private static void Stop(string[] command)
        {
            Console.WriteLine("Stoping server");
        }

        private static void Help(string[] command)
        {
            List<string> help = new List<string> { "start - starting server", "add - add new lot",
                "edit - edit lot", "exit - close console", "stop - stop the server",
                "inspect - show info about lot" };

            foreach (var h in help)
            {
                Console.WriteLine(h);
            }

            Console.WriteLine(Environment.NewLine);
        }
    }
}