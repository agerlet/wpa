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
    class BracketParser : IParser
    {
        public IEnumerable<Element> Parse(string[] input)
        {
            var elements = input.Select(line => new Element { Input = line }).ToList();

            elements.ForEach(element => 
            {
                if (IsTitle(element)) 
                    ParseTitle(element); 
                else 
                    ParseContent(element); 
            });

            return elements;
        }

        bool IsTitle(Element input)
        {
            return input.Input.Contains("【");
        }

        Element ParseTitle(Element element)
        {
            element.ElementType = ElementType.Title;
            element.Verses = element.Input.SplitByLanguage();
            return element;
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