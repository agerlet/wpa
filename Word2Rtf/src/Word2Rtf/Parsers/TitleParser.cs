
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class TitleParser : ParserBase<Element>
    {
        public TitleParser() : base() { }

        public override bool CanHandle(Element input)
        {
            return input.Input.Contains("【");
        }

        public override void Parse(Element input)
        {
            input.ElementType = ElementType.Title;
            input.Verses = input.Input.SplitByLanguage();
            Elements.Add(input);
        }
    }
}