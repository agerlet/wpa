using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class LyricsParser : GroupingParserBase
    {
        public LyricsParser() : base() { }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var input = group.First().Input;
            var isHymn = input.Contains("Hymn", StringComparison.InvariantCultureIgnoreCase);
            var isSong = input.Contains("Song", StringComparison.InvariantCultureIgnoreCase);
            var isMercySeat = input.Contains("Mercy Seat Appeal", StringComparison.InvariantCultureIgnoreCase);
            return isHymn || isSong || isMercySeat;
        }

    }
}