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

        static ParserHandler()
        {
            _titleParser = new TitleParser();
        }

        public static IEnumerable<Element> Parse(this string[] input)
        {
            var handler = new BracketParser();
            handler.Parse(input);
            return handler.Elements.Where(element => element.Pass);
        }

        internal static bool IsTitle(this Element element)
        {
            return _titleParser.CanHandle(element);
        }

        internal static IEnumerable<Element> ParseTitle(this Element title)
        {
            _titleParser.Flush();
            _titleParser.Parse(title);
            return _titleParser.Elements;
        }
    }
}