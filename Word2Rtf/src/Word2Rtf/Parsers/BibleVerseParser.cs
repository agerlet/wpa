using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class BibleVerseParser : ParserBase<IGrouping<int, Element>>
    {
        public BibleVerseParser() : base() { }
        
        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var input = group
                .FirstOrDefault(g => g.ElementType == ElementType.Title)
                .Input;
                
            //var isBibleReading = input.Contains("");
            return false;
        }

        public override void Parse(IGrouping<int, Element> group)
        {
            throw new System.NotImplementedException();
        }
    }
}