using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    
    class BibleVerseParser : GroupingParserBase
    {
        public BibleVerseParser(Mixers.MixerFactory mixerFactory) : base(mixerFactory) { }
        
        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var input = group.FirstOrDefault(g => g.ElementType == ElementType.Title);             
            var isBibleVerses = IsBibleVerses(input);
            var isStartWithVerseNumber = startWithVerseNumber(group);
            return isBibleVerses && isStartWithVerseNumber;
        }

        internal override void Adjust(List<Element> elements)
        {
            if (!startWithVerseNumber(elements)) return; 

            var raw = new List<Element>(elements);
            elements.Clear();

            var results = raw.SelectMany(element => 
                element.Input
                       .SplitByVerseNumbers()
                       .Select(verse => new Element 
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

        protected bool startWithVerseNumber(IEnumerable<Element> elements)
        {
            var firstChar = elements
                .FirstOrDefault(g => g.ElementType == ElementType.Content)
                .Input
                .Trim()
                .FirstOrDefault()
                ;

            return Char.IsNumber(firstChar);
        }
    
        protected bool IsBibleVerses(Element input)
        {
            var isCallToWorship = input.Input.Contains("Call To Worship", StringComparison.InvariantCultureIgnoreCase);
            var isTithe = input.Input.Contains("Tithe", StringComparison.InvariantCultureIgnoreCase);
            var isScripture = input.Input.Contains("Scripture", StringComparison.InvariantCultureIgnoreCase);
            var isBibleReading = input.Input.Contains("Bible Reading", StringComparison.InvariantCultureIgnoreCase);
            var isLordsPrayer = input.Input.Contains("主禱文", StringComparison.InvariantCultureIgnoreCase);
            return isCallToWorship || isTithe || isScripture || isBibleReading || isLordsPrayer;
        }
    }
}