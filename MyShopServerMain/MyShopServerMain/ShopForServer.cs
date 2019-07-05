using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;


namespace ShopServer
{
    [Serializable]
    internal class Shop : IShop, IAccountHolder
    {
        private List<ShopLot> _lots;

        public Shop()
        {
            _lots = new List<ShopLot>();
        }

        public void AddShopLot(ShopLot lot)
        {
            if (!_lots.Contains(lot))
            {
                if(this.GetShopLot(lot.Name) != null)
                {
                    _lots.Add(lot);
                }
                else
                {
                    throw new ArgumentException("Shop lot name is already exists", lot.Name);
                }
            }
            else
            {
                throw new ArgumentException("Shop lot is already exists", lot.ToString());
            }
        }

        public void AddShopLot(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            ShopLot lot;
            try
            {
                lot = (ShopLot)bf.Deserialize(stream);
            }
            catch(Exception e)
            {
                throw e;
            }

            if (!_lots.Contains(lot))
            {
                if (this.GetShopLot(lot.Name) != null)
                {
                    _lots.Add(lot);
                }
                else
                {
                    throw new ArgumentException("Shop lot name is already exists", lot.Name);
                }
            }
            else
            {
                throw new ArgumentException("Shop lot is already exists", lot.ToString());
            }
        }

        public void AddShopLot(string lotPath)
        {
            BinaryFormatter bf = new BinaryFormatter();
            ShopLot lot;
            try
            {
                using (FileStream fs = new FileStream(lotPath, FileMode.Open))
                {
                    lot = (ShopLot)bf.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            if (!_lots.Contains(lot))
            {
                if (this.GetShopLot(lot.Name) != null)
                {
                    _lots.Add(lot);
                }
                else
                {
                    throw new ArgumentException("Shop lot name is already exists", lot.Name);
                }
            }
            else
            {
                throw new ArgumentException("Shop lot is already exists", lot.ToString());
            }
        }

        public void RemoveShopLot(ShopLot lot)
        {
            if (_lots.Contains(lot))
            {
                _lots.Remove(lot);
            }
            else
            {
                throw new ArgumentException("Shop lot is not in the Shop", lot.ToString());
            }
        }

        public List<string> GetShopLots()
        {
            List<string> result = new List<string>();
            foreach(var temp in _lots)
            {
                result.Add(temp.Name);
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
            foreach(var t in _lots)
            {
                if (t.Name == name) result = t;
            }
            return result;
        }

        public ShopLot GetShopLot(string[] tags)
        {
            ShopLot result = null;
            
            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;
            HashSet<string> temp = new HashSet<string>();
            foreach(var t in _lots)
            {
                temp.Add(t.ToString());
            }
            foreach(var t in temp)
            {
                result += (t + Environment.NewLine);
            }
            return result;
        }

        public void AddAccount(Account account)
        {
            
        }

        public void RemoveAccount(Account account)
        {
            
        }

        public List<string> GetAccounts()
        {
            
        }

        public Account GetAccount(string name)
        {
            
        }
    }


    internal interface IAccountHolder
    {
        void AddAccount(Account account);

        void RemoveAccount(Account account);

        List<string> GetAccounts();

        Account GetAccount(string name);
    }

    internal interface IShop
    {
        void AddShopLot(ShopLot lot);

        void RemoveShopLot(ShopLot lot);

        List<string> GetShopLots();

        ShopLot GetShopLot(string name);
    }
}