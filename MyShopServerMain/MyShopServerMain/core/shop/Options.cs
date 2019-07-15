using System.Collections.Generic;

namespace MyShopServerMain.core.shop
{
    internal struct Options
    {
        internal List<string> optList;

        internal Options(string[] tOpt)
        {
            optList = new List<string>();
            foreach (var t in tOpt)
            {
                if (t != null)
                {
                    if (!optList.Contains(t))
                    {
                        optList.Add(t);
                    }
                }
            }
        }

        internal bool AddOption(string t)
        {
            if (t != null)
            {
                if (!optList.Contains(t))
                {
                    optList.Add(t);
                    return true;
                }
            }
            return false;
        }

        internal bool RemoveOption(string t)
        {
            if (t != null)
            {
                if (optList.Contains(t))
                {
                    optList.Remove(t);
                    return true;
                }
            }
            return false;
        }
    }
}