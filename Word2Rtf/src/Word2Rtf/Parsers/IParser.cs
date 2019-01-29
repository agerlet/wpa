using System.Collections.Generic;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal interface IParser<T>
    {
        bool CanHandle(T input);
        IEnumerable<Element> Parse(T input);
    }
}