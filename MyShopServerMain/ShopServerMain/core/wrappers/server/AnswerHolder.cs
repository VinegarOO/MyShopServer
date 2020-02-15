using System.Net;

namespace ShopServerMain.core.wrappers.server
{
    public class AnswerHolder
    {
        internal readonly IPEndPoint Client = null;
        internal readonly byte[] Answer = null;

        internal AnswerHolder(IPEndPoint tClient, byte[] tAnswer)
        {
            Client = tClient;
            Answer = tAnswer;
        }
    }
}