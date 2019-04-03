using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class BibleVerseParser : GroupingParserBase
    {
        public BibleVerseParser() : base() { }
        
        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var input = group
                .FirstOrDefault(g => g.ElementType == ElementType.Title)
                .Input;
                
            var isCallToWorship = input.Contains("Call To Worship", StringComparison.InvariantCultureIgnoreCase);
            var isTithe = input.Contains("Tithe", StringComparison.InvariantCultureIgnoreCase);
            var isScripture = input.Contains("Scripture", StringComparison.InvariantCultureIgnoreCase);
            var isBibleReading = input.Contains("Bible Reading", StringComparison.InvariantCultureIgnoreCase);
            var isLordsPrayer = input.Contains("主禱文", StringComparison.InvariantCultureIgnoreCase);

            return isCallToWorship || isTithe || isScripture || isLordsPrayer;
        }

        internal virtual bool IsBibleVerse(Element element)
        {
            return Char.IsNumber(element.Input.Trim().First());
        }
    }
}