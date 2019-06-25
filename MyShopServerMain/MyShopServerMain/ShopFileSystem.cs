using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace ShopFileSystems
{
    static class ShopFileSystem
    {
        static public void SaveToFile(ShopFileSystemData fsInfo)
        {
            String f_name = String.Concat(fsInfo.name, fsInfo.typeOfData);
            if (File.Exists(f_name))//cheking file for exists
            {
                Console.WriteLine("File is exists.", Environment.NewLine, "Do you want to rewrite it?", 
                    Environment.NewLine, "(yes/no)");
                for (; ; )// waiting for answer
                {
                    String temp = Console.ReadLine();
                    if (temp == "n" || temp == "no")
                    {
                        throw new FileLoadException("File already exists", f_name);
                    }
                    else if (temp == "y" || temp == "yes")
                    {
                        File.Delete(f_name);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Eror, please try again.");
                    }
                }
                
            }
            using (FileStream fs = new FileStream(f_name, FileMode.Create))
            {
                Int32 numbers = fsInfo.data.Length;
                fs.Write(BitConverter.GetBytes(numbers), 0, 4);
                Int64 data_position = (numbers + 1) * 4;
                for (Int32 i = 0; i < numbers; i++)
                {
                    fs.Write(BitConverter.GetBytes(fsInfo.data[i].Length), 0, 4);
                    fs.Position = data_position;
                    fs.Write(fsInfo.data[i], 0, fsInfo.data[i].Length);
                    data_position = fs.Position;
                    fs.Position = (i + 2) * 4;
                }
            }
        }

        static public ShopFileSystemData LoadFromFile(String name , String typeOfData)
        {
            Byte[][] data;
            String f_name = String.Concat(name, typeOfData);
            if (!File.Exists(f_name))
            {
                throw new FileNotFoundException("File is not exists", f_name);
            }
            Byte[] temp = new Byte[4];
            using (FileStream fs = new FileStream(f_name, FileMode.Open))
            {
                fs.Read(temp, 0, 4);
                Int32 numbers = BitConverter.ToInt32(temp, 0);
                data = new Byte[numbers][];
                Int64 data_position = (numbers + 1) * 4;
                for (Int32 i = 0; i < numbers; i++)
                {
                    fs.Read(temp, 0, 4);
                    fs.Position = data_position;
                    Int32 t_leght = BitConverter.ToInt32(temp, 0);
                    data[i] = new Byte[t_leght];
                    fs.Read(data[i], 0, t_leght);
                    data_position = fs.Position;
                    fs.Position = (i + 2) * 4;
                }
            }
            ShopFileSystemData fsData = new ShopFileSystemData(name, data);
            return fsData;
        }

        static public List<String> GetAllFiles(String typeOfData)
        {
            String[] t_files;
            List<String> files = new List<String>();
            String directory = FileSystem.CurrentDirectory;
            t_files = FileSystem.GetFiles(directory).ToArray<String>();
            for (Int32 i = 0; i < t_files.Length; i++)
            {
                if (t_files[i].EndsWith(typeOfData))
                {
                    Int32 start = t_files[i].LastIndexOf('\\') + 1;
                    Int32 end = t_files[i].LastIndexOf('.');
                    String temp = t_files[i].Substring(start, end - start);
                    files.Add(temp);
                }
            }
            return files;
        }
    }

    class ShopFileSystemData
    {
        public readonly String name;
        public readonly String typeOfData = ".safer";
        public readonly Byte[][] data;
        private Encoding encoding = new UTF8Encoding();

        public ShopFileSystemData(String t_name, Int32 price, String about, Byte[] image)
        {
            name = t_name;
            data = new Byte[3][];
            data[0] = BitConverter.GetBytes(price);
            data[1] = encoding.GetBytes(about);
            data[2] = image;
        }

        public ShopFileSystemData(String t_name, Byte[][] t_data)
        {
            if (t_data.Length != 3) throw new ArgumentException("Byte array must have only 3 members");
            name = t_name;
            data = t_data;
        }
    }
}
 