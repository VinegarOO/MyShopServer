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

        internal static void SendAnswer(IPAddress client, string message)
        {
            byte[] messageBytes = DataForWrappers.encoding.GetBytes(message);


        }

        internal static void SendAnswer(IPAddress client, string message, string image)
        {
            byte[] messageBytes = DataForWrappers.encoding.GetBytes(message);

            messageBytes = messageBytes.Concat(CompareImage(image)).ToArray();


        }

        internal static void SendAnswer(IPAddress client, string message, List<string> images)
        {
            byte[] messageBytes = DataForWrappers.encoding.GetBytes(message);

            IEnumerable<byte> b_images = new byte[0];

            foreach (var image in images)
            {
                b_images = b_images.Concat(CompareImage(image));
            }

            messageBytes = messageBytes.Concat(b_images).ToArray();


        }

        internal static IEnumerable<byte> CompareImage(string image)
        {
            byte[] img_start = DataForWrappers.encoding.GetBytes("<image>");
            byte[] img_end = DataForWrappers.encoding.GetBytes("</image>");
            byte[] img = File.ReadAllBytes(image);

            return img_start.Concat(img).Concat(img_end);
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
