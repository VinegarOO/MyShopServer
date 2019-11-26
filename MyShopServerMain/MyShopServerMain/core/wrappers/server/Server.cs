using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyShopServerMain.core.wrappers.server
{
    static class Server
    {
        internal static void WaitingConnection()
        {
            Socket rSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(DataForWrappers.ServerIPAddres)
                , DataForWrappers.ServerPort);
            rSocket.Bind(endPoint);

            while (true)
            {
                // getting request
                byte[] request = new byte[1024];
                rSocket.ReceiveFrom(request, ref endPoint);

                // process request
                var t = new RequestHolder(new IPEndPoint(((IPEndPoint)endPoint).Address,
                    ((IPEndPoint)endPoint).Port + 1), request);

                DataForWrappers.Requests.Enqueue(t);
            }
        }

        internal static void SendingAnswer()
        {
            Socket aSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while (true)
            {
                AnswerHolder answer;
                
                while (!DataForWrappers.Answers.TryDequeue(out answer))
                {
                    Thread.Sleep(10);
                }
                
                // Sending answer
                aSocket.SendTo(answer.Answer, answer.Client);
            }
        }

        private static void Send(EndPoint client, byte[] message)
        {
            DataForWrappers.Answers.Enqueue(new AnswerHolder(client, message));
        }

        internal static void SendAnswer(EndPoint client, string message)
        {
            byte[] messageBytes = DataForWrappers.Encoding.GetBytes(message);

            Send(client, messageBytes);
        }

        internal static void SendAnswer(EndPoint client, string message, Image image)
        {
            byte[] messageBytes = DataForWrappers.Encoding.GetBytes(message);

            messageBytes = messageBytes.Concat(CompareImage(image)).ToArray();

            Send(client, messageBytes);
        }

        internal static void SendAnswer(EndPoint client, string message, List<Image> images)
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

        internal static IEnumerable<byte> CompareImage(Image image)
        {
            byte[] imgStart = DataForWrappers.Encoding.GetBytes("<image>");
            byte[] imgEnd = DataForWrappers.Encoding.GetBytes("</image>");
            byte[] img;
            if (image == null)
            {
                img = new byte[1];
            }
            else
            {
                ImageConverter ic = new ImageConverter();
                img = (byte[])ic.ConvertTo(image, typeof(byte[]));
            }

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
