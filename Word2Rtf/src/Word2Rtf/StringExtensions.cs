using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

                ReplaceSimplifiedChineseBookNamesWithTranditionalOnes(chineseTerms);
                
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

        private static void ReplaceSimplifiedChineseBookNamesWithTranditionalOnes(List<string> chineseTerms)
        {
            for (var i = 0; i < chineseTerms.Count(); i++)
            {
                var term = chineseTerms[i];
                var book = C.Books.SingleOrDefault(_ =>
                    _.Versions.Any(x => x.Chinese == Chinese.Simplified && x.Name == term));
                if (book != null)
                {
                    var traditionalChinese = book.Versions.FirstOrDefault(C.GetFullChinese);
                    chineseTerms[i] = traditionalChinese?.Name ?? term;
                }
            }
        }

        public static void BalanceLanguages(List<string> chineseTerms, List<string> englishTerms)
        {
            if (chineseTerms.Count() == englishTerms.Count()) return;

            try
            {
                if (chineseTerms.Count() > englishTerms.Count())
                {
                    var section = C.Sections.FirstOrDefault(n => n.Instances.Any(_ => 
                        _.Content.Equals(chineseTerms[0], StringComparison.InvariantCultureIgnoreCase)));
                    var englishCounterpart = section?.Instances.Where(_ => _.Language == Language.English).ToArray();
                    if (englishCounterpart.Any() &&
                        !englishCounterpart.Join(englishTerms, v => v.Content, s => s, (v, s) => v.Content.Equals(s, StringComparison.InvariantCultureIgnoreCase)).Any())
                    {
                        englishTerms.Insert(0, englishCounterpart.First().Content);
                    }
                    if (chineseTerms.Count() == englishTerms.Count()) return;
                    
                    for (var i = 1; i < chineseTerms.Count(); i++)
                    {
                        var name = C.Books.FirstOrDefault(b =>
                            b.Versions.Any(n => n.Name == chineseTerms[i]))?.Versions?.FirstOrDefault(_ =>
                            _.Language == Language.English && _.Format == NameFormat.Full)?.Name;
                        
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            englishTerms.Insert(i, name);
                        }
                    }
                }
                else
                {
                    var section = C.Sections.FirstOrDefault(x => x.Instances.Any(_ =>
                        _.Content.Equals(englishTerms[0], StringComparison.InvariantCultureIgnoreCase)));
                    var chineseCounterpart = section?.Instances.Where(_ => _.Language == Language.Chinese).ToArray();
                    if (chineseCounterpart.Any() &&
                        !chineseCounterpart.Join(chineseTerms, v => v.Content, s => s, (v, s) => v.Content.Equals(s, StringComparison.InvariantCultureIgnoreCase)).Any())
                    {
                        chineseTerms.Insert(0, chineseCounterpart.First().Content);
                    }
                    if (chineseTerms.Count() == englishTerms.Count()) return;
                    
                    for (var i = 1; i < englishTerms.Count(); i++)
                    {
                        var englishTerm = englishTerms[i];
                        var book = C.Books.FirstOrDefault(_ => _.Versions.Any(b => b.Language == Language.English &&
                                                                                   b.Name.Equals(englishTerm)));
                        if (book == null) continue;
                        
                        var name = book.Versions.First(C.GetFullChinese);
                        chineseTerms.Insert(i, name.Name);
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

        #endregion
        
        
        #region section related
        
        public static bool IsBibleReadingTitle(this string input)
        {
            var doesContainBookName = C.Books.Any(_ => _.Versions.Any(x => 
                                          input.Contains(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                                      && input.Any(c => c >= '0' && c <= '9')
                                      && input.All(c => c != '#');

            var isBibleVerse = C.Sections.Any(_ =>
                _.SectionType.HasFlag(ElementType.BibleVerses)
                && _.Instances.Any(x => input.Contains(x.Content))
            );
            
            return doesContainBookName || isBibleVerse;
        }        

        public static IEnumerable<string> GetEnglishAndVerseNumber(this string input)
        {
            var purified = input.Purify();
            var englishAndVerseNumbersLine = new StringBuilder(string.Concat(purified.Select(c =>
            {
                if (c.IsEnglish() || Char.IsNumber(c)
                    || c == ':' || c == ',' || c == '-'
                    || c == ';' || c == '；' || c == '、'
                    || c == '【' || c == '】' || c == '#'
                    || c == '\'')
                {
                    return c;
                }

                return ' ';
            })));
            
            foreach (var s in C.FullEnglishBookNames)
            {
                englishAndVerseNumbersLine.Replace(s, $"{Environment.NewLine}{s}");
            }
            
            foreach (var s in C.ShortEnglishBookNames)
            {
                englishAndVerseNumbersLine.Replace(s, $"{Environment.NewLine}{s}");
            }

            englishAndVerseNumbersLine.Replace($"Song of {Environment.NewLine}Songs", $"Song of Songs");
            
            var removeExtraSpaces = englishAndVerseNumbersLine.ToString().RemoveDuplicateSpaces();
            var splitedTitleAndBody = removeExtraSpaces.Split(new [] { '【', '】', ';', '；', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var processed = splitedTitleAndBody.Select(phrase => phrase.Trim(' ', ',', ';'))
                .Where(phrase => !string.IsNullOrWhiteSpace(phrase)
                    && phrase.Any(c => c.IsEnglish())
                );
            
            return processed;
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

            foreach (var s in C.FullEnglishBookNames)
            {
                process.Replace(s, Environment.NewLine);
            }

            foreach (var s in C.ShortEnglishBookNames)
            {
                process.Replace(s, Environment.NewLine);
            }
            
            foreach (var s in C.TranditionChineseBookNames)
            {
                process.Replace(s, Environment.NewLine);
            }
            
            foreach (var s in C.ShortChineseBookNames)
            {
                process.Replace(s, Environment.NewLine);
            }

            for (var i = 0; i < process.Length; i++)
            {
                if (process[i].IsChinese()) 
                    process.Replace(process[i].ToString(), Environment.NewLine, i, 1);
            }

            return process.ToString()
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', ',', ';', '；'))
                .Distinct();
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