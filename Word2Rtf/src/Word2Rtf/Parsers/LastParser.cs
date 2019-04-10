using System.Linq;
using Word2Rtf.Mixers;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class LastParser : GroupingParserBase
    {
        public LastParser(MixerFactory mixerFactory) : base(mixerFactory)
        {
        }

        public override bool CanHandle(IGrouping<int, Element> input)
        {
            return true;
        }

        public override void Parse(IGrouping<int, Element> group)
        {
            Elements.Clear();
            Elements.AddRange(group);
        }
    }
}