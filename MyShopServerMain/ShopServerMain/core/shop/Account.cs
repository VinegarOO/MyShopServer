using System;

namespace ShopServerMain.core.shop
{
    [Serializable]
    internal class Account : ShopLib.Account
    {
        internal Account(string name, string newPassword, AccessRights newAccessRight = AccessRights.User)
        {
            Password = newPassword;
            Name = name;
            Money = 0;
            AccessRight = (int)newAccessRight;
        }

        internal bool Verify(string tPassword)
        {
            return tPassword == Password;
        }

        internal void ChangePassword(string newPassword, string tPassword)
        {
            if (tPassword == Password)
            {
                Password = newPassword;
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
                    Password = newPassword;
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
            if (tPassword == Password)
            {
                if (sum < 0)
                {
                    throw new ArgumentException("you can't withdraw sum below zero");
                }
                if (Money >= sum)
                {
                    Money -= sum;
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
                    if (Money >= sum)
                    {
                        Money -= sum;
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
                    Money += sum;
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
            result += $"State of account: {Money}" + Environment.NewLine;
            result += $"AccessRights: {(AccessRights)AccessRight}";
            return result;
        }
    }
}