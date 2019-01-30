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
    internal class BracketParser : ParserBase<string[]>
    {
        public BracketParser() : base()
        {
        }

        public override bool CanHandle(string[] input)
        {
            return input.Any(line => line.Contains("【"));
        }

        public override void Parse(string[] input)
        {
            Elements.AddRange(input.Select(line => new Element { Input = line }));

            Elements.ForEach(element => 
            {
                if (element.IsTitle()) 
                    element.ParseTitle(); 
                else 
                    ParseContent(element); // Awaiting to be handled by actual handlers
            });
        }

        Element ParseContent(Element element)
        {
            element.ElementType = ElementType.Content;
            element.Verses = new [] 
            {
                new Verse 
                {
                    Language = element.Input.GetLanguage(),
                    Content = element.Input,
                }
            };
            return element;
        }
    }
}