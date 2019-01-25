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
        const string _source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26；Luke路2:10b-11,14";

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
            var actual = _source.Purify();
            var expected = "【宣告 Call to worship】 詩篇 Song of Songs50:10,23-26；Luke路2:10b-11,14";
            Assert.Equal(expected, actual);

            actual = "【Hymn唱詩】You Are My All In All《你是我的一切》".Purify();
            expected = "【Hymn唱詩】You Are My All In All 你是我的一切";
            Assert.Equal(expected, actual);

            actual = "【Call To Worship宣告】Luke路2:10b-11,14".Purify();
            expected = "【Call To Worship宣告】Luke路2:10b-11,14";
            Assert.Equal(expected, actual);

            actual = "【Hymn唱詩】SASB82 Joy To The World《普世歡騰》".Purify();
            expected = "【Hymn唱詩】SASB82 Joy To The World 普世歡騰";
            Assert.Equal(expected, actual);

            actual = "【Benediction/Sending祝福/差遣】".Purify();
            expected = "【Benediction Sending祝福 差遣】";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_SplitByLanguage_BibleVerse()
        {
            var actual = _source.SplitByLanguage();

            var expectedLength = 2;
            var expectedVerse1Language = Language.English;
            var expectedVerse2Language = Language.Chinese;
            var expectedVerse1Content = "Call to worship\nSong of Songs50:10,23-26\nLuke 2:10b-11,14";
            var expectedVerse2Content = "宣告\n詩篇 50:10,23-26\n路 2:10b-11,14";

            Assert.Equal(expectedLength, actual.Length);
            Assert.Equal(expectedVerse1Language, actual[0].Language);
            Assert.Equal(expectedVerse2Language, actual[1].Language);
            Assert.Equal(expectedVerse1Content, actual[0].Content);
            Assert.Equal(expectedVerse2Content, actual[1].Content);
        }

        [Fact]
        public void Test_SplitByLanguage_Song()
        {
            var source = "【唱詩/Song】因著十架愛/Love From The Cross";
            var actual = source.SplitByLanguage();

            var expectedLength = 2;
            var expectedVerse1Language = Language.English;
            var expectedVerse2Language = Language.Chinese;
            var expectedVerse1Content = "Song\nLove From The Cross";
            var expectedVerse2Content = "唱詩\n因著十架愛";

            Assert.Equal(expectedLength, actual.Length);
            Assert.Equal(expectedVerse1Language, actual[0].Language);
            Assert.Equal(expectedVerse2Language, actual[1].Language);
            Assert.Equal(expectedVerse1Content, actual[0].Content);
            Assert.Equal(expectedVerse2Content, actual[1].Content);
        }
        [Fact]
        public void Test_IsBibleReadingTitle()
        {
            Assert.True(_source.IsBibleReadingTitle());
            Assert.False("【唱詩/Song】因著十架愛/Love From The Cross".IsBibleReadingTitle());
        }    

        [Fact]
        public void Test_GetEnglishAndVerseNumber()
        {
            var source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26 ;Luke路2:10b-11,14";
            var expected = new [] { "Call to worship", "Song of Songs50:10,23-26", "Luke 2:10b-11,14" };
            var actual = source.GetEnglishAndVerseNumber().ToArray();
            for(var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }

        [Fact]
        public void Test_GetChinese()
        {
            var source = "【Call To Worship宣告】Luke路3:4,6;Habakkuk哈巴谷書 2:20b";
            var expected = new [] { "宣告", "路", "哈巴谷書"};
            var actual = source.GetChinese();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_GetVerseNumbers()
        {
            string source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26;Luke路2:10b-11,14";
            var expected = new [] { "50:10,23-26", "2:10b-11,14" };
            var actual = source.GetVerseNumbers();
            Assert.Equal(expected, actual);
        }
    }
}
