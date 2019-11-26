using System.Net;

namespace MyShopServerMain.core.wrappers.server
{
    internal class RequestHolder
    {
        internal readonly EndPoint Client = null;
        internal readonly byte[] Request = null;
        internal string[] Command;

        internal RequestHolder(EndPoint tClient, byte[] tRequest)
        {
            Client = tClient;
            Request = tRequest;
        }
    }
}
