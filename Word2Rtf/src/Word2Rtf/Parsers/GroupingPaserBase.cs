using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal abstract class GroupingParserBase : ParserBase<IGrouping<int, Element>>
    {
        public GroupingParserBase() : base() { }

        public sealed override void Parse(IGrouping<int, Element> group)
        {
            var title = group.Get(ElementType.Title);
            var content = group.Get(ElementType.Content);

            content = Adjust(content);

            var main = content.Get(Language.English).ToList();
            var overlay = content.Get(Language.Chinese).ToList();

            Adjust(main);
            Adjust(overlay);

            Mix(main, overlay);

            Elements.Clear();
            Elements.AddRange(title);
            Elements.AddRange(main);
        }

        internal virtual void Adjust(List<Element> verses) { }
        internal virtual IEnumerable<Element> Adjust(IEnumerable<Element> verses) 
        {
            return verses;
        }
    }
}