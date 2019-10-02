using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyShopServerMain.core.wrappers.DB
{
    public static class MyDb
    {
        private static HashSet<Type> _typesOfData = new HashSet<Type>();
        private static readonly BinaryFormatter Bf = new BinaryFormatter();

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
                    Bf.Serialize(fs, data);
                }

                return true;
            }
            return false;
        }

        public static T GetData<T> (string name) where T : class
        {
            Type dataType = typeof(T);
            T result = null;
            string path = dataType + "/" + name + ".shda";

            if (dataType.IsSerializable && _typesOfData.Contains(dataType))
            {
                if (!File.Exists(path))
                {
                    return result;
                }

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    result = (T)Bf.Deserialize(fs);
                }

                return result;
            }
            return result;
        }

        public static List<string> GetListOfData(Type typeOfData)
        {
            List<string> result = new List<string>();
            string path = typeOfData + "/";
            
            if (_typesOfData.Contains(typeOfData))
            {
                result = Directory.GetFiles(path).ToList();

                foreach (var name in result)
                {
                    if (name.EndsWith(".shda"))
                    {
                        int dot = name.LastIndexOf('.');
                        name.Remove(dot);
                    }
                    else
                    {
                        result.Remove(name);
                    }
                }

                return result;
            }

            return null;
        }
    }
}