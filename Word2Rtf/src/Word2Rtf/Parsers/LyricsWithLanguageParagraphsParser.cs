using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class LyricsWithLanguageParagraphsParser : LyricsParser
    {
        public LyricsWithLanguageParagraphsParser(Mixers.MixerFactory mixerFactory) 
            : base(mixerFactory) { }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var isLyrics = base.CanHandle(group);

            var pattern = GetLanguagePattern(group);
            var languageChanged = LanguageChanged(pattern);

            var eachElementWithOneVerse = group.Skip(1).All(element => element.Verses.Count() == 1);
            
            var numberOfLinesInChinese = pattern.Count(l => l == Language.Chinese);
            var numberOfLinesInEnglish = pattern.Count(l => l == Language.English);
            var languageLinesMatching = numberOfLinesInChinese == numberOfLinesInEnglish;

            return isLyrics && languageChanged == 1 && eachElementWithOneVerse && languageLinesMatching;
        }
    }
}