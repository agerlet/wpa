using System.Collections.Generic;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal interface IParser
    {
        IEnumerable<Element> Parse(string[] input);
    }
}