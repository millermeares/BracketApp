﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using UserManagement.UserModels;

namespace UserManagement.Authentication
{
    public class Password
    {
        private static int _hashIterations = 10000;
        private static int _hashSize = 128;
        internal string Salt { get; set; } = string.Empty;
        internal byte[] Hash { get; set; } = new byte[0];
        public UserID AssociatedID { get; set; } = new UserID();

        public bool Match(string password)
        {
            byte[] auth_attempt_hash = MakeHash(password, Guid.Parse(Salt).ToByteArray());
            return ByteArrayEquality(Hash, auth_attempt_hash);
        }

        private static bool ByteArrayEquality(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length) return false;
            for(int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i]) return false;
            }
            return true;
        }

        public bool IsEmpty()
        {
            return AssociatedID.IsEmpty();
        }


        internal static Password Make(string password)
        {
            Guid salt = Guid.NewGuid();
            byte[] hash = MakeHash(password, salt);
            return new Password
            {
                Salt = salt.ToString(),
                Hash = hash
            };
        }

        internal static byte[] MakeHash(string password, Guid salt)
        {
            return MakeHash(password, salt.ToByteArray());
        }
        internal static byte[] MakeHash(string password, byte[] salt)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, _hashIterations);
            return pbkdf2.GetBytes(_hashSize);
        }

        public static Password EmptyPassword()
        {
            return new Password()
            {
                AssociatedID = new UserID()
                {
                    ID=string.Empty
                }
        };
    }

}
}
