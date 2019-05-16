using System;
using System.Linq;
using System.Security.Cryptography;

namespace LoginModule
{
    public static class Utilities
    {
        private static readonly RNGCryptoServiceProvider CryptoServiceProvider = new RNGCryptoServiceProvider();

        public static bool CheckPasswordMatch(string hashedPassword, byte[] salt, string plainText)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, 10000);
            var passwordToCheck = Convert.ToBase64String(pbkdf2.GetBytes(20).Concat(salt).ToArray());
            return hashedPassword.Equals(passwordToCheck);
        }

        public static string HashPassword(string plainText, out byte[] salt)
        {
            CryptoServiceProvider.GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, 10000);

            var hashBytes = pbkdf2.GetBytes(20).Concat(salt).ToArray();
            return Convert.ToBase64String(hashBytes);
        }
    }
}
