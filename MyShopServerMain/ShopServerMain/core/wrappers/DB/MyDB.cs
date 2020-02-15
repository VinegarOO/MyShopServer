using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShopLib;

namespace ShopServerMain.core.wrappers.DB
{
    public static class MyDb
    {
        private static Object _locker = new object();

        public static bool AddData(byte[] data, string name, string type)
        {
            string path = $"{type}//{name}.shda";

            if (GetListOfData(type).Contains(name))
            {
                return false;
            }

            lock (_locker)
            {
                if (!Directory.Exists(type))
                {
                    Directory.CreateDirectory(type + "//");
                }

                using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                {
                    int l = (int) data.Length;
                    fs.Write(data, 0 , l);
                }
            }

            return true;
        }

        public static byte[] GetData (string name, string type)
        {
            byte[] result;
            string path = $"{type}//{name}.shda";

            if (Directory.Exists(type))
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException();
                }

                lock (_locker)
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

        public static bool RemoveData(string name, string type)
        {
            string path = $"{type}//{name}.shda";

            if (Directory.Exists(type))
            {
                if (File.Exists(path))
                {
                    lock (_locker)
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

        public static bool UpdateData(byte[] data, string name, string type)
        {
            lock (_locker)
            {
                if (RemoveData(name, type))
                {
                    return AddData(data, name, type);
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<string> GetListOfData(string typeOfData)
        {
            List<string> result = new List<string>();
            string path = $"{typeOfData}//";
            
            if (Directory.Exists(typeOfData))
            {
                result = Directory.GetFiles(path).ToList();

                foreach (var name in result)
                {
                    lock (_locker)
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