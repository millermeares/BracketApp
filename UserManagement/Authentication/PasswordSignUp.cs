using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace UserManagement.Authentication
{
    internal class PasswordSignUp
    {
        private static int _hashIterations = 10000;
        private static int _hashSize = 128;
        internal string Salt { get; set; } = string.Empty;
        internal string Hash { get; set; } = string.Empty;

        internal static PasswordSignUp Make(string password)
        {
            Guid salt = Guid.NewGuid();
            string hash = MakeHash(password, salt);
            return new PasswordSignUp
            {
                Salt = salt.ToString(),
                Hash = hash
            };
        }

        internal static string MakeHash(string password, Guid salt)
        {
            return MakeHash(password, salt.ToByteArray());
        }
        internal static string MakeHash(string password, byte[] salt)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            return System.Text.Encoding.Default.GetString(pbkdf2.GetBytes(_hashSize));
        }
    }
}
