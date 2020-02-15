using System.Net;

namespace ShopServerMain.core.wrappers.server
{
    internal class RequestHolder
    {
        internal readonly IPEndPoint Client = null;
        internal readonly byte[] Request = null;
        internal string[] Command;

        internal RequestHolder(IPEndPoint tClient, byte[] tRequest)
        {
            Client = tClient;
            Request = tRequest;
        }
    }
}
