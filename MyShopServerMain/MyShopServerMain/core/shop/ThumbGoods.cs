using System.Drawing;
using MyShopServerMain.core.wrappers.DB;

namespace MyShopServerMain.core.shop
{
    public class ThumbGoods
    {
        public readonly Image Image;
        public readonly string Name;

        public ThumbGoods(string name)
        {
            Image = MyDb.GetData<Image>(name);
            Name = name;
        }
    }
}