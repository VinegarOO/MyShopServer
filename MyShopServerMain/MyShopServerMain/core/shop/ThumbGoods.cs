using System.Drawing;
using MyShopServerMain.core.wrappers.DB;

namespace MyShopServerMain.core.shop
{
    public class ThumbGoods : ShopLib.ThumbGoods
    {
        public ThumbGoods(string name, byte[] picture)
        {
            Name = name;
            Picture = picture;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}