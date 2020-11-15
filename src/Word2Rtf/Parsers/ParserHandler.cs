using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Word2Rtf.Analysers;
using Word2Rtf.Mixers;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    public class ParserHandler
    {
        MixerFactory _mixerFactory;
        private readonly BibleAnalyser _bibleAnalyser;

        IParser<Element> _getTitleParser()
        {
            return new TitleParser(_mixerFactory, _bibleAnalyser);
        }

        IParser<IGrouping<int, Element>>[] GetContentParsers()
        {
            return new IParser<IGrouping<int, Element>>[]
            {
                new ResponsiveReadingVersesParser(_mixerFactory, this),
                new MixedLanguagesParser(_mixerFactory, this),
                new BibleVerseSplitedParser(_mixerFactory, this),
                new BibleVerseParser(_mixerFactory, this),
                new LyricsWithVersesParagraphsParser(_mixerFactory, this),
                new LyricsWithLanguageParagraphsParser(_mixerFactory, this),
                new LyricsParser(_mixerFactory, this),
                new LastParser(_mixerFactory, this),
            };
        }

        public ParserHandler(MixerFactory mixerFactory, BibleAnalyser bibleAnalyser)
        {
            _mixerFactory = mixerFactory;
            _bibleAnalyser = bibleAnalyser;
        }

        public IEnumerable<Element> Parse(string[] input)
        {
            var handler = new BracketParser(_mixerFactory, this);
            handler.Parse(input);
            return handler.Elements.Where(element => element.Pass);
        }

        #region Element processes

        internal bool IsTitle(Element element)
        {
            return _getTitleParser().CanHandle(element);
        }

        internal Element ParseTitle(Element title)
        {
            var titleParser = _getTitleParser();
            titleParser.Flush();
            titleParser.Parse(title);
            title.TitleId = title.GetHashCode();
            return title;
        }

        internal void ParseContent(Element content, int id)
        {
            content.TitleId = id;
            content.ElementType = ElementType.Content;
            content.Verses =
                string.IsNullOrWhiteSpace(content.Input)
                ? new Verse[] { }
                : new [] 
                {
                    new Verse 
                    {
                        Language = content.Input.GetLanguage(),
                        Content = content.Input,
                    }
                };
        }

        #endregion Element processes

        #region IGrouping<int, Element> processes

        internal IEnumerable<Element> ParseContent(IGrouping<int, Element> group)
        {
            var contentParsers = GetContentParsers();
            foreach (var parser in contentParsers)
            {
                if (!parser.CanHandle(@group)) continue;
                
                parser.Flush();
                parser.Parse(@group);
                return parser.Elements;
            }

            throw new NotImplementedException($"Cannot handle this group {group.First().Input}.");
        }

        internal IEnumerable<Element> Get(IGrouping<int, Element> input, ElementType elementType) => 
            input.Where(element => element.ElementType == elementType);

        internal IEnumerable<Element> Get(IEnumerable<Element> content, Language language) => 
            content.Where(element => !element.Verses.Any() 
                                  || element.Verses.First().Language == language
                         );

        
        #endregion IGrouping<int, Element> processes
    }
}