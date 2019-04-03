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
            var isResponsiveReading = input.Contains("Responsive", StringComparison.InvariantCultureIgnoreCase)
                                   || input.Contains("啟應讀經", StringComparison.InvariantCultureIgnoreCase);

            return isCallToWorship || isTithe || isScripture || isLordsPrayer || isResponsiveReading;
        }

        internal override void Adjust(List<Element> elements)
        {
            var raw = new List<Element>(elements);
            elements.Clear();

            var results = raw.SelectMany(element => 
                element.Input.SplitByVerseNumbers().Select(verse => new Element 
                {
                    Input = verse,
                    TitleId = element.TitleId,
                    ElementType = element.ElementType,
                    Verses = new List<Verse> 
                    {
                        new Verse 
                        {
                            Content = verse.Trim(),
                            Language = verse.GetLanguage()
                        } 
                    }
                })
            );

            elements.AddRange(results);
        }
    }
}