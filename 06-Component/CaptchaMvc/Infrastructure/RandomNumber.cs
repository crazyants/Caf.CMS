using System;
using System.Security.Cryptography;

namespace CaptchaMvc.Infrastructure
{
    /// <summary>
    ///     Helper class to generates the random numbers.
    /// </summary>
    public static class RandomNumber
    {
        #region Fields

        private static readonly byte[] Randb = new byte[4];
        private static readonly RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

        #endregion

        #region Methods

        /// <summary>
        ///     Generates a positive random number.
        /// </summary>
        private static int Next()
        {
            Rand.GetBytes(Randb);
            int value = BitConverter.ToInt32(Randb, 0);
            return Math.Abs(value);
        }

        /// <summary>
        ///     Generates a positive random number.
        /// </summary>
        public static int Next(int max)
        {
            return Next()%(max + 1);
        }

        /// <summary>
        ///     Generates a positive random number.
        /// </summary>
        public static int Next(int min, int max)
        {
            return Next(max - min) + min;
        }

        #endregion
    }
}