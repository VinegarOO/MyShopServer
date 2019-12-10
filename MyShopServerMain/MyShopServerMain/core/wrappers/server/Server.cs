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
                var t = new RequestHolder(endPoint, request);

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
                aSocket.SendTo(BitConverter.GetBytes(answer.Answer.Length), answer.Client);
                Thread.Sleep(10);
                aSocket.SendTo(answer.Answer, answer.Client);
            }
        }

        public static void Send(EndPoint client, byte[] message)
        {
            DataForWrappers.Answers.Enqueue(new AnswerHolder(client, message));
        }
    }
}
