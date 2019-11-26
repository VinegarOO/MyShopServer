using System;
using System.Drawing;
using MyShopServerMain.core.wrappers.DB;

namespace MyShopServerMain.core.shop
{
    [Serializable]
    internal class ShopLot
    {
        internal string Name { get; private set; }
        internal decimal Price { get; private set; }
        internal string About { get; private set; }
        internal Image Picture { get; private set; }
        internal string[] Tags { get; private set; }

        private bool ThumbnailCallback()
        {
            return false;
        }

        internal ShopLot(string tName, string picturePath, string tAbout, decimal tPrice, string[] tags)
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

            Image temp = Image.FromFile(picturePath);
            Picture = temp;
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

            if (tags == null)
            {
                Tags = new string[0];
            }
            else
            {
                Tags = tags;
            }
        }

        internal void EditPicture(string picturePath)
        {
            Image temp = Image.FromFile(picturePath);
            Picture = temp;
            temp = temp.GetThumbnailImage(100,100
                , new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
            MyDb.AddData(temp, picturePath);
        }

        internal void EditPrice(decimal tPrice)
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

        internal string GetTags()
        {
            string result = String.Empty;
            foreach (var tag in Tags)
            {
                result += tag;
                result += " ";
            }

            return result;
        }

        public override string ToString()
        {
            string result = String.Empty;
            result += $"Name: {Name}\n";
            result += $"Price: {Price}\n";
            result += $"Tags: {this.GetTags()}\n";
            result += $"Description: {About}";
            return result;
        }
    }
}
