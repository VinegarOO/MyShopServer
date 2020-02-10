using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShopLib;

namespace ShopServerMain.core.wrappers.DB
{
    public static class MyDb
    {
        private static Object locker = new object();

        public static bool AddData(ISaveable data, string name)
        {
            Type dataType = data.GetType();
            string path = $"{dataType.Name}//{name}.shda";

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

                using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                {
                    var buffer = data.Save();
                    int l = (int) buffer.Length;
                    fs.Write(buffer, 0 , l);
                }
            }

            return true;
        }

        public static byte[] GetData (string name, string Type)
        {
            byte[] result;
            string path = $"{Type}//{name}.shda";

            if (Directory.Exists(Type))
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException();
                }

                lock (locker)
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        int l = (int)fs.Length;
                        result = new byte[l];
                        fs.Read(result, 0, l);
                    }
                }

                return result;
            }
            throw new FileNotFoundException();
        }

        public static bool RemoveData(string name, string Type)
        {
            string path = $"{Type}//{name}.shda";

            if (Directory.Exists(Type))
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

        public static bool UpdateData(ISaveable data, string name)
        {
            Type dataType = data.GetType();

            lock (locker)
            {
                if (RemoveData(name, dataType.Name))
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

            return result;
        }
    }
}