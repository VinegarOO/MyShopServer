using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyShopServerMain.core.shop
{
    [Serializable]
    internal class Shop : IShop, IAccountHolder
    {
        private List<string> _lots;
        private List<string> _accounts;
        internal readonly string Name;

        public Shop(string tName)
        {
            _lots = new List<string>();
            _accounts = new List<string>();

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
                if (!_lots.Contains(lot.Name)) // checking for exists
                {
                    SaveShopLot(lot);
                    _lots.Add(lot.Name);
                }
                else
                {
                    throw new ArgumentException("Shop lot is already exists", lot.ToString());
                }
            }
            else
            {
                throw new ArgumentNullException("lot.Name");
            }
        }

        public void AddShopLot(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter(); // getting shop lot
            ShopLot lot = (ShopLot)bf.Deserialize(stream);

            AddShopLot(lot);
        }

        public void AddShopLot(string lotPath)
        {
            BinaryFormatter bf = new BinaryFormatter(); // getting shop lot
            ShopLot lot;
            using (FileStream fs = new FileStream(lotPath, FileMode.Open))
            {
                lot = (ShopLot)bf.Deserialize(fs);
            }

            AddShopLot(lot);
        }

        public void UpdateShopLot(ShopLot shopLot)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream($"{shopLot.Name}.safer", FileMode.Create);
            bf.Serialize(fs, shopLot);
        }

        public void RemoveShopLot(ShopLot lot)
        {
            if (_lots.Contains(lot.Name)) // checking for exists
            {
                File.Delete($"{lot.Name}.safer"); // delete file
                _lots.Remove(lot.Name); // remove from list
            }
            else
            {
                throw new ArgumentException("Shop lot is not exists", "lot");
            }
        }

        private void SaveShopLot(ShopLot lot)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream($"{lot.Name}.safer", FileMode.Create))
            {
                bf.Serialize(fs, lot);
            }
        }

        public List<string> GetShopLots()
        {
            List<string> result = new List<string>();
            foreach (var tLot in _lots)
            {
                if (File.Exists($"{tLot}.safer")) // checking for exists
                {
                    result.Add(tLot);
                }
            }
            return result;
        }

        public List<ShopLot> GetShopLots(string[] tags)
        {
            List<ShopLot> result = new List<ShopLot>();
            foreach (var temp in _lots) // checking all lots
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
        }

        public void SaveShop(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, this);
        }

        public MemoryStream SaveShop()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            return ms;
        }

        public ShopLot GetShopLot(string name)
        {
            ShopLot result = null;
            if (_lots.Contains(name)) // checking for exists in list
            {
                if (File.Exists($"{name}.safer")) // checking for exists in directory
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fs = new FileStream($"{name}.safer", FileMode.Open))
                    {
                        result = (ShopLot)bf.Deserialize(fs);
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;
            foreach(var t in _lots)
            {
                result += (t + Environment.NewLine);
            }
            foreach (var t in _accounts)
            {
                result += (t + Environment.NewLine);
            }
            return result;
        }

        public void AddAccount(Account account)
        {
            if (account.Name == null) // check account name for null
            {
                throw new ArgumentNullException("account.Name", "Name can't be null");
            }
            if (_accounts.Contains(account.Name)) // check account name for exists
            {
                throw new ArgumentException("This account name already exists");
            }
            SaveAccount(account);
            _accounts.Add(account.Name);
        }

        public void AddAccount(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Account account = (Account)bf.Deserialize(stream);

            AddAccount(account);
        }

        public void AddAccount(string accPath)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Account account;
            using (FileStream fs = new FileStream(accPath, FileMode.Open))
            {
                account = (Account)bf.Deserialize(fs);
            }

            AddAccount(account);
        }

        public void UpdateAccount(Account account)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream($"{account.Name}.acc", FileMode.Create);
            bf.Serialize(fs, account);
        }

        public void RemoveAccount(Account account)
        {
            if (_lots.Contains(account.Name))
            {
                File.Delete($"{account.Name}.acc");
                _lots.Remove(account.Name);
            }
            else
            {
                throw new ArgumentException("Account is not exists", "account");
            }
        }

        private void SaveAccount(Account account)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream($"{account.Name}.acc", FileMode.Create))
            {
                bf.Serialize(fs, account);
            }
        }

        public List<string> GetAccounts()
        {
            List<string> result = new List<string>();
            foreach (var tAcc in _accounts) // search in all accounts
            {
                if (File.Exists($"{tAcc}.acc")) // if account exists
                {
                    result.Add(tAcc);
                }
            }
            return result;
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
            Account result = null;
            if (_accounts.Contains(name)) // if account exists
            {
                if (File.Exists($"{name}.acc")) // if file exists
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fs = new FileStream($"{name}.acc", FileMode.Open)) // load from file
                    {
                        result = (Account)bf.Deserialize(fs);
                    }
                }
            }
            return result;
        }

        public bool SelfChecking() // checking for corrupt in list or in directory
        {
            foreach (var tLot in _lots)
            {
                if (!File.Exists($"{tLot}.safer"))
                {
                    return false;
                }
            }
            foreach (var tAcc in _accounts)
            {
                if (!File.Exists($"{tAcc}.acc"))
                {
                    return false;
                }
            }
            return true;
        }
    }
}