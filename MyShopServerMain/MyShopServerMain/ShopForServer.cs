using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ShopFileSystems;

namespace Shop
{
    class Shop : IShop
    {
        private List<ShopLot> shopLots;

        public Shop()
        {
            List<String> shopFiles;
            shopFiles = ShopFileSystem.GetAllFiles(".safer");
            shopLots = new List<ShopLot>(shopFiles.Count);
            for (Int32 i = 0; i < shopLots.Count; i++)
            {
                shopLots[i] = new ShopLot(shopFiles[i]);
                shopLots[i].LoadShopLot();
            }
        }

        public void AddShopLot(IShopLot shopLot)
        {
            if (shopLot is ShopLot)
            {
                shopLots.Add(shopLot as ShopLot);
            }
            else throw new ArgumentException("IShopLot must be ShopLot", shopLot.ToString());
        }

        public void RemoveShopLot(IShopLot shopLot)
        {

        }
    }

    class ShopLot : IShopLot
    {
        public String Name { get; private set; }
        public Double Price { get; private set; }
        private String about;
        private Byte[] picture;

        public ShopLot(String name)
        {
            Name = name;
        }

        public void FillShopLot(Double price, String picture_path, String t_about)
        {
            if (price > 0) Price = price;
            else throw new ArgumentException("price must be greater than 0", new Exception(price.ToString()));
            FileInfo fl = new FileInfo(picture_path);
            picture = new Byte[fl.Length];
            using (FileStream pic = new FileStream(picture_path, FileMode.Open))
            {
                pic.Read(picture, 0, (Int32)fl.Length);
            };
            fl.Delete();
            this.about = t_about;
        }

        public void LoadShopLot()
        {
            ShopFileSystemData data;
            data = ShopFileSystem.LoadFromFile(Name, ".safer");
            if (data.data.Length != 3) throw new InvalidDataException("must be 3 filds");

        }

        public void SaveShopLot()
        {
            
        }
    }

    interface IShopLot
    {
        void LoadShopLot();

        void SaveShopLot();
    }

    interface IAccount
    {
        void ChangePassword(String password);

        void LoadAccout();

        void SaveAccout();
    }

    interface IShop
    {
        void AddShopLot(IShopLot shopLot);

        void RemoveShopLot(IShopLot shopLot);

        List<IShopLot> GetShopLots();


    }
}