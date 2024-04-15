using System.Security.Cryptography;
using System.Text;

namespace Project.Models
{
    public class PasswordHash
    {



        private const int DefaultKeySize = 64;
        private const int DefaultIterations = 350000;

        static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public static byte[] createSalt(int keySize = DefaultKeySize, int iterations = DefaultIterations)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(keySize);
            return salt;
        }

        public static string HashPassword(string password, byte[] salt)
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,                             
                DefaultIterations,                
                hashAlgorithm,                    
                DefaultKeySize);                 

            return Convert.ToHexString(hash);   
        }


        public static bool VerifyPassword(string password, string storedHash, byte[] salt)
        {
            var testHash = HashPassword(password, salt);
            return CryptographicOperations.FixedTimeEquals(Convert.FromHexString(testHash), Convert.FromHexString(storedHash));
        }

    }
}