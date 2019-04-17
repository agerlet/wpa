using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected override void Adjust(List<Element> elements)
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

        protected override IEnumerable<Element> Adjust(IEnumerable<Element> elements)
        {
            var list = elements.SelectMany(element => 
                element.Verses.SelectMany(verse => 
                    verse.Content
                        .Replace("(L)", "\n(L)")
                        .Replace("(Leader)", "\n(Leader)")
                        .Replace("(C)", "\n(C)")
                        .Replace("(Congregation)", "\n(Congregation)")
                        .Replace("(領)", "\n(領)")
                        .Replace("(眾)", "\n(眾)")
                        .Replace("(領會)", "\n(領會)")
                        .Replace("(會眾)", "\n(會眾)")
                        .Replace("（L）", "\n（L）")
                        .Replace("（Leader）", "\n（Leader）")
                        .Replace("（C)", "\n（C）")
                        .Replace("（Congregation)", "\n（Congregation）")
                        .Replace("（領）", "\n（領）")
                        .Replace("（眾）", "\n（眾）")
                        .Replace("（領會）", "\n（領會）")
                        .Replace("（會眾）", "\n（會眾）")
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => new Element 
                        {
                            TitleId = element.TitleId,
                            Input = element.Input,
                            ElementType = element.ElementType,
                            Verses = new List<Verse>
                            {
                                new Verse 
                                {
                                    Content = line,
                                    Language = line.GetLanguage()
                                }
                            }
                        })
                    )
                ).ToList();
            return list;
        }
    }
}