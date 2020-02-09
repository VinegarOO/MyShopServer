using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopLib
{
    public class ThumbGoods : ISaveable
    {
        public string Name;
        public byte[] Picture;

        public virtual byte[] Save()
        {
            var buffer = Encoding.UTF8.GetBytes(Name);
            IEnumerable<byte> result = BitConverter.GetBytes(buffer.Length);
            result = result.Concat(buffer);

            result = result.Concat(BitConverter.GetBytes(Picture.Length));
            result = result.Concat(Picture);

            return result.ToArray();
        }

        public virtual bool Load(byte[] data)
        {
            int position = 0;
            int size;

            size = BitConverter.ToInt32(data, position);
            position += 4;
            Name = Encoding.UTF8.GetString(data, position, size);
            position += size;

            size = BitConverter.ToInt32(data, position);
            position += 4;
            for (int i = 0; i < size; i++)
            {
                Picture[i] = data[i + position];
            }

            return true;
        }
    }
}