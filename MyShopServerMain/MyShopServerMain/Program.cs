using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShopServerMain
{
    class Program
    {
        static void Main(string[] args)
        {
            List<String> help = new List<String> { "start - starting server", "add - add new lot",
                "edit - edit lot", "exit - close console", "stop - stop the server",
                "inspect - show info about lot" };
            while (true)
            {
                Console.WriteLine("Waiting for comand");
                String[] comand = Console.ReadLine().Split();
                switch (comand[0])
                {
                    case "start":
                        Console.WriteLine("Starting server");
                        break;
                    case "exit":
                        Console.WriteLine("Closing console");
                        return;
                    case "add":
                        Console.WriteLine("Adding new lot to the shop");
                        break;
                    case "edit":
                        Console.WriteLine("Changing a lot into the shop");
                        break;
                    case "inspect":
                        Console.WriteLine("It is info");
                        break;
                    case "stop":
                        Console.WriteLine("Stoping server");
                        break;
                    case "help":
                        foreach (var h in help)
                        {
                            Console.WriteLine(h);
                        }
                        break;
                    default:
                        Console.WriteLine("Not a comand");
                        break;
                }
            }
        }
    }
}
