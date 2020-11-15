using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word2Rtf.Mixers;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class MixedLanguagesParser : GroupingParserBase
    {
        public MixedLanguagesParser(MixerFactory mixerFactory, ParserHandler parserHandler) 
            : base(mixerFactory, parserHandler)
        {
        }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            if (group.Count() < 2) return false;
            if (group.First().ElementType != ElementType.Title) return false;

            var content = group.Skip(1).ToList();
            var isMixed = content.All(element => element.Input.GetLanguage() == Language.Mixed);
            return isMixed;
        }

        protected override IEnumerable<Element> Adjust(IEnumerable<Element> elements)
        {
            return base.Adjust(elements).SelectMany(element =>
                element.Verses.SelectMany(verse => 
                    verse.Content.SplitByLanguages()
                        .Select(line => line.Trim())
                        .Select(line => 
                            new Element 
                            {
                                Input = line,
                                TitleId = element.TitleId,
                                ElementType = ElementType.Content,
                                Verses = new List<Verse> 
                                {
                                    new Verse 
                                    {
                                        Content = line,
                                        Language = line.GetLanguage(),
                                    }
                                },
                            })
                )
            ).ToList();
        }
    }
}