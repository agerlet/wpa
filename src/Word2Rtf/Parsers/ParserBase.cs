using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    abstract class ParserBase<T> : IParser<T>
    {
        protected readonly Mixers.MixerFactory MixerFactory;
        public List<Element> Elements { get; }

        public ParserBase(Mixers.MixerFactory mixerFactory)
        {
            Elements = new List<Element>();
            MixerFactory = mixerFactory;
        }

        public abstract bool CanHandle(T input);

        public void Flush()
        {
            Elements.Clear();
        }

        public abstract void Parse(T input);
    }
}