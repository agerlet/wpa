using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class LyricsWithLanguageParagraphsParser : LyricsWithParagraphsParser
    {
        public LyricsWithLanguageParagraphsParser(Mixers.MixerFactory mixerFactory) : base(mixerFactory) { }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var isLyrics = base.CanHandle(group);            
            var pattern = GetLanguagePattern(group);
            var languageChanged = LanguageChanged(pattern);
            return isLyrics && languageChanged == 1;
        }
    }
}