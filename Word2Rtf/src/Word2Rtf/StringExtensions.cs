using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Word2Rtf.Models;

namespace Word2Rtf
{
    public static class StringExtensions
    {
        public static string[] Break(this string input)
        {
            return input.Split(new [] { '\r', '\n' })
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();
        }

        public static Language GetLanguage(this string input)
        {
            char[] symbels = new char[] 
            {
                ' ', ';', ',', '(', ')', '!', '.'
            };

            return input.All(c => c >= 'a' && c <= 'z' 
                               || c >= 'A' && c <= 'Z'
                               || c >= '0' && c <= '9'
                               || symbels.Contains(c)) 
                 ? Language.English : Language.Chinese;
        }

        public static string Purify(this string input)
        {
            char[] invalids = new char[]
            {
                '《', '》', '，', '/', '【', '】'
            };
            return string.Concat(input.Select(c => invalids.Contains(c) ? ' ' : (c == '：' ? ':' : c))).Trim();
        }

    } 
}