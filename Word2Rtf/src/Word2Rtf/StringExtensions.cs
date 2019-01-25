using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var purified = new StringBuilder(input.Length);

            char[] toBeRemoved = new char[]
            {
                '《', '》', '/', 
            };

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

        public static Verse[] SplitByLanguage(this string input)
        {
            // sample input: 
            //      【宣告 Call to worship】 詩篇 Psalm 50:10,23-26 ,Luke路2:10b-11,14
            var purified = input.Purify();
            var english = new StringBuilder();
            var chinese = new StringBuilder();

            if (input.IsBibleReadingTitle())
            {
                var chineseTerms = input.GetChinese().ToArray();
                var englishTerms = input.GetEnglishAndVerseNumber();
                var verseNumberTerms = input.GetVerseNumbers().ToArray();
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
                new Verse { Content = chinese.ToString().Trim(), Language = Language.Chinese },
            }; 
        }

        public static bool IsBibleReadingTitle(this string input)
        {
            return input.Any(c => c >= '0' && c <= '9' || c == ':');
        }

        public static IEnumerable<string> GetEnglishAndVerseNumber(this string input)
        {
            var purified = input.Purify();
            var englishAndVerseNumbersLine = string.Concat(purified.Select(c => 
            {
                if (c >= 'a' && c <= 'z' 
                    || c >= 'A' && c <= 'Z' 
                    || c >= '0' && c <= '9'
                    || c == ':' || c == ',' || c == '-' 
                    || c == ';' || c == '；'
                    || c == '【' || c == '】')
                {
                    return c;
                }
                return ' ';
            }));
            var removeExtraSpaces = englishAndVerseNumbersLine.RemoveDuplicateSpaces();
            var splitedTitleAndBody = removeExtraSpaces.Split(new [] { '【', '】', ';', '；' }, StringSplitOptions.RemoveEmptyEntries);
            var processed = splitedTitleAndBody.Select(phrase => phrase.Trim());
            
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
                        || c == ';' || c == '；' 
                        || c == '【' || c == '】')
                    {
                        return ' ';
                    }
                    return c;
                }))
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);        
        }

        public static IEnumerable<string> GetVerseNumbers(this string input)
        {
            var verses = input.GetEnglishAndVerseNumber().Skip(1); // skip the title

            var filtered = string.Concat(verses.Select(verse => 
                string.Concat(verse.Select(term => 
                    term >= 'a' && term <= 'z' || term >= 'A' && term <= 'Z'
                    ? term
                    : ' '
                ))
            ));

            var pureEnglish = filtered.RemoveDuplicateSpaces()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(term => term.Length > 1) // this might lost "a"
                .OrderByDescending(term => term.Length)
                .ToList();

            var verseNumbers = verses.Select(verse => {
                var withoutEnglishWords = new StringBuilder(verse);
                foreach (var word in pureEnglish)
                {
                    withoutEnglishWords.Replace(word, " ");
                }

                var verseNumber = string.Concat(withoutEnglishWords
                    .ToString()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(term => term.Length > 1)
                );
                
                return verseNumber;
            });

            return verseNumbers;
        }

        public static string RemoveDuplicateSpaces(this string input)
        {
            return string.Join(' ', input.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    } 
}