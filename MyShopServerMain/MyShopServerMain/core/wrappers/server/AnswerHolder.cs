using System.Net;

namespace MyShopServerMain.core.wrappers.server
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