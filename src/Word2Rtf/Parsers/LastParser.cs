using System.Linq;
using Word2Rtf.Mixers;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class LastParser : GroupingParserBase
    {
        public LastParser(MixerFactory mixerFactory, ParserHandler parserHandler) 
            : base(mixerFactory, parserHandler)
        {
        }

        public override bool CanHandle(IGrouping<int, Element> input)
        {
            return true;
        }
    }
}