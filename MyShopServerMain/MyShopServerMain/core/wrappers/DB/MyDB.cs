using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyShopServerMain.core.wrappers.DB
{
    public static class MyDb
    {
        private static readonly BinaryFormatter Bf = new BinaryFormatter();
        private static Object locker;

        public static bool AddData(object data, string name)
        {
            Type dataType = data.GetType();
            string path = $"{dataType.Name}//{name}.shda";

            if (dataType.IsSerializable)
            {
                if (GetListOfData(dataType).Contains(name))
                {
                    return false;
                }

                lock (locker)
                {
                    if (!Directory.Exists(dataType.Name))
                    {
                        Directory.CreateDirectory(dataType.Name + "//");
                    }
                
                    FileStream fs = new FileStream(path, FileMode.CreateNew);
                    Bf.Serialize(fs, data);
                    fs.Dispose();
                }

                return true;
            }
            return false;
        }

        public static T GetData<T> (string name) where T : class
        {
            Type dataType = typeof(T);
            T result = null;
            string path = $"{dataType.Name}//{name}.shda";

            if (dataType.IsSerializable && Directory.Exists(dataType.Name))
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                lock (locker)
                {
                    FileStream fs = new FileStream(path, FileMode.Open);
                    result = (T)Bf.Deserialize(fs);
                    fs.Dispose();
                }

                return result;
            }
            return null;
        }

        public static bool RemoveData(object data, string name)
        {
            Type dataType = data.GetType();
            string path = $"{dataType.Name}//{name}.shda";

            if (Directory.Exists(dataType.Name))
            {
                if (File.Exists(path))
                {
                    lock (locker)
                    {
                        File.Delete(path);
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
            return true;
        }

        public static bool UpdateData(object data, string name)
        {
            Type dataType = data.GetType();

            lock (locker)
            {
                if (RemoveData(data, name))
                {
                    return AddData(data, name);
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<string> GetListOfData(Type typeOfData)
        {
            List<string> result = new List<string>();
            string path = $"{typeOfData.Name}//";
            
            if (Directory.Exists(typeOfData.Name))
            {
                result = Directory.GetFiles(path).ToList();

                foreach (var name in result)
                {
                    lock (locker)
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
                }

                return result;
            }

            return null;
        }
    }
}