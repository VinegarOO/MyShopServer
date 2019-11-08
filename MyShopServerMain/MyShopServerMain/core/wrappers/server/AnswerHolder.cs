using System.Net;

namespace MyShopServerMain.core.wrappers.server
{
    public class AnswerHolder
    {
        internal readonly IPAddress Client = null;
        internal readonly byte[] Answer = null;

        internal AnswerHolder(IPAddress tClient, byte[] tAnswer)
        {
            Client = tClient;
            Answer = tAnswer;
        }
    }
}