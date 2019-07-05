using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Globalization;

namespace ShopServer
{
    [Serializable]
    internal class ShopLot
    {
        internal string Name { get; private set; }
        internal decimal Price { get; private set; }
        internal string About { get; private set; }
        internal Image Picture { get; private set; }
        internal string[] Tags { get; private set; }
        internal Options Option { get; private set; }

        internal ShopLot(string tName, string picturePath, string tAbout, decimal tPrice, string[] tags, Options tOpt)
        {
            if (tName == null)
            {
                throw new ArgumentNullException("tName");
            }
            if (tName == String.Empty)
            {
                throw new ArgumentException("Name can't be empty", "tName");
            }
            Name = tName;

            if (picturePath == null)
            {
                throw new ArgumentNullException("picturePath");
            }
            Picture = Image.FromFile(picturePath);

            if (tPrice > 0)
            {
                Price = tPrice;
            }
            else
            {
                throw new ArgumentException("price might be grater than zero", "tPrice");
            }

            About = tAbout;

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }
            Tags = tags;

            if (tOpt.optList.Count != 0)
            {
                Option = tOpt;
            }
            else
            {
                throw new ArgumentNullException("tOpt", "Must contain at least one Option");
            }
        }

        internal void EditPicture(string picturePath)
        {
            if (picturePath == null)
            {
                throw new ArgumentNullException("picturePath");
            }
            Picture = Image.FromFile(picturePath);
        }

        internal void EditPrice(decimal tPrice)
        {
            if (tPrice > 0)
            {
                Price = tPrice;
            }
            else
            {
                throw new ArgumentException("price might be grater than zero", "tPrice");
            }
        }

        internal void EditAbout(string tAbout)
        {
            About = tAbout;
        }
    }

    [Serializable]
    internal class Account
    {
        internal AccessRights AccessRight { get; set; }

        private string _password;
        private decimal _money;

        internal string Name { get; private set; }

        internal Account(string name, string newPassword, AccessRights newAccessRight = AccessRights.User)
        {
            _password = newPassword;
            Name = name;
            _money = 0;
            AccessRight = newAccessRight;
        }

        internal bool Verify(string tPassword)
        {
            return tPassword == _password;
        }

        internal void ChangePassword(string newPassword, string tPassword)
        {
            if (tPassword == _password)
            {
                _password = newPassword;
            }
            else
            {
                throw new MemberAccessException("Incorrect password");
            }
        }

        internal void Withdraw(decimal sum, string tPassword)
        {
            if (tPassword == _password)
            {
                if (_money >= sum)
                {
                    _money -= sum;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("sum", "Not enough money to do withdraw");
                }
            }
            else
            {
                throw new MemberAccessException("Incorrect password");
            }
        }

        internal void Refill(decimal sum, Account account, string aPassword)
        {
            if (account.AccessRight == AccessRights.Admin)
            {
                if (account.Verify(aPassword))
                {
                    _money += sum;
                }
                else
                {
                    throw new MemberAccessException("Incorrect password");
                }
            }
            else
            {
                throw new FormatException("Not enouth rights");
            }
        }

        internal void ChangeAccessRights(AccessRights newRights, Account account, string aPassword)
        {
            if (account.AccessRight == AccessRights.Admin)
            {
                if (account.Verify(aPassword))
                {
                    if (newRights == AccessRights.Admin)
                    {
                        throw new FormatException("Account cant be the shop");
                    }
                    AccessRight = newRights;
                }
                else
                {
                    throw new MemberAccessException("Incorrect password");
                }
            }
            else
            {
                throw new FormatException("Not enouth rights");
            }
        }
    }

    internal enum AccessRights
    {
        User = 1,
        VipUser = 2,
        Moder = 3,
        Admin = 4
    }

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