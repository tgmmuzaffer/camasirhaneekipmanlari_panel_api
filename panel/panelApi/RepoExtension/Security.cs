using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;

namespace panelApi.RepoExtension
{
    public static class Security
    {
        public static string HashCreate(string pass, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                     password: pass,
                                     salt: System.Text.Encoding.UTF8.GetBytes(salt),
                                     prf: KeyDerivationPrf.HMACSHA512,
                                     iterationCount: 10000,
                                     numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes) + "æ" + salt;
        }

        public static bool ValidateHash(string pass, string salt, string hash)
        {
            var _hash = HashCreate(pass, salt);
            if (hash == _hash)
            {
                return true;
            }

            return false;
        }


        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using var generator = System.Security.Cryptography.RandomNumberGenerator.Create();
            generator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static List<string> Get(string pass)
        {
            if (!string.IsNullOrEmpty(pass))
            {
                string salt = CreateSalt();
                string hash_salt = HashCreate(pass, salt);

                string hash = hash_salt.Split('æ')[0];
                string _salt = hash_salt.Split('æ')[1];
                string result = ValidateHash(pass, _salt, hash).ToString();

                return new List<string> { hash_salt, result, salt };
            }

            return null;
        }
    }
}
