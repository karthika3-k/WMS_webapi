using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace TEC_WMS_API.Handler
{
    using System;
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;

    public class PasswordHashHandler
    {
        private static int _interationCount = 100000;
        private static RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

        public static string HashPassword(string password)
        {
            int saltSize = 128 / 8;
            var salt = new byte[saltSize];
            _randomNumberGenerator.GetBytes(salt);

            var subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, _interationCount, 256 / 8);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01;
            WriteNetworkByteOrder(outputBytes, 1, (uint)KeyDerivationPrf.HMACSHA1);
            WriteNetworkByteOrder(outputBytes, 5, (uint)_interationCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + salt.Length, subkey.Length);

            return Convert.ToBase64String(outputBytes);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            try
            {
                var hashPassword = Convert.FromBase64String(hash);
                var keyDerivationPrf = (KeyDerivationPrf)ReadNetworkByteOrder(hashPassword, 1);
                var iterationCount = (int)ReadNetworkByteOrder(hashPassword, 5);
                var saltLength = (int)ReadNetworkByteOrder(hashPassword, 9);

                if (saltLength != 128 / 8) // Ensure the salt length is correct
                {
                    return false;
                }

                var salt = new byte[saltLength];
                Buffer.BlockCopy(hashPassword, 13, salt, 0, salt.Length);
                var storedSubkey = new byte[256 / 8];
                Buffer.BlockCopy(hashPassword, 13 + salt.Length, storedSubkey, 0, storedSubkey.Length);

                // Generate the subkey using the provided password and stored salt
                var generatedSubkey = KeyDerivation.Pbkdf2(password, salt, keyDerivationPrf, iterationCount, 256 / 8);

                // Compare the generated subkey with the stored one
                return AreByteArraysEqual(generatedSubkey, storedSubkey);
            }
            catch (Exception)
            {
                // Handle exceptions (you can log them as needed)
                return false;
            }
        }

        private static void WriteNetworkByteOrder(byte[] outputBytes, int offset, uint value)
        {
            // Write the uint value in big-endian order
            outputBytes[offset] = (byte)(value >> 24);
            outputBytes[offset + 1] = (byte)(value >> 16);
            outputBytes[offset + 2] = (byte)(value >> 8);
            outputBytes[offset + 3] = (byte)(value);
        }

        private static uint ReadNetworkByteOrder(byte[] bytes, int offset)
        {
            // Read the uint value from a byte array in big-endian order
            return (uint)((bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3]);
        }

        private static bool AreByteArraysEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

}
