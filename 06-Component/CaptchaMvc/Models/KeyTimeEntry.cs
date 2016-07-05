using System;
using System.Collections.Generic;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Structure to store keys in dictionary in time order.
    /// </summary>
    [Serializable]
    public struct KeyTimeEntry<TKey> : IEquatable<KeyTimeEntry<TKey>>
    {
        #region Equality members

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(KeyTimeEntry<TKey> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key);
        }

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is KeyTimeEntry<TKey> && Equals((KeyTimeEntry<TKey>) obj);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return EqualityComparer<TKey>.Default.GetHashCode(Key);
        }

        #endregion

        #region Fields

        /// <summary>
        ///     Gets the key.
        /// </summary>
        public readonly TKey Key;

        /// <summary>
        ///     Gets the timestamp of the current entry.
        /// </summary>
        public readonly long Timestamp;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public KeyTimeEntry(TKey key)
            : this()
        {
            Key = key;
            Timestamp = DateTime.UtcNow.Ticks;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Implicit conversion operator overload.
        /// </summary>
        public static implicit operator KeyTimeEntry<TKey>(TKey key)
        {
            return new KeyTimeEntry<TKey>(key);
        }

        #endregion
    }
}