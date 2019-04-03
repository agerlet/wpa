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
                            Content = verse,
                            Language = verse.GetLanguage()
                        } 
                    }
                })
            );

            elements.AddRange(results);
        }

        private List<string> BreakByNumber(IEnumerable<string> input)
        {
            return input.ToList();
        }
    }
}