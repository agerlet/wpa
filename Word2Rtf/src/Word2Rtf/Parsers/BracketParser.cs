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
    internal class BracketParser : IParser<string[]>
    {
        public bool CanHandle(string[] input)
        {
            return input.Any(line => line.Contains("【"));
        }

        public IEnumerable<Element> Parse(string[] input)
        {
            var elements = input.Select(line => new Element { Input = line }).ToList();

            elements.ForEach(element => 
            {
                if (element.IsTitle()) 
                    element.ParseTitle(); 
                else 
                    ParseContent(element); // Awaiting to be handled by actual handlers
            });

            return elements;
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