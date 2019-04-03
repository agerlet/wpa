using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class ResponsiveBibleReadingParser : BibleVerseParser
    {
        public ResponsiveBibleReadingParser() : base() { }

        public override bool CanHandle(IGrouping<int, Element> input)
        {
            var title = input.First(group => group.ElementType == ElementType.Title).Input;

            return title.Contains("Responsive") 
                || title.Contains("啟應讀經");
        }
    }
}