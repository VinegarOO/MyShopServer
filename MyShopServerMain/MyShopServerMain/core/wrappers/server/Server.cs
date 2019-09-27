using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyShopServerMain.core.shop;
using static MyShopServerMain.core.wrappers.server.RequestsProcessor;

namespace MyShopServerMain.core.wrappers.server
{
    static class Server
    {
        internal static void WaitingConnection()
        {
            while (true)
            {
                // Get connection

                IPAddress client = null;
                byte[] request = null;

                // process request
                var t = new RequestHolder(client, request);

                ThreadPool.QueueUserWorkItem(ProcessRequest, t);
            }
        }

        private static void Send(IPAddress client, byte[] message)
        {
            // sending answer
        }

        internal static void SendAnswer(IPAddress client, string message)
        {
            byte[] messageBytes = DataForWrappers.Encoding.GetBytes(message);

            Send(client, messageBytes);
        }

        internal static void SendAnswer(IPAddress client, string message, string image)
        {
            byte[] messageBytes = DataForWrappers.Encoding.GetBytes(message);

            messageBytes = messageBytes.Concat(CompareImage(image)).ToArray();

            Send(client, messageBytes);
        }

        internal static void SendAnswer(IPAddress client, string message, List<string> images)
        {
            byte[] messageBytes = DataForWrappers.Encoding.GetBytes(message);

            IEnumerable<byte> bImages = new byte[0];

            foreach (var image in images)
            {
                bImages = bImages.Concat(CompareImage(image));
            }

            messageBytes = messageBytes.Concat(bImages).ToArray();

            Send(client, messageBytes);
        }

        internal static IEnumerable<byte> CompareImage(string image)
        {
            byte[] imgStart = DataForWrappers.Encoding.GetBytes("<image>");
            byte[] imgEnd = DataForWrappers.Encoding.GetBytes("</image>");
            byte[] img = File.ReadAllBytes(image);

            return imgStart.Concat(img).Concat(imgEnd);
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
