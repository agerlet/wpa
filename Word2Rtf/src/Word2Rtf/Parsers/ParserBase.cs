using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    abstract class ParserBase<T> : IParser<T>
    {
        protected Mixers.MixerFactory _mixerFactory;
        public List<Element> Elements { get; private set; }

        public ParserBase(Mixers.MixerFactory mixerFactory)
        {
            Elements = new List<Element>();
            _mixerFactory = mixerFactory;
        }

        public abstract bool CanHandle(T input);

        public void Flush()
        {
            Elements.Clear();
        }

        public abstract void Parse(T input);
    }
}