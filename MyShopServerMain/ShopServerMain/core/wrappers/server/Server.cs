using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ShopServerMain.core.wrappers.server
{
    static class Server
    {
        internal static void WaitingConnection()
        {
            UdpClient rUdpClient = new UdpClient(DataForWrappers.ServerPort);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(DataForWrappers.ServerIpAddres)
                , DataForWrappers.ServerPort);

            while (DataForWrappers.ServerWorkingFlag)
            {
                // getting request
                byte[] request = rUdpClient.Receive(ref endPoint);

                // process request
                var t = new RequestHolder(endPoint, request);

                DataForWrappers.Requests.Enqueue(t);
            }
        }

        internal static void SendingAnswer()
        {
            UdpClient aUdpClient = new UdpClient();

            while (DataForWrappers.ServerWorkingFlag)
            {
                AnswerHolder answer;
                
                while (!DataForWrappers.Answers.TryDequeue(out answer))
                {
                    Thread.Sleep(10);
                }
                
                // Sending answer
                aUdpClient.Send(answer.Answer, answer.Answer.Length, answer.Client);
            }
        }

        public static void Send(IPEndPoint client, byte[] message)
        {
            DataForWrappers.Answers.Enqueue(new AnswerHolder(client, message));
        }
    }
}
