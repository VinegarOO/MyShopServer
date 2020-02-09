using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopLib
{
    public class ListOfGoods : ISaveable
    {
        public List<ThumbGoods> Goods = new List<ThumbGoods>();


        public virtual byte[] Save()
        {
            IEnumerable<byte> result = new byte[0];
            byte[] buffer;

            for (int i = 0; i < Goods.Count; i++)
            {
                buffer = Goods[i].Save();
                result = result.Concat(BitConverter.GetBytes(buffer.Length));
                result = result.Concat(buffer);
            }

            return result.ToArray();
        }

        public virtual bool Load(byte[] data)
        {
            int position = 0;
            int size;
            ThumbGoods tempThumbGoods;
            byte[] buffer;

            for (int i = 0; i < data.Length;)
            {
                size = BitConverter.ToInt32(data, position);
                position += 4;
                buffer = new byte[size];
                for (int j = position; j < position + size; j++)
                {
                    buffer[j] = data[j];
                }

                position += size;
                tempThumbGoods = new ThumbGoods();
                tempThumbGoods.Load(buffer);
                Goods.Add(tempThumbGoods);
            }

            return true;
        }
    }
}