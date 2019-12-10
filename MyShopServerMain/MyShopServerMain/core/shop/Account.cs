using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyShopServerMain.core.shop
{
    [Serializable]
    internal class Account : ShopLib.Account
    {
        internal Account(string name, string newPassword, AccessRights newAccessRight = AccessRights.User)
        {
            _password = newPassword;
            Name = name;
            _money = 0;
            AccessRight = (int)newAccessRight;
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

        internal void ChangePassword(string newPassword, Account adminAccount, string aPassword)
        {
            if (adminAccount.AccessRight == (int)AccessRights.Admin)
            {
                if (adminAccount.Verify(aPassword))
                {
                    _password = newPassword;
                }
                else
                {
                    throw new MemberAccessException("Incorrect password");
                }
            }
            else
            {
                throw new FormatException("Not enough rights");
            }
        }

        internal void Withdraw(long sum, string tPassword)
        {
            if (tPassword == _password)
            {
                if (sum < 0)
                {
                    throw new ArgumentException("you can't withdraw sum below zero");
                }
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

        internal void Withdraw(long sum, Account account, string aPassword)
        {
            if (account.AccessRight == (int)AccessRights.Admin)
            {
                if (sum < 0)
                {
                    throw new ArgumentException("you can't withdraw sum below zero");
                }
                if (account.Verify(aPassword))
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
            else
            {
                throw new FormatException("Not enough rights");
            }
        }

        internal void Refill(long sum, Account aAccount, string aPassword)
        {
            if (aAccount.AccessRight == (int)AccessRights.Admin)
            {
                if (sum < 0)
                {
                    throw new ArgumentException("you can't refill sum below zero");
                }
                if (aAccount.Verify(aPassword))
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
                throw new FormatException("Not enough rights");
            }
        }

        internal void ChangeAccessRights(AccessRights newRights, Account account, string aPassword)
        {
            if (account.AccessRight == (int)AccessRights.Admin)
            {
                if (account.Verify(aPassword))
                {
                    if (newRights == AccessRights.Admin)
                    {
                        throw new FormatException("Account cant be the shop");
                    }
                    AccessRight = (int)newRights;
                }
                else
                {
                    throw new MemberAccessException("Incorrect password");
                }
            }
            else
            {
                throw new FormatException("Not enough rights");
            }
        }

        public override string ToString()
        {
            string result = String.Empty;
            result += $"Name: {Name}" + Environment.NewLine;
            result += $"State of account: {_money}" + Environment.NewLine;
            result += $"AccessRights: {(AccessRights)AccessRight}";
            return result;
        }
    }
}