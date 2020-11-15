using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    ///
    /// Assuming there are brackets in the title elements
    ///
    class BracketParser : ParserBase<string[]>
    {
        private readonly ParserHandler _parserHandler;

        public BracketParser(Mixers.MixerFactory mixerFactory, ParserHandler parserHandler) 
            : base(mixerFactory)
        {
            _parserHandler = parserHandler;
        }

        public override bool CanHandle(string[] input)
        {
            return input.Any(line => line.Contains("【"));
        }

        public override void Parse(string[] input)
        {
            Elements.AddRange(input.Select(line => new Element { Input = line }));

            int titleId = new int();

            foreach(var element in Elements)
            {
                if (_parserHandler.IsTitle(element)) 
                    titleId = _parserHandler.ParseTitle(element).TitleId; 
                else
                    _parserHandler.ParseContent(element, titleId);
            }
            
            var elements = Elements
                .GroupBy(element => element.TitleId)
                .SelectMany(group => 
                {
                    if (group.All(g => g.ElementType == ElementType.Title))
                        return group;
                    
                    return _parserHandler.ParseContent(group);
                })
                .ToList();

            Elements.Clear();
            Elements.AddRange(elements);
        }
    }
}