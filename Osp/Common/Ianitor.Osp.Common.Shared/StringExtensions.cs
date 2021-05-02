using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ianitor.Osp.Common.Shared
{
    public static class StringExtensions
    {
        // ReSharper disable InconsistentNaming

        public static string EnsureEndsWith(this string _this, string value)
        {
            if (!_this.EndsWith(value))
            {
                return _this + value;
            }

            return _this;
        }

        public static string MakeKey(this string _this)
        {
            return _this.Trim().ToLower();
        }

        /// <summary>
        /// Creates a GraphQL name of the given string
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static string GetGraphQLName(this string _this)
        {
            return _this.Replace(".", "");
        }
        
        public static string ToCamelCase(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? string.Empty : $"{(object) char.ToLowerInvariant(s[0])}{(object) s.Substring(1)}";
        }

        // ReSharper disable once UnusedMember.Global
        public static string ToPascalCase(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? string.Empty : $"{(object) char.ToUpperInvariant(s[0])}{(object) s.Substring(1)}";
        }

        public static string ToConstantCase(this string str)
        {
            return ChangeCase(str, "_", w => w.ToUpperInvariant());
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string ChangeCase(this string str, string sep, Func<string, string> composer)
        {
            return ChangeCase(str, sep, (w, i) => composer(w));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string ChangeCase(this string str, string sep, Func<string, int, string> composer)
        {
            string str1 = "";
            int num = 0;
            foreach (string word in ToWords(str))
                str1 = str1 + (num == 0 ? "" : sep) + composer(word, num++);
            return str1;
        }

        public static string EncodeBase64(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static readonly Regex ReWords =
            new Regex(
                "[A-Z\\xc0-\\xd6\\xd8-\\xde]?[a-z\\xdf-\\xf6\\xf8-\\xff]+(?:['’](?:d|ll|m|re|s|t|ve))?(?=[\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000]|[A-Z\\xc0-\\xd6\\xd8-\\xde]|$)|(?:[A-Z\\xc0-\\xd6\\xd8-\\xde]|[^\\ud800-\\udfff\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000\\d+\\u2700-\\u27bfa-z\\xdf-\\xf6\\xf8-\\xffA-Z\\xc0-\\xd6\\xd8-\\xde])+(?:['’](?:D|LL|M|RE|S|T|VE))?(?=[\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000]|[A-Z\\xc0-\\xd6\\xd8-\\xde](?:[a-z\\xdf-\\xf6\\xf8-\\xff]|[^\\ud800-\\udfff\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000\\d+\\u2700-\\u27bfa-z\\xdf-\\xf6\\xf8-\\xffA-Z\\xc0-\\xd6\\xd8-\\xde])|$)|[A-Z\\xc0-\\xd6\\xd8-\\xde]?(?:[a-z\\xdf-\\xf6\\xf8-\\xff]|[^\\ud800-\\udfff\\xac\\xb1\\xd7\\xf7\\x00-\\x2f\\x3a-\\x40\\x5b-\\x60\\x7b-\\xbf\\u2000-\\u206f \\t\\x0b\\f\\xa0\\ufeff\\n\\r\\u2028\\u2029\\u1680\\u180e\\u2000\\u2001\\u2002\\u2003\\u2004\\u2005\\u2006\\u2007\\u2008\\u2009\\u200a\\u202f\\u205f\\u3000\\d+\\u2700-\\u27bfa-z\\xdf-\\xf6\\xf8-\\xffA-Z\\xc0-\\xd6\\xd8-\\xde])+(?:['’](?:d|ll|m|re|s|t|ve))?|[A-Z\\xc0-\\xd6\\xd8-\\xde]+(?:['’](?:D|LL|M|RE|S|T|VE))?|\\d+|(?:[\\u2700-\\u27bf]|(?:\\ud83c[\\udde6-\\uddff]){2}|[\\ud800-\\udbff][\\udc00-\\udfff])[\\ufe0e\\ufe0f]?(?:[\\u0300-\\u036f\\ufe20-\\ufe23\\u20d0-\\u20f0]|\\ud83c[\\udffb-\\udfff])?(?:\\u200d(?:[^\\ud800-\\udfff]|(?:\\ud83c[\\udde6-\\uddff]){2}|[\\ud800-\\udbff][\\udc00-\\udfff])[\\ufe0e\\ufe0f]?(?:[\\u0300-\\u036f\\ufe20-\\ufe23\\u20d0-\\u20f0]|\\ud83c[\\udffb-\\udfff])?)*");

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<string> ToWords(this string str)
        {
            foreach (Capture match in ReWords.Matches(str))
                yield return match.Value;
        }
        
        // ReSharper restore InconsistentNaming

    }
}