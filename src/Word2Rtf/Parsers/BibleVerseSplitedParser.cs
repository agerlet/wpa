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

        protected override void Adjust(List<Element> elements) { }
    }
}