﻿using System;

namespace MyShopServerMain.core.shop
{
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
}