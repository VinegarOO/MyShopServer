using System.Net;

namespace ShopServerMain.core.wrappers.server
{
    public class AnswerHolder
    {
        internal readonly EndPoint Client = null;
        internal readonly byte[] Answer = null;

        internal AnswerHolder(EndPoint tClient, byte[] tAnswer)
        {
            Client = tClient;
            Answer = tAnswer;
        }
    }
}