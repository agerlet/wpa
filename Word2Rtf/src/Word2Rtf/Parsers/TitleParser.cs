
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class TitleParser : IParser<Element>
    {
        public bool CanHandle(Element input)
        {
            return input.Input.Contains("【");
        }

        public IEnumerable<Element> Parse(Element input)
        {
            input.ElementType = ElementType.Title;
            input.Verses = input.Input.SplitByLanguage();
            return new [] { input };
        }
    }
}