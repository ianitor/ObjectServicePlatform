using System;

namespace Ianitor.Osp.Common.Shared
{
    public class Utils
    {
        /// <summary>
        /// Converts a DateTime to UTC (with special handling for MinValue and MaxValue).
        /// </summary>
        /// <param name="dateTime">A DateTime.</param>
        /// <returns>The DateTime in UTC.</returns>
        public static DateTime ToUniversalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            return dateTime == DateTime.MaxValue
                ? DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc)
                : dateTime.ToUniversalTime();
        }

        public static char ToHexChar(int value)
        {
            return (char) (value + (value < 10 ? 48 : 87));
        }

        /// <summary>Parses a hex string into its equivalent byte array.</summary>
        /// <param name="s">The hex string to parse.</param>
        /// <returns>The byte equivalent of the hex string.</returns>
        public static byte[] ParseHexString(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            byte[] bytes;
            if (!TryParseHexString(s, out bytes))
                throw new FormatException("String should contain only hexadecimal digits.");
            return bytes;
        }

        /// <summary>Tries to parse a hex string to a byte array.</summary>
        /// <param name="s">The hex string.</param>
        /// <param name="bytes">A byte array.</param>
        /// <returns>True if the hex string was successfully parsed.</returns>
        public static bool TryParseHexString(string s, out byte[] bytes)
        {
            bytes = (byte[]) null;
            if (s == null)
                return false;
            byte[] numArray = new byte[(s.Length + 1) / 2];
            int num1 = 0;
            int num2 = 0;
            if (s.Length % 2 == 1)
            {
                int num3;
                if (!TryParseHexChar(s[num1++], out num3))
                    return false;
                numArray[num2++] = (byte) num3;
            }

            while (num1 < s.Length)
            {
                string str1 = s;
                int index1 = num1;
                int num3 = index1 + 1;
                int num4;
                if (!TryParseHexChar(str1[index1], out num4))
                    return false;
                string str2 = s;
                int index2 = num3;
                num1 = index2 + 1;
                int num5;
                if (!TryParseHexChar(str2[index2], out num5))
                    return false;
                numArray[num2++] = (byte) (num4 << 4 | num5);
            }

            bytes = numArray;
            return true;
        }

        private static bool TryParseHexChar(char c, out int value)
        {
            if (c >= '0' && c <= '9')
            {
                value = (int) c - 48;
                return true;
            }

            if (c >= 'a' && c <= 'f')
            {
                value = 10 + ((int) c - 97);
                return true;
            }

            if (c >= 'A' && c <= 'F')
            {
                value = 10 + ((int) c - 65);
                return true;
            }

            value = 0;
            return false;
        }
    }
}