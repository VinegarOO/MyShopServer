using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopLib
{
    public class ShopLot : ISaveable
    {
        public string Name { get; private set; }
        public long Price { get; private set; }
        public string About { get; private set; }
        public byte[] Picture { get; private set; }

        public virtual byte[] Save()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Name);
            IEnumerable<byte> result = BitConverter.GetBytes(buffer.Length);
            result = result.Concat(buffer);

            buffer = BitConverter.GetBytes(Price);
            result = result.Concat(BitConverter.GetBytes(buffer.Length));
            result = result.Concat(buffer);

            buffer = Encoding.UTF8.GetBytes(About);
            result = result.Concat(BitConverter.GetBytes(buffer.Length));
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
            Price = BitConverter.ToInt64(data, position);
            position += size;

            size = BitConverter.ToInt32(data, position);
            position += 4;
            About = Encoding.UTF8.GetString(data, position, size);
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