using System.Collections.Generic;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal abstract class ParserBase<T> : IParser<T>
    {
        public List<Element> Elements { get; private set; }

        public ParserBase()
        {
            Elements = new List<Element>();
        }

        public abstract bool CanHandle(T input);

        public void Flush()
        {
            Elements.Clear();
        }

        public abstract void Parse(T input);
    }
}