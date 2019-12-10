using System;
using System.Drawing;
using System.IO;
using MyShopServerMain.core.wrappers.DB;

namespace MyShopServerMain.core.shop
{
    [Serializable]
    public class ShopLot : ShopLib.ShopLot
    {
        private bool ThumbnailCallback()
        {
            return false;
        }

        internal ShopLot(string tName, string picturePath, string tAbout, long tPrice)
        {
            if (tName == null)
            {
                throw new ArgumentNullException("tName", "Name can't be null");
            }
            if (tName == String.Empty)
            {
                throw new ArgumentException("Name can't be empty", "tName");
            }
            Name = tName;

            using (FileStream fs = new FileStream(picturePath, FileMode.Open))
            {
                Picture = new byte[fs.Length];
                fs.Read(Picture, 0, (int)fs.Length);
            }
            Image temp = Image.FromFile(picturePath);
            temp = temp.GetThumbnailImage(100, 100
                , new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            MyDb.AddData(temp, Name);

            if (tPrice > 0)
            {
                Price = tPrice;
            }
            else
            {
                throw new ArgumentException("price might be grater than zero", "tPrice");
            }

            About = tAbout;
        }

        internal void EditPrice(long tPrice)
        {
            if (tPrice > 0)
            {
                Price = tPrice;
            }
            else
            {
                throw new ArgumentException("price might be grater than zero", "tPrice");
            }
        }

        internal void EditAbout(string tAbout)
        {
            About = tAbout;
        }

        public override string ToString()
        {
            string result = String.Empty;
            result += $"Name: {Name}\n";
            result += $"Price: {Price}\n";
            result += $"Description: {About}";
            return result;
        }
    }
}
