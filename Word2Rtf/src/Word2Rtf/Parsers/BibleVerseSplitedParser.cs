using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Mixers;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class BibleVerseSplitedParser : BibleVerseParser
    {
        public BibleVerseSplitedParser(MixerFactory mixerFactory) : base(mixerFactory)
        {
        }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            if (!group.Any()) return false;
            var input = group.First();
            if (input.ElementType != ElementType.Title) return false;
            var isBibleVerses = IsBibleVerses(input);
            var noVerseNumber = !startWithVerseNumber(group);
            return isBibleVerses && noVerseNumber;
        }

        protected override IEnumerable<Element> Adjust(IEnumerable<Element> verses) 
        { 
            var lines = verses.ToList();

            var lastLanguage = verses.First().Verses.First().Language;
            var firstSameLanguageElement = verses.First();
            for (int i = 1; i < verses.Count(); i++)
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

            return verses.Where(l => !string.IsNullOrWhiteSpace(l.Input)).ToList();
        }

        protected override void Adjust(List<Element> elements) { }
    }
}