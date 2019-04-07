using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class LyricsWithLanguageParagraphsParser : LyricsWithParagraphsParser
    {
        public LyricsWithLanguageParagraphsParser() : base() { }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var isLyrics = base.CanHandle(group);            
            var pattern = GetLanguagePattern(group);
            var languageChanged = LanguageChanged(pattern);
            return isLyrics && languageChanged == 1;
        }
    }
}