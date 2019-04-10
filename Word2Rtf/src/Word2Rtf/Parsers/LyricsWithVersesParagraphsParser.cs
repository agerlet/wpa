using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class LyricsWithVersesParagraphsParser : LyricsParser
    {
        public LyricsWithVersesParagraphsParser(Mixers.MixerFactory mixerFactory) 
            : base(mixerFactory) { }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var isLyrics = base.CanHandle(group);            
            var pattern = GetLanguagePattern(group);
            var languageChangedCount = LanguageChanged(pattern);
            return isLyrics && languageChangedCount > 1;
        }

        internal override IEnumerable<Element> Adjust(IEnumerable<Element> lyrics) 
        { 
            var lines = lyrics.ToList();

            var lastLanguage = lyrics.First().Verses.First().Language;
            var firstSameLanguageElement = lyrics.First();
            for (int i = 1; i < lyrics.Count(); i++)
            {
                if (lines[i].Verses.First().Language != lastLanguage)
                {
                    lastLanguage = lines[i].Verses.First().Language;
                    firstSameLanguageElement = lines[i];
                }
                else
                {
                    firstSameLanguageElement.Input += '\n' + lines[i].Input;
                    firstSameLanguageElement.Verses.First().Content += '\n' + lines[i].Input;
                    lines[i].Input = string.Empty;
                }
            }

            return lyrics.Where(l => !string.IsNullOrWhiteSpace(l.Input)).ToList();
        }
    }
}