using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class ResponsiveReadingVersesParser : BibleVerseParser
    {
        public ResponsiveReadingVersesParser(Mixers.MixerFactory mixerFactory) : base(mixerFactory) { }
        
        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var input = group
                .FirstOrDefault(g => g.ElementType == ElementType.Title)
                .Input;
                
            var isResponsiveReading = input.Contains("Responsive Reading", StringComparison.InvariantCultureIgnoreCase)
                                   || input.Contains("啟應讀經", StringComparison.InvariantCultureIgnoreCase)
                                   || input.Contains("啟 應 讀 經", StringComparison.InvariantCultureIgnoreCase);

            return isResponsiveReading;
        }

        internal override void Adjust(List<Element> elements)
        {
            if (elements.Count == 1)
                base.Adjust(elements);

            if (elements.Any(element => element.Verses.Any(verse => 
                false
                || verse.Content.StartsWith("(L)", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("(Leader)", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("(C)", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("(Congregation)", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("(領)")
                || verse.Content.StartsWith("(眾)")
                || verse.Content.StartsWith("(領會)")
                || verse.Content.StartsWith("(會眾)")
            )))
                return;
            if (elements.Any(element => element.Verses.Any(verse => 
                false
                || verse.Content.StartsWith("（L）", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("（Leader）", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("（C）", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("（Congregation）", StringComparison.InvariantCultureIgnoreCase)
                || verse.Content.StartsWith("（領）")
                || verse.Content.StartsWith("（眾）")
                || verse.Content.StartsWith("（領會）")
                || verse.Content.StartsWith("（會眾）")
                )))
                {
                    elements.ForEach(element => {
                        foreach (var verse in element.Verses)
                        {
                            verse.Content = verse.Content
                                                 .Replace("（", "(")
                                                 .Replace("）", ") ")
                                                 .Replace(")  ", ") ")
                                                 ;
                        }
                    });
                    return;
                }

            var isLeading = true;
            elements.ForEach(element => 
            {
                if (isLeading)
                {
                    if (element == elements.Last())
                    {
                        foreach (var verse in element.Verses)
                        {
                            verse.Content = (verse.Language == Language.English
                                            ? "(T) "
                                            : "(合) ") + verse.Content;
                        }
                    }
                    else
                    {                    
                        foreach (var verse in element.Verses)
                        {
                            verse.Content = (verse.Language == Language.English
                                            ? "(L) "
                                            : "(領) ") + verse.Content;
                        }
                    }
                }
                else
                {
                    foreach (var verse in element.Verses)
                    {
                        verse.Content = (verse.Language == Language.English
                                            ? "(C) "
                                            : "(眾) ") + verse.Content;
                    }
                }
                isLeading = !isLeading;
            });
        }
    }
}