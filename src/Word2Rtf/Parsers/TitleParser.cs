
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Analysers;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    class TitleParser : ParserBase<Element>
    {
        private readonly BibleAnalyser _bibleAnalyser;

        public TitleParser(Mixers.MixerFactory mixerFactory, BibleAnalyser bibleAnalyser) 
            : base(mixerFactory)
        {
            _bibleAnalyser = bibleAnalyser;
        }

        public override bool CanHandle(Element input)
        {
            return input.Input.Contains("【");
        }

        public override void Parse(Element input)
        {
            input.ElementType = ElementType.Title;
            input.Verses = _bibleAnalyser.FilterByLanguages(input.Input);
            Elements.Add(input);
        }
    }
}