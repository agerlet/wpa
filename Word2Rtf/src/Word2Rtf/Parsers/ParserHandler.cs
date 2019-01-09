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
        public static IEnumerable<Element> Parse(this string[] input)
        {
            var handlers = new [] { new BracketParser() };
            return handlers.Select(handler => handler.Parse(input))
                           .Where(elements => elements.All(element => element.Pass))
                           .FirstOrDefault();
        }
    }
}