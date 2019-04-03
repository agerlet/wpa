using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Word2Rtf.Models;

[assembly: InternalsVisibleTo("Word2Rtf.Tests")]
namespace Word2Rtf.Parsers
{
    internal static class ParserHandler
    {
        readonly static IParser<Element> _titleParser;
        readonly static IParser<IGrouping<int, Element>>[] _contentParsers;

        static ParserHandler()
        {
            _titleParser = new TitleParser();
            _contentParsers = new IParser<IGrouping<int, Element>>[]
            {
                new BibleVerseParser(),
                new LyricsParser(),
            };
        }

        public static IEnumerable<Element> Parse(this string[] input)
        {
            var handler = new BracketParser();
            handler.Parse(input);
            return handler.Elements.Where(element => element.Pass);
        }

        #region Element processes

        internal static bool IsTitle(this Element element)
        {
            return _titleParser.CanHandle(element);
        }

        internal static Element ParseTitle(this Element title)
        {
            _titleParser.Flush();
            _titleParser.Parse(title);
            title.TitleId = title.GetHashCode();
            return title;
        }

        internal static Element ParseContent(this Element content, int id)
        {
            content.TitleId = id;
            content.ElementType = ElementType.Content;
            content.Verses = new [] 
            {
                new Verse 
                {
                    Language = content.Input.GetLanguage(),
                    Content = content.Input,
                }
            };
            return content;
        }

        #endregion Element processes

        #region IGrouping<int, Element> processes

        internal static IEnumerable<Element> ParseContent(this IGrouping<int, Element> group)
        {
            foreach (var parser in _contentParsers)
            {
                if (parser.CanHandle(group))
                {
                    parser.Flush();
                    parser.Parse(group);
                    return parser.Elements;
                }
            }

            throw new NotImplementedException($"Cannot handle this group {group.First().Input}.");
        }

        internal static IEnumerable<Element> Get(this IGrouping<int, Element> input, ElementType elementType) => 
            input.Where(element => element.ElementType == elementType);

        internal static IEnumerable<Element> Get(this IEnumerable<Element> content, Language language) => 
            content.Where(element => element.Verses.First().Language == language);

        
        #endregion IGrouping<int, Element> processes
    }
}