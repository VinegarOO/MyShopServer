using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ShopServerMain.core.shop
{
    [Serializable]
    public class ShopLot : ShopLib.ShopLot
    {
        public bool ThumbnailCallback()
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

            if (picturePath == "null")
            {
                Picture = new byte[]{1};
            }
            else
            {
                using (FileStream fs = new FileStream(picturePath, FileMode.Open))
                {
                    Picture = new byte[fs.Length];
                    fs.Read(Picture, 0, (int)fs.Length);
                }
            }

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

        internal ThumbGoods GetThumbGoods()
        {
            Image image;
            using (MemoryStream ms = new MemoryStream(Picture))
            {
                image = new Bitmap(ms);
            }

            Image thumb = image.GetThumbnailImage(128, 128,
                ThumbnailCallback, IntPtr.Zero);
            byte[] thumbBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                thumb.Save(ms, ImageFormat.Png);
                int length = (int)ms.Length;
                thumbBytes = new byte[length];
                ms.Read(thumbBytes, 0, length);
            }

            return new ThumbGoods(Name, thumbBytes);
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
