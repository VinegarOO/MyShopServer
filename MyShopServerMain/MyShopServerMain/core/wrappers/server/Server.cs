using MyShopServerMain.core.shop;
using static MyShopServerMain.core.wrappers.server.ConnectionsHolder;

namespace MyShopServerMain.core.wrappers.server
{
    static class Server
    {
        internal static void WaitingForCommand(Shop shop)
        {

        }

        internal static void WaitingConnection(Shop shop)
        {
            while (true)
            {
                // Get connection
                // Check LogIn
                // Add connection
                // Send port
                // Send token
            }
        }
    }
}
