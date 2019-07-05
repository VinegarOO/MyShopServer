using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShopServer;

namespace MyShopServerMain
{
    public static class UserInterface
    {
        public static void Menu()
        {
            
            while (true)
            {
                Dictionary<string, Action> comands = new Dictionary<string, Action>
                {
                    {"start", new Action(Start) },
                    {"exit", new Action(Exit) },
                    {"add", new Action(Add) },
                    {"edit", new Action(Edit) },
                    {"inspect", new Action(Inspect) },
                    {"stop", new Action(Stop) },
                    {"help", new Action(Help) }
                };
                Console.WriteLine("Waiting for comand" + Environment.NewLine);
                string[] comand = Console.ReadLine().Split();

                if (comands.ContainsKey(comand[0]))
                {
                    comands[comand[0]]();
                }
                else
                {
                    Console.WriteLine("Not a comand");
                }
            }
        }

        private static void Start()
        {
            Console.WriteLine("Starting server");
        }

        private static void Exit()
        {
            Console.WriteLine("Closing console");
        }

        private static void Add()
        {
            Console.WriteLine("Adding new lot to the shop");
        }

        private static void Edit()
        {
            Console.WriteLine("Changing a lot into the shop");
        }

        private static void Inspect()
        {
            Console.WriteLine("It is info");
        }

        public static void Stop()
        {
            Console.WriteLine("Stoping server");
        }

        public static void Help()
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