using System.Collections.Generic;

namespace MyShopServerMain.core.shop
{
    internal interface IShop
    {
        void AddShopLot(ShopLot lot);

        void RemoveShopLot(ShopLot lot);

        List<ThumbGoods> GetShopLots();

        ShopLot GetShopLot(string name);
    }
}