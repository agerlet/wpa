using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class LyricsWithParagraphsParser : LyricsParser
    {
        public LyricsWithParagraphsParser() : base() { }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var isLyrics = base.CanHandle(group);
            var eachElementWithOneVerse = group.Skip(1).All(element => element.Verses.Count() == 1);
            
            var languagePattern = GetLanguagePattern(group);
            var numberOfLinesInChinese = languagePattern.Count(l => l == Language.Chinese);
            var numberOfLinesInEnglish = languagePattern.Count(l => l == Language.English);
            var languageLinesMatching = numberOfLinesInChinese == numberOfLinesInEnglish;

            return isLyrics && eachElementWithOneVerse && languageLinesMatching;
        }

        protected List<Language> GetLanguagePattern(IGrouping<int, Element> group)
        {
            return group
                .Get(ElementType.Content)
                .Select(element => element.Verses.First().Language).ToList();
        }

        protected int LanguageChanged(IEnumerable<Language> pattern)
        {
            var changed = 0;
            Language? lastLanguage = default(Language?);
            foreach(var l in pattern)
            {    
                if (!lastLanguage.HasValue)
                    lastLanguage = l;
                
                if (lastLanguage.HasValue && lastLanguage.Value != l)
                {
                    lastLanguage = l;
                    changed++;
                }
            }
            return changed;
        }
    }
}