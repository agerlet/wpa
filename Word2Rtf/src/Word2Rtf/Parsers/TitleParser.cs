
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class TitleParser : ParserBase<Element>
    {
        public TitleParser(Mixers.MixerFactory mixerFactory) : base(mixerFactory) { }

        public override bool CanHandle(Element input)
        {
            return input.Input.Contains("【");
        }

        public override void Parse(Element input)
        {
            input.ElementType = ElementType.Title;
            input.Verses = input.Input.FilterByLanguages();
            Elements.Add(input);
        }
    }
}