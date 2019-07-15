using System.Collections.Generic;
using System.Timers;
using MyShopServerMain.core.shop;
using System.Net;

namespace MyShopServerMain.core.server
{
    internal class ConnectionsHolder
    {
        private List<ConnectionHolder> connectionHolders;

        private class ConnectionHolder
        {
            private Account _account;
            private Timer _timer;
            private SocketAddress _socketAddress;

            internal ConnectionHolder(Account tAccount, SocketAddress socketAddress)
            {
                _account = tAccount;
                _socketAddress = socketAddress;
                _timer = new Timer { Interval = 1800000 }; //half hour
                _timer.Start();
                _timer.Elapsed += Server.OnDispose;
            }
        }

        internal ConnectionsHolder()
        {
            connectionHolders = new List<ConnectionHolder>();

        }

        internal void AddConnection(Account tAccount, SocketAddress socketAddress)
        {
            ConnectionHolder connectionHolder = new ConnectionHolder(tAccount, socketAddress);
            connectionHolders.Add(connectionHolder);
        }
    }
}