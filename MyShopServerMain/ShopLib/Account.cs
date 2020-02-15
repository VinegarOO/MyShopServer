using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopLib
{
    public class Account : ISaveable
    {
        public string Name;
        public int AccessRight;
        public string Password;
        public long Money;

        public virtual byte[] Save()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Name);
            IEnumerable<byte> result = BitConverter.GetBytes(buffer.Length);
            result = result.Concat(buffer);

            buffer = BitConverter.GetBytes(AccessRight);
            result = result.Concat(BitConverter.GetBytes(buffer.Length));
            result = result.Concat(buffer);

            buffer = Encoding.UTF8.GetBytes(Password);
            result = result.Concat(BitConverter.GetBytes(buffer.Length));
            result = result.Concat(buffer);

            buffer = BitConverter.GetBytes(Money);
            result = result.Concat(BitConverter.GetBytes(buffer.Length));
            result = result.Concat(buffer);

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
            AccessRight = BitConverter.ToInt32(data, position);
            position += size;

            size = BitConverter.ToInt32(data, position);
            position += 4;
            Password = Encoding.UTF8.GetString(data, position, size);
            position += size;

            size = BitConverter.ToInt32(data, position);
            position += 4;
            Money = BitConverter.ToInt64(data, position);

            return true;
        }
    }
}