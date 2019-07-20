using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using MyShopServerMain.core.shop;

namespace MyShopServerMain.core.wrappers.server
{
    internal static class ConnectionsHolder
    {
        private static List<ConnectionHolder> _connectionHolders;

        private class ConnectionHolder
        {
            internal readonly Account Account;
            private DateTime _lastAction;
            internal readonly IPAddress IpAddress;

            internal ConnectionHolder(Account tAccount, IPAddress ipAddress)
            {
                Account = tAccount;
                IpAddress = ipAddress;
                _lastAction = DateTime.Now;
            }

            internal bool IsShouldBeDeleted()
            {
                if ((DateTime.Now - _lastAction) > new TimeSpan(0, 30, 0))
                {
                    return true;
                }

                return false;
            }

            internal Account GetAccount()
            {
                _lastAction = DateTime.Now;
                return Account;
            }
        }

        static ConnectionsHolder()
        {
            _connectionHolders = new List<ConnectionHolder>();

        }

        internal static Account FindConnection(SocketAddress address)
        {
            foreach (var connection in _connectionHolders)
            {
                if (connection.IpAddress.Equals(address))
                {
                    return connection.Account;
                }
            }
            throw new ArgumentException("connection not found");
        }

        internal static void CheckingConnection() // with minimal priority
        {
            while (true)
            {
                List<ConnectionHolder> connectionHoldersRemoveList = new List<ConnectionHolder>();
                foreach (var connection in _connectionHolders)
                {
                    if (connection.IsShouldBeDeleted())
                    {
                        connectionHoldersRemoveList.Add(connection);
                    }
                }

                foreach (var connectionToRemove in connectionHoldersRemoveList)
                {
                    _connectionHolders.Remove(connectionToRemove);
                }
                Thread.Sleep(30000); // half minute
            }
        }

        internal static void AddConnection(Account tAccount, IPAddress ipAddress)
        {
            ConnectionHolder connectionHolder = new ConnectionHolder(tAccount, ipAddress);
            _connectionHolders.Add(connectionHolder);
        }
    }
}