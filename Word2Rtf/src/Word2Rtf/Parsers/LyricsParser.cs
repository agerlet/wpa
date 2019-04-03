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
            var isHymn = input.Contains("Hymn");
            var isSong = input.Contains("Song");
            return isHymn || isSong;
        }

    }
}