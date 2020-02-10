namespace ShopLib
{
    public interface ISaveable
    {
        byte[] Save();

        bool Load(byte[] data);
    }
}