using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using Word2Rtf.Parsers;
using Word2Rtf.Models;

namespace Word2Rtf.Tests
{
    public class ParserHandlerTests
    {
        [Fact]
        public void Test_BracketParser_EnglishFirst_WithMultipleBibleVerses()
        {
            var input = new [] 
            {
                "【宣告/Proclaim】《詩篇/Psalm 50:23》，《希伯來書/Hebrew 13:15》"
            };
            var element = ParserHandler.Parse(input).FirstOrDefault();
            Assert.NotNull(element);
            Assert.Equal(input.First(), element.Input);
            Assert.True(element.Pass);
            Assert.Equal(ElementType.Title, element.ElementType);
            Assert.Equal("Proclaim\nPsalm 50:23\nHebrew 13:15b", element.Verses.First().Content);
            Assert.Equal(Language.English, element.Verses.First().Language);
            Assert.Equal("宣告\n詩篇 50:23\n希伯來書 13:15b", element.Verses.Last().Content);
            Assert.Equal(Language.Chinese, element.Verses.Last().Language);            
        }

        [Fact]
        public void Test_BracketParser_EnglishLyrics()
        {
            var input = new [] 
            {
                "While fields and floods, "
            };
        } 
    }
}