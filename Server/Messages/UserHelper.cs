using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Server.Messages
{
    public class UserHelper
    {
        public static string GenerateToken(string userId, int secondsToExpirate)
        {
            var tokenInfo = new TokenInfo
            {
                UserId = userId,
                ExpirationDate = DateTime.Now.AddSeconds(secondsToExpirate)
            };

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tokenInfo)));
        }

        public static string ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(json);

            if (tokenInfo == null || tokenInfo?.ExpirationDate <= DateTime.Now)
                return null;

            return tokenInfo.UserId;
        }

        public class TokenInfo
        {
            public string UserId { get; set; }
            public DateTime ExpirationDate { get; set; }
        }

        public static bool PasswordEquals(string purePassword, string encryptedPassword)
        {
            return Encrypt(purePassword)?.Equals(encryptedPassword) ?? false;
        }

        public static string Encrypt(string text)
        {
            string cryptSalt = "1a11a";

            using (var sha256 = SHA256.Create())
            {
                return Encoding.UTF8.GetString(sha256.ComputeHash(Encoding.UTF8.GetBytes($"{cryptSalt}{text}{cryptSalt}")));
            };
        }
    }
}
