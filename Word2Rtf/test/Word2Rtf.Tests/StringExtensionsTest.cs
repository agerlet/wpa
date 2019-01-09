using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using Word2Rtf;
using Word2Rtf.Models;

namespace Word2Rtf.Tests
{
    public class StringExtensionTest
    {
        [Fact]
        public void Test_Break()
        {
            var input = @"line1
            line2
            line3";

            var broken = input.Break();

            Assert.Equal(new [] { "line1", "line2", "line3" }, broken);
        }

        [Fact]
        public void Test_Break_WithEmptyLine()
        {
            var input = @"line1
            line2

            line3";

            var broken = input.Break();

            Assert.Equal(new [] { "line1", "line2", "line3" }, broken);
        }

        [Fact]
        public void Test_Language_TextWithSymbols()
        {
            var actual = "1.Hello(World);".GetLanguage();

            Assert.Equal(Language.English, actual);
        }

        [Fact]
        public void Test_Purify()
        {
            var actual = "【宣告/Proclaim】《詩篇/Psalm 50：23》，《希伯來書/Hebrew 13:15》".Purify();
            var expected = "宣告 Proclaim  詩篇 Psalm 50:23   希伯來書 Hebrew 13:15";
            Assert.Equal(expected, actual);
        }
    }
}
