using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Organizer.Business
{
    public static class CryptoService
    {
        public static string HashingPassword(string password)
        {
            // generate salt 22 symbols // $2b$10$
            // where 2b - version       // 10 - work factor (default) 
            string salt = BCrypt.Net.BCrypt.GenerateSalt(SaltRevision.Revision2B);

            // Hashing... 
            // Generate hash - $2b$10$saltpass
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hash;
        }

        public static bool VerificationPassword(string input, string hash)
        {
            string hashInput = BCrypt.Net.BCrypt.HashPassword(input, GetSaltInHash(hash));
            return hashInput == hash;
        }
        public static string GetRandomPassword()
        {
            Random random = new Random();

            string lower_case = "abcdefghijklmnopqrstuvwxyz";
            string upper_case = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";
            string password = "";

            for (int i = 0; i < 4; i++)
            {
                password += lower_case[random.Next(lower_case.Length)].ToString() +
                    upper_case[random.Next(upper_case.Length)].ToString() +
                    digits[random.Next(digits.Length)].ToString();
            }

            return password;
        }
        private static string GetSaltInHash(string hash)
        {
            return hash.Substring(0, 29);
        }
    }
}
