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
    class Shop : IShop
    {
        private List<IShopLot> lots = new List<IShopLot>();

        public Shop()
        {
            
        }

        public void AddShopLot(IShopLot lot)
        {
            
        }

        public void AddShopLot(String lot_path)
        {

        }

        public void AddShopLot(Stream stream)
        {

        }

        public void RemoveShopLot(IShopLot lot)
        {

        }

        public List<String> GetShopLots()
        {
            List<String> result = new List<String>();
            return result;
        }

        public void SaveIShop(Stream stream)
        {

        }
    }

    [Serializable]
    class ShopLot : IShopLot
    {
        private String name;
        public String Name { get { return name; } }
        private Double price;
        public Double Price { get { return price; } }
        private String about;
        public String About { get { return about; } }
        private Image picture;
        public Image Picture { get { return picture; } }

        public ShopLot(String t_name, String picture_path, String t_about, Double t_price)
        {
            name = t_name;

            picture = Image.FromFile(picture_path);

            if (t_price > 0) price = t_price;
            else throw new ArgumentException("price might be grater than zero", price.ToString());

            about = t_about;
        }

        public void EditPicture(String picture_path)
        {
            picture = Image.FromFile(picture_path);
        }

        public void EditPrice(Double t_price)
        {
            if (t_price > 0) price = t_price;
            else throw new ArgumentException("price might be grater than zero", price.ToString());
        }

        public void EditAbout(String t_about)
        {
            about = t_about;
        }
    }

    interface IShopLot
    {
        String Name { get; }
        Double Price { get; }
        String About { get; }
        Image Picture { get; }

        void EditPicture(String picture_path);

        void EditPrice(Double t_price);

        void EditAbout(String t_about);
    }

    interface IAccount
    {
        void ChangePassword(String password);

        void EditAbout(String about);

        Int32 GetWeight();
    }

    interface IShop
    {
        void AddShopLot(IShopLot lot);

        void AddShopLot(String lot_path);

        void AddShopLot(Stream stream);

        void RemoveShopLot(IShopLot lot);

        List<String> GetShopLots();

        void SaveIShop(Stream stream);

    }
}