using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ShopServerMain.core.wrappers;
using ShopServerMain.core.wrappers.DB;

namespace ShopServerMain.core.shop
{
    [Serializable]
    internal class Shop : IShop, IAccountHolder
    {
        public void AddShopLot(ShopLot lot)
        {
            if (GetShopLot(lot.Name) != null)
            {
                var temp = lot.Save();
                if (temp.Length > DataForWrappers.MaxSizeOfShopLot)
                {
                    throw new ArgumentException("Image too large");
                }
                bool result = MyDb.AddData(temp, lot.Name, "ShopLot");
                if (!result)
                {
                    throw new ArgumentException("Goods is already exists");
                }
            }
            else
            {
                throw new ArgumentNullException(lot.Name);
            }
        }

        public void UpdateShopLot(ShopLot shopLot)
        {
            if (!MyDb.UpdateData(shopLot.Save(), shopLot.Name, "ShopLot"))
            {
                throw new ArgumentException("Goods is already exists");
            }
        }

        public void RemoveShopLot(ShopLot lot)
        {
            if(!MyDb.RemoveData(lot.Name, "ShopLot"))
            {
                throw new ArgumentException("Goods is not exists");
            }
        }

        public List<ThumbGoods> GetShopLots()
        {
            List<string> listOfData = MyDb.GetListOfData("ShopLot");
            
            List<ThumbGoods> result = new List<ThumbGoods>();

            foreach (var data in listOfData)
            {
                var t = GetShopLot(data);
                result.Add(t.GetThumbGoods());
            }
            
            return result;
        }

        public ShopLot GetShopLot(string name)
        {
            var buffer = MyDb.GetData(name, "ShopLot");
            ShopLot result = new ShopLot("null", "null", "null", 1);
            result.Load(buffer);
            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;
            foreach(var t in MyDb.GetListOfData("ShopLot"))
            {
                result += (t + Environment.NewLine);
            }
            foreach(var t in MyDb.GetListOfData("Account"))
            {
                result += (t + Environment.NewLine);
            }
            return result;
        }

        public void AddAccount(Account account)
        {
            if (account.Name == null) // check account name for null
            {
                throw new ArgumentNullException(account.Name, "Name can't be null");
            }

            if (!MyDb.AddData(account.Save(), account.Name, "Account")) // check account name for exist
            {
                throw new ArgumentException("This account name already exists");
            }
        }

        public void UpdateAccount(Account account)
        {
            if (!MyDb.UpdateData(account.Save(), account.Name, "Account"))
            {
                throw new ArgumentException("Account is already exists");
            }
        }

        public void RemoveAccount(Account account)
        {
            if(!MyDb.RemoveData(account.Name, "Account"))
            {
                throw new ArgumentException("Account is not exists", "account");
            }
        }

        public List<string> GetAccounts()
        {
            return MyDb.GetListOfData("Account");
        }

        public Account GetAccount(string name)
        {
            var buffer = MyDb.GetData(name, "Account");
            Account result = new Account("null", "null");
            result.Load(buffer);
            return result;
        }
    }
}