using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;


namespace ShopForServer
{
    [Serializable]
    class Shop : IShop
    {
        private List<IShopLot> lots;

        public Shop()
        {
            lots = new List<IShopLot>();
        }

        public void AddShopLot(IShopLot lot)
        {
            if (!lots.Contains(lot))
            {
                lots.Add(lot);
            }
            else
            {
                throw new ArgumentException("Shop lot is already exists", lot.ToString());
            }
        }

        public void AddShopLot(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            IShopLot lot;
            try
            {
                lot = (IShopLot)bf.Deserialize(stream);
            }
            catch(Exception e)
            {
                throw e;
            }

            if (!lots.Contains(lot))
            {
                lots.Add(lot);
            }
            else
            {
                throw new ArgumentException("Shop lot is already exists", lot.ToString());
            }
        }

        public void AddShopLot(string lot_path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            IShopLot lot;
            try
            {
                using (FileStream fs = new FileStream(lot_path, FileMode.Open))
                {
                    lot = (IShopLot)bf.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            if (!lots.Contains(lot))
            {
                lots.Add(lot);
            }
            else
            {
                throw new ArgumentException("Shop lot is already exists", lot.ToString());
            }
        }

        public void RemoveShopLot(IShopLot lot)
        {
            if (lots.Contains(lot))
            {
                lots.Remove(lot);
            }
            else
            {
                throw new ArgumentException("Shop lot is not in the Shop", lot.ToString());
            }
        }

        public List<string> GetShopLots()
        {
            List<string> result = new List<string>();
            foreach(var temp in lots)
            {
                result.Add(temp.Name);
            }
            return result;
        }

        public void SaveIShop(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, this);
        }

        public MemoryStream SaveIShop()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);
            return ms;
        }

        public IShopLot[] GetLotByName(string name)
        {
            IShopLot[] result;
            List<IShopLot> temp = new List<IShopLot>();
            foreach(var t in lots)
            {
                if (t.Name == name) temp.Add(t);
            }
            result = temp.ToArray();
            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;
            HashSet<string> temp = new HashSet<string>();
            foreach(var t in lots)
            {
                temp.Add(t.ToString());
            }
            foreach(var t in temp)
            {
                result = String.Concat(t.ToString());
            }
            return result;
        }
    }

    [Serializable]
    class ShopLot : IShopLot
    {
        private string name;
        public string Name { get { return name; } }
        private double price;
        public double Price { get { return price; } }
        private string about;
        public string About { get { return about; } }
        private Image picture;
        public Image Picture { get { return picture; } }

        public ShopLot(string t_name, string picture_path, string t_about, double t_price)
        {
            name = t_name;

            picture = Image.FromFile(picture_path);

            if (t_price > 0) price = t_price;
            else throw new ArgumentException("price might be grater than zero", price.ToString());

            about = t_about;
        }

        public void EditPicture(string picture_path)
        {
            picture = Image.FromFile(picture_path);
        }

        public void EditPrice(double t_price)
        {
            if (t_price > 0) price = t_price;
            else throw new ArgumentException("price might be grater than zero", price.ToString());
        }

        public void EditAbout(string t_about)
        {
            about = t_about;
        }
    }

    interface IShopLot
    {
        string Name { get; }
        double Price { get; }
        string About { get; }
        Image Picture { get; }

        void EditPicture(string picture_path);

        void EditPrice(double t_price);

        void EditAbout(string t_about);
    }

    interface IAccount
    {
        void ChangePassword(string password);

        void EditAbout(string about);

        int GetWeight();
    }

    interface IShop
    {
        void AddShopLot(IShopLot lot);

        void AddShopLot(Stream stream);

        void AddShopLot(string lot_path);

        void RemoveShopLot(IShopLot lot);

        List<string> GetShopLots();

        void SaveIShop(Stream stream);

        MemoryStream SaveIShop();

        IShopLot[] GetLotByName(string name);


    }
}