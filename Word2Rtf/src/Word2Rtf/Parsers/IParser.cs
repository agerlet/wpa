using System.Collections.Generic;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal interface IParser<T>
    {
        List<Element> Elements { get; }
        bool CanHandle(T input);
        void Parse(T input);
        void Flush();
    }
}