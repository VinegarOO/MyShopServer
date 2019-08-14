using System;
using System.Drawing;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using MyShopServerMain.core.shop;
using static MyShopServerMain.core.wrappers.server.ConnectionsHolder;

namespace MyShopServerMain.core.wrappers.server
{
    static class Server
    {
        internal static void WaitingCommand(Shop shop)
        {

        }

        internal static void WaitingConnection(Shop shop)
        {
            while (true)
            {
                // Get connection
                // Check LogIn
                // Add connection
                // Send port
                // Send token
            }
        }

        internal static void SendAnswer(IPAddress client, string message)
        {
            byte[] messageBytes;
            Encoding encoding = new UTF8Encoding();
            messageBytes = encoding.GetBytes(message);


        }

        internal static void SendAnswer(IPAddress client, string message, Image image)
        {
            byte[] messageBytes;
            Encoding encoding = new UTF8Encoding();
            messageBytes = encoding.GetBytes(message);


        }

        internal static string CreateAnswer(string theme, string text)
        {
            string result = string.Empty;
            result += $"<head><title>{DataForWrappers.Shop.Name}</title></head>";
            result += Environment.NewLine;
            string[] texts = text.Split(Environment.NewLine.ToCharArray());
            result += $"<body><h1>{theme}</h1>";
            foreach (var temp in texts)
            {
                result += $"<h2>{temp}</h2>";
            }
            result += "</body";
            return result;
        }
    }
}
