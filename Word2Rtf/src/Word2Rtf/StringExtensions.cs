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
        public static string[] Break(this string input)
        {
            return input.Split(new [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();
        }

        public static Language GetLanguage(this string text)
        {
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

            if (numberOfEnglishLetters * 1.0 / totalLength > 0.9)
                return Language.English;
            
            if (numberOfChineseCharacters * 1.0 / totalLength > 0.9)
                return Language.Chinese;
            
            return Language.Mixed;
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

        public static Verse[] FilterByLanguages(this string input)
        {
            var english = new StringBuilder();
            var chinese = new StringBuilder();

            if (input.IsBibleReadingTitle())
            {
                var chineseTerms = input.GetChinese().ToList();
                var englishTerms = input.GetEnglishAndVerseNumber().ToList();
                var verseNumberTerms = input.GetVerseNumbers().ToList();

                BalanceLanguages(chineseTerms, englishTerms);

                if (chineseTerms.Count() != englishTerms.Count())
                    throw new ImbalancedLanguagesException(input);
                
                for(var i = verseNumberTerms.Count(); i > 0; i--)
                {
                    chineseTerms[i] = $"{chineseTerms[i]} {verseNumberTerms[i-1]}";
                }

                chinese.AppendLine(chineseTerms[0]);
                chineseTerms.Skip(1).ToList().ForEach(term => chinese.AppendLine(term));
                englishTerms.ToList().ForEach(term => english.AppendLine(term));
            }
            else
            {
                input.GetChinese().ToList().ForEach(term => chinese.AppendLine(term));
                input.GetEnglishAndVerseNumber().ToList().ForEach(v => english.AppendLine(v));
            }

            return new [] 
            {
                new Verse { Content = english.ToString().Trim(), Language = Language.English },
                new Verse { Content = chinese.ToString().Trim(), Language = Language.Chinese }
            }; 
        }

        public static void BalanceLanguages(List<string> chineseTerms, List<string> englishTerms)
        {
            if (chineseTerms.Count() == englishTerms.Count()) return;

            try
            {

                if (chineseTerms.Count() > englishTerms.Count())
                {
                    for (var i = 1; i < chineseTerms.Count(); i++)
                    {
                        var idx = BookNames.Chinese.IndexOf(chineseTerms[i]);
                        var term = BookNames.English[idx];
                        englishTerms.Insert(i, term);
                    }
                }
                else
                {
                    for (var i = 1; i < englishTerms.Count(); i++)
                    {
                        var idx = BookNames.English.IndexOf(englishTerms[i]);
                        var term = BookNames.Chinese[idx];
                        chineseTerms.Insert(i, term);
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // do nothing
            }
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

        public static bool IsBibleReadingTitle(this string input)
        {
            return (BookNames.Chinese.Any(input.Contains)
                   || BookNames.English.Any(input.Contains)
                   || BookNames.ChineseShortName.Any(input.Contains)
                   || BookNames.EnglishShortName.Any(input.Contains))
                && input.Any(c => c >= '0' && c <= '9')
                && input.All(c => c != '#');
        }

        public static IEnumerable<string> GetEnglishAndVerseNumber(this string input)
        {
            var purified = input.Purify();
            var englishAndVerseNumbersLine = new StringBuilder(string.Concat(purified.Select(c =>
            {
                if (c >= 'a' && c <= 'z'
                    || c >= 'A' && c <= 'Z'
                    || c >= '0' && c <= '9'
                    || c == ':' || c == ',' || c == '-'
                    || c == ';' || c == '；' || c == '、'
                    || c == '【' || c == '】' || c == '#')
                {
                    return c;
                }

                return ' ';
            })));
            
            foreach (var s in BookNames.English)
            {
                englishAndVerseNumbersLine.Replace(s, $"{Environment.NewLine}{s}");
            }
            
            var removeExtraSpaces = englishAndVerseNumbersLine.ToString().RemoveDuplicateSpaces();
            var splitedTitleAndBody = removeExtraSpaces.Split(new [] { '【', '】', ';', '；', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var processed = splitedTitleAndBody.Select(phrase => phrase.Trim(' ', ',', ';'))
                .Where(phrase => !string.IsNullOrWhiteSpace(phrase));
            
            return processed;
        }

        public static IEnumerable<string> GetChinese(this string input)
        {
            var purified = input.Purify();

            return string
                .Concat(purified.Select(c => 
                {
                    if (c >= 'a' && c <= 'z' 
                        || c >= 'A' && c <= 'Z' 
                        || c >= '0' && c <= '9'
                        || c == ':' || c == ',' || c == '-'
                        || c == ';' || c == '；' || c == '、'
                        || c == '【' || c == '】' || c == '#')
                    {
                        return ' ';
                    }
                    return c;
                }))
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);        
        }

        public static IEnumerable<string> GetVerseNumbers(this string input)
        {
            var purified = input.Purify();
            var index = 0;
            if (purified.StartsWith("【"))
            {
                index = purified.IndexOf("】", StringComparison.Ordinal) + 1;
            }
            
            var process = new StringBuilder(purified.Substring(index));


            foreach (var s in BookNames.English)
            {
                process.Replace(s, Environment.NewLine);
            }

            foreach (var s in BookNames.EnglishShortName)
            {
                process.Replace(s, Environment.NewLine);
            }
            
            foreach (var s in BookNames.Chinese)
            {
                process.Replace(s, Environment.NewLine);
            }
            
            foreach (var s in BookNames.ChineseShortName)
            {
                process.Replace(s, Environment.NewLine);
            }

            return process.ToString()
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', ',', ';', '；'));
        }

        public static string RemoveDuplicateSpaces(this string input)
        {
            return string.Join(' ', input.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

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

        public static Language? GetLanguage(this char c)
        {
            if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z') return Language.English;
            if (Char.IsLetter(c) && !(c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')) return Language.Chinese;
            return default(Language?);
        }

        public static bool IsEnglish(this char c)
        {
            return c.GetLanguage() == Language.English;
        }

        public static bool IsChinese(this char c)
        {
            return c.GetLanguage() == Language.Chinese;
        }

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