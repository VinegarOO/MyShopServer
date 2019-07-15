using System;
using System.Drawing;

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
        internal Options Option { get; private set; }

        internal ShopLot(string tName, string picturePath, string tAbout, decimal tPrice, string[] tags, Options tOpt)
        {
            if (tName == null)
            {
                throw new ArgumentNullException("tName");
            }
            if (tName == String.Empty)
            {
                throw new ArgumentException("Name can't be empty", "tName");
            }
            Name = tName;

            if (picturePath == null)
            {
                throw new ArgumentNullException("picturePath");
            }
            Picture = Image.FromFile(picturePath);

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
                throw new ArgumentNullException(nameof(tags));
            }
            Tags = tags;

            if (tOpt.optList.Count != 0)
            {
                Option = tOpt;
            }
            else
            {
                throw new ArgumentNullException("tOpt", "Must contain at least one Option");
            }
        }

        internal void EditPicture(string picturePath)
        {
            if (picturePath == null)
            {
                throw new ArgumentNullException("picturePath");
            }
            Picture = Image.FromFile(picturePath);
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
    }
}