using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word2Rtf.Exceptions;
using Word2Rtf.Models;

namespace Word2Rtf
{
    internal static class StringExtensions
    {
        #region pre-process
        
        public static string[] Break(this string input)
        {
            return input.Split(new [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();
        }

        public static string Purify(this string input)
        {
            var purified = new StringBuilder(input.Length);

            char[] toBeRemoved = {'《', '》', '/'};

            foreach(var c in input)
            {
                if (toBeRemoved.Contains(c))
                    purified.Append(' ');
                else if (c == '：')
                    purified.Append(':');
                else if (c == '，')
                    purified.Append(',');
                else if (c == '—')
                    purified.Append('-');
                else
                    purified.Append(c);
            }

            purified.Replace("--", "-");

            return string.Join(' ', 
                purified
                    .ToString()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            );
        }

        public static string RemoveDuplicateSpaces(this string input)
        {
            return string.Join(' ', input.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        
        #endregion
        
        
        #region language related
        
        public static Language GetLanguage(this string text)
        {
            const double threshold = 0.9;
            
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException();

            var input = text.Replace(" ", "");
            var totalLength = input.Length;
            var numberOfEnglishLetters = input.ToCharArray()
                .Count(c => c.IsEnglish()
                            || Char.IsNumber(c)
                            || Char.IsPunctuation(c)
                            || Char.IsWhiteSpace(c));

            var numberOfChineseCharacters = input.ToCharArray()
                .Count(c => c.IsChinese()
                            || Char.IsNumber(c)
                            || Char.IsPunctuation(c));

            if (numberOfEnglishLetters * 1.0 / totalLength > threshold)
                return Language.English;

            if (numberOfChineseCharacters * 1.0 / totalLength > threshold)
                return Language.Chinese;

            return Language.Mixed;
        }

        public static Language? GetLanguage(this char c)
        {
            if (c.IsEnglish()) return Language.English;
            if (c.IsChinese()) return Language.Chinese;
            return default(Language?);
        }

        public static bool IsEnglish(this char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }

        public static bool IsChinese(this char c)
        {
            return Encoding.Unicode
                       .GetBytes(new[] {c})
                       .Count(_ => _ != 0) > 1
                && !char.IsPunctuation(c);
        }

        public static IEnumerable<string> GetChinese(this string input)
        {
            var purified = input.Purify();

            return string
                .Concat(purified.Select(c => 
                {
                    if (c.IsEnglish() || char.IsNumber(c)
                        || c == ':' || c == ',' || c == '-'
                        || c == ';' || c == '；' || c == '、'
                        || c == '【' || c == '】' || c == '#'
                        || c == '\'')
                    {
                        return ' ';
                    }
                    return c;
                }))
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);        
        }
        

        public static IEnumerable<string> SplitByLanguages(this string content)
        {
            var list = new List<string>();
            var builder = new StringBuilder();
            var lastLanguage = content.First(c => c.GetLanguage() != default(Language?)).GetLanguage();

            for(var i = 0; i < content.Length; i ++)
            {
                var c = content[i];
                var currentLanguage = c.GetLanguage();
                if (currentLanguage.HasValue && currentLanguage.Value != lastLanguage.Value)
                {
                    list.Add(builder.ToString());
                    builder.Clear();
                    lastLanguage = currentLanguage.Value;
                }
                if (Char.IsNumber(c))
                {
                    var nextNonWhitespace = content.GetNextLetter(i);
                    if (nextNonWhitespace != default(char)) 
                    {
                        currentLanguage = nextNonWhitespace.GetLanguage();
                        if (currentLanguage != lastLanguage.Value)
                        {
                            list.Add(builder.ToString());
                            builder.Clear();
                            lastLanguage = currentLanguage.Value;
                        }
                    }
                }
                builder.Append(c);
            }

            list.Add(builder.ToString());

            return list;
        }

        #endregion
        
        
        #region section related
            


        public static IEnumerable<string> SplitByVerseNumbers(this string input)
        {
            var temp = new StringBuilder();

            temp.Append(input[0]);
            for(int i = 1; i < input.Length; i ++)
            {
                if (Char.IsNumber(input[i]) && !Char.IsNumber(input[i-1]))
                {
                    temp.Append("\r\n");
                }
                temp.Append(input[i]);
            }

            return temp.ToString()
                .Split(new [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim());

        }        
        
        #endregion

        public static char GetNextLetter(this string line, int start)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Length < start)
                return default(char);

            for(int i = start + 1; i < line.Length; i ++)
            {
                if (Char.IsLetter(line, i))
                    return line[i];
            }

            return default(char);
        }
    } 
}