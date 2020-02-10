using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ShopServerMain.core.wrappers.DB;

namespace ShopServerMain.core.shop
{
    [Serializable]
    internal class Shop : IShop, IAccountHolder
    {
        internal readonly string Name;

        public Shop(string tName)
        {
            if (tName == null)
            {
                throw new NullReferenceException("Name can't be null");
            }

            if (tName == string.Empty)
            {
                throw new NullReferenceException("Name can't be blank");
            }
            Name = tName;
        }

        public void AddShopLot(ShopLot lot)
        {
            if (this.GetShopLot(lot.Name) != null)
            {
                bool result = MyDb.AddData(lot, null);
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
            if (!MyDb.UpdateData(shopLot, shopLot.Name))
            {
                throw new ArgumentException("Goods is already exists");
            }
        }

        public void RemoveShopLot(ShopLot lot)
        {
            if(!MyDb.RemoveData(lot, lot.Name))
            {
                throw new ArgumentException("Goods is not exists");
            }
        }

        public List<ThumbGoods> GetShopLots()
        {
            List<string> listOfData = MyDb.GetListOfData(typeof(ShopLot));
            
            List<ThumbGoods> result = new List<ThumbGoods>();

            foreach (var data in listOfData)
            {
                var t = MyDb.GetData<ShopLot>(data);
                result.Add(new ThumbGoods(t.Name, t.Picture));
            }
            
            return result;
        }

        /*public List<ShopLot> GetShopLots(string[] tags)
        {
            List<ShopLot> result = new List<ShopLot>();
            foreach (var temp in MyDb.GetListOfData(typeof(ShopLot))) // checking all lots
            {
                ShopLot tLot = GetShopLot(temp);
                foreach (var tag in tags) // checking all tags
                {
                    if (tLot.Tags.Contains(tag)) // checking for contains tag
                    {
                        result.Add(tLot);
                        break;
                    }
                }
            }
            return result;
        }*/

        public void SaveShop(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, this);
        }

        public ShopLot GetShopLot(string name)
        {
            return MyDb.GetData<ShopLot>(name);
        }

        public override string ToString()
        {
            string result = string.Empty;
            foreach(var t in MyDb.GetListOfData(typeof(ShopLot)))
            {
                result += (t + Environment.NewLine);
            }
            foreach(var t in MyDb.GetListOfData(typeof(Account)))
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

            if (!MyDb.AddData(account, account.Name)) // check account name for exist
            {
                throw new ArgumentException("This account name already exists");
            }
        }

        public void UpdateAccount(Account account)
        {
            if (!MyDb.UpdateData(account, account.Name))
            {
                throw new ArgumentException("Account is already exists");
            }
        }

        public void RemoveAccount(Account account)
        {
            if(!MyDb.RemoveData(account, account.Name))
            {
                throw new ArgumentException("Account is not exists", "account");
            }
        }

        public List<string> GetAccounts()
        {
            return MyDb.GetListOfData(typeof(Account));
        }

        /*public List<string> GetAccounts(AccessRights accessRights) // finding account by access rights
        {
            List<string> result = new List<string>();
            foreach (var tAcc in _accounts) // search in all accounts
            {
                if (File.Exists($"{tAcc}.acc")) // if account exists
                {
                    Account tempAccount = GetAccount($"{tAcc}.acc");
                    if (tempAccount.AccessRight == accessRights) // if access rights is correct
                    {
                        result.Add(tAcc);
                    }
                }
            }
            return result;
        }*/

        public Account GetAccount(string name)
        {
            Account result = MyDb.GetData<Account>(name);
            
            return result;
        }
    }
}