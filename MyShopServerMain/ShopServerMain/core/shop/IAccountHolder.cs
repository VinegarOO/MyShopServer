using System.Collections.Generic;

namespace ShopServerMain.core.shop
{
    internal interface IAccountHolder
    {
        void AddAccount(Account account);

        void RemoveAccount(Account account);

        List<string> GetAccounts();

        Account GetAccount(string name);
    }
}