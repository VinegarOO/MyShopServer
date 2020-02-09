namespace ShopLib
{
    interface ISaveable
    {
        byte[] Save();

        bool Load(byte[] data);
    }
}