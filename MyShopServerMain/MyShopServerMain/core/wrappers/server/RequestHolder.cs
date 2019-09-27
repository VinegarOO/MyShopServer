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
        internal readonly IPAddress Client = null;
        internal readonly byte[] Request = null;
        internal string[] Command;

        internal RequestHolder(IPAddress tClient, byte[] tRequest)
        {
            Client = tClient;
            Request = tRequest;
        }
    }
}
