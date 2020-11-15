using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word2Rtf.Exceptions;
using Word2Rtf.Models;

namespace Word2Rtf.Analysers
{
    public class BibleAnalyser
    {
        private readonly Bible _bible;

        public BibleAnalyser(Bible bible)
        {
            _bible = bible;
        }

        public bool IsBibleReadingTitle(string input)
        {
            var doesContainBookName = _bible.Books.Any(_ => _.Versions.Any(x =>
                                          input.Contains(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                                      && input.Any(c => c >= '0' && c <= '9')
                                      && input.All(c => c != '#');

            var isBibleVerse = _bible.Sections.Any(_ =>
                _.SectionType.HasFlag(ElementType.BibleVerses)
                && _.Instances.Any(x => input.Contains(x.Content))
            );

            return doesContainBookName || isBibleVerse;
        }

        public IEnumerable<string> GetEnglishAndVerseNumber(string input)
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
            
            foreach (var s in _bible.FullEnglishBookNames)
            {
                englishAndVerseNumbersLine.Replace(s, $"{Environment.NewLine}{s}");
            }
            
            foreach (var s in _bible.ShortEnglishBookNames)
            {
                englishAndVerseNumbersLine.Replace(s, $"{Environment.NewLine}{s}");
            }

            englishAndVerseNumbersLine.Replace($"Song of {Environment.NewLine}Songs", $"Song of Songs");
            englishAndVerseNumbersLine.Replace($"1 {Environment.NewLine}{Environment.NewLine}John", "1 John");
            englishAndVerseNumbersLine.Replace($"2 {Environment.NewLine}{Environment.NewLine}John", "2 John");
            englishAndVerseNumbersLine.Replace($"3 {Environment.NewLine}{Environment.NewLine}John", "3 John");

            var removeExtraSpaces = englishAndVerseNumbersLine.ToString().RemoveDuplicateSpaces();
            var splitedTitleAndBody = removeExtraSpaces.Split(new [] { '【', '】', ';', '；', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var processed = splitedTitleAndBody.Select(phrase => phrase.Trim(' ', ',', ';'))
                .Where(phrase => !string.IsNullOrWhiteSpace(phrase)
                                 && phrase.Any(c => c.IsEnglish())
                );
            
            return processed;
        }
        
        public IEnumerable<string> GetVerseNumbers(string input)
        {
            var purified = input.Purify();
            var index = 0;
            if (purified.StartsWith("【"))
            {
                index = purified.IndexOf("】", StringComparison.Ordinal) + 1;
            }
            
            var process = new StringBuilder(purified.Substring(index));

            foreach (var s in _bible.FullEnglishBookNames)
            {
                process.Replace(s, Environment.NewLine);
            }

            foreach (var s in _bible.ShortEnglishBookNames)
            {
                process.Replace(s, Environment.NewLine);
            }
            
            foreach (var s in _bible.TranditionChineseBookNames)
            {
                process.Replace(s, Environment.NewLine);
            }
            
            foreach (var s in _bible.ShortChineseBookNames)
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
        
        public Verse[] FilterByLanguages(string input)
        {
            var english = new StringBuilder();
            var chinese = new StringBuilder();

            if (IsBibleReadingTitle(input))
            {
                var chineseTerms = input.GetChinese().ToList();
                var englishTerms = GetEnglishAndVerseNumber(input).ToList();
                var verseNumberTerms = GetVerseNumbers(input).ToList();

                BalanceLanguages(chineseTerms, englishTerms);
                if (chineseTerms.Count() != englishTerms.Count())
                    throw new ImbalancedLanguagesException(input);

                ReplaceSimplifiedChineseBookNamesWithTraditionalOnes(chineseTerms);
                
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
                GetEnglishAndVerseNumber(input).ToList().ForEach(v => english.AppendLine(v));
            }

            return new [] 
            {
                new Verse { Content = english.ToString().Trim(), Language = Language.English },
                new Verse { Content = chinese.ToString().Trim(), Language = Language.Chinese }
            }; 
        }

        private void ReplaceSimplifiedChineseBookNamesWithTraditionalOnes(IList<string> chineseTerms)
        {
            for (var i = 0; i < chineseTerms.Count(); i++)
            {
                var term = chineseTerms[i];
                var book = _bible.Books.SingleOrDefault(_ =>
                    _.Versions.Any(x => x.Chinese == Chinese.Simplified && x.Name == term));
                if (book == null) continue;

                var traditionalChinese = book.Versions.FirstOrDefault(Bible.FullChineseFilter);
                chineseTerms[i] = traditionalChinese?.Name ?? term;
            }
        }


        public void BalanceLanguages(List<string> chineseTerms, List<string> englishTerms)
        {
            if (chineseTerms.Count() == englishTerms.Count()) return;

            try
            {
                if (chineseTerms.Count() > englishTerms.Count())
                {
                    var section = _bible.Sections.FirstOrDefault(n => n.Instances.Any(_ =>
                        _.Content.Equals(chineseTerms[0], StringComparison.InvariantCultureIgnoreCase)));
                    var englishCounterpart = section?.Instances.Where(_ => _.Language == Language.English).ToArray();
                    if (englishCounterpart.Any() &&
                        !englishCounterpart.Join(englishTerms, v => v.Content, s => s,
                            (v, s) => v.Content.Equals(s, StringComparison.InvariantCultureIgnoreCase)).Any())
                    {
                        englishTerms.Insert(0, englishCounterpart.First().Content);
                    }

                    if (chineseTerms.Count() == englishTerms.Count()) return;

                    for (var i = 1; i < chineseTerms.Count(); i++)
                    {
                        var name = _bible.Books.FirstOrDefault(b =>
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
                    var section = _bible.Sections.FirstOrDefault(x => x.Instances.Any(_ =>
                        _.Content.Equals(englishTerms[0], StringComparison.InvariantCultureIgnoreCase)));
                    var chineseCounterpart = section?.Instances.Where(_ => _.Language == Language.Chinese).ToArray();
                    if (chineseCounterpart.Any() &&
                        !chineseCounterpart.Join(chineseTerms, v => v.Content, s => s,
                            (v, s) => v.Content.Equals(s, StringComparison.InvariantCultureIgnoreCase)).Any())
                    {
                        chineseTerms.Insert(0, chineseCounterpart.First().Content);
                    }

                    if (chineseTerms.Count() == englishTerms.Count()) return;

                    for (var i = 1; i < englishTerms.Count(); i++)
                    {
                        var englishTerm = englishTerms[i];
                        var book = _bible.Books.FirstOrDefault(_ =>
                            _.Versions.Any(b => b.Language == Language.English &&
                                                b.Name.Equals(englishTerm)));
                        if (book == null) continue;

                        var name = book.Versions.First(Bible.FullChineseFilter);
                        chineseTerms.Insert(i, name.Name);
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // do nothing
            }
        }

    }
}