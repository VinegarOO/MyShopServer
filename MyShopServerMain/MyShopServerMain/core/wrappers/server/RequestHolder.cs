using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyShopServerMain.core.wrappers.server
{
    internal class RequestHolder
    {
        internal readonly IPAddress client = null;
        internal readonly byte[] request = null;
        internal string[] command;

        internal RequestHolder(IPAddress t_client, byte[] t_request)
        {
            client = t_client;
            request = t_request;
        }
    }
}
