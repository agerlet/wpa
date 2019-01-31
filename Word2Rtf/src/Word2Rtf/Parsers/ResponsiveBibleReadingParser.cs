using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class ResponsiveBibleReadingParser : ParserBase<IGrouping<int, Element>>
    {
        public ResponsiveBibleReadingParser() : base() { }

        public override bool CanHandle(IGrouping<int, Element> input)
        {
            return input.First(group => group.ElementType == ElementType.Title)
                        .Input.Contains("Responsive");
        }

        public override void Parse(IGrouping<int, Element> input)
        {
            Elements.Clear();
            Elements.AddRange(input);
        }
    }
}