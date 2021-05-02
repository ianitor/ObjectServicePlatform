using System;
using System.Security.Cryptography;

namespace Ianitor.Osp.Common.Shared
{
  /// <summary>
  /// Implementation more independent Random numbers
  /// </summary>
  public static class RandomGenerator
  {
    private static readonly RandomNumberGenerator Rng = new RNGCryptoServiceProvider();
    private static readonly object Locker = new object();

    /// <summary>
    /// Generate new integer number between minValue and maxValue
    /// </summary>
    /// <param name="minValue">Min limit of random number</param>
    /// <param name="maxValue">Max limit of random number</param>
    /// <returns>New random number</returns>
    public static int NextRandom(int minValue, int maxValue)
    {
      if (minValue > maxValue)
      {
        throw new ArgumentOutOfRangeException(nameof(minValue));
      }

      if (minValue == maxValue)
      {
        return minValue;
      }

      lock (Locker)
      {
        var data = new byte[4];
        Rng.GetBytes(data);

        int generatedValue = Math.Abs(BitConverter.ToInt32(data, startIndex: 0));

        int range = maxValue - minValue;
        int mod = generatedValue % range;
        int normalizedNumber = minValue + mod;

        return normalizedNumber;
      }
    }
  }
}
