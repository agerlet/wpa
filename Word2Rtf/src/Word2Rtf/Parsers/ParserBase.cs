using System.Collections.Generic;
using System.Linq;
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

        internal void Mix(List<Element> main, List<Element> addon)
        {
            for (int i = 0; i < main.Count(); i++)
            {
                main[i].Input = $"{main[i].Input}\n{addon[i].Input}";
                var verses = main[i].Verses.ToList();
                verses.AddRange(addon[i].Verses);
                main[i].Verses = verses; 
            }
        }
    }
}