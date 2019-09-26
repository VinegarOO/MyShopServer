using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MyShopServerMain.core.wrappers.DB
{
    public static class MyDB
    {
        private static HashSet<Type> _typesOfData = new HashSet<Type>();

        public static bool AddData(object data, string name)
        {
            Type dataType = data.GetType();

            if (dataType.IsSerializable)
            {
                if (!_typesOfData.Contains(dataType))
                {
                    Directory.CreateDirectory(dataType.Name + "/");
                    _typesOfData.Add(dataType);
                }

                if (GetListOfData(dataType).Contains(name))
                {
                    return false;
                }

                using (FileStream fs = new FileStream(dataType.Name + "/" + name + ".shda", FileMode.Create))
                {
                    DataForWrappers.bf.Serialize(fs, data);
                }

                return true;
            }
            return false;
        }

        public static bool GetData(ref object data, string name)
        {
            Type dataType = data.GetType();

            if (dataType.IsSerializable && _typesOfData.Contains(dataType))
            {


                return true;
            }
            return false;
        }

        public static HashSet<string> GetListOfData(Type typeOfData)
        {
            if (_typesOfData.Contains(typeOfData))
            {

            }

            return null;
        }
    }
}