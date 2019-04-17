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

            actual = "Prepare the way for the Lord,make straight paths for him. And all people will see God’s salvation.’ Let all the earth be still before Him. ".GetLanguage();
            Assert.Equal(Language.English, actual);

            actual = "12 大聲說、曾被殺的羔羊、是配得權柄、豐富、智慧、能力、尊貴、榮耀、頌讚的。13我又聽見、在天上、地上、地底下、滄海裡、和天地間一切所有被造之物、都說、但願頌讚、尊貴、榮耀、權勢、都歸給坐寶座的和羔羊、直到永永遠遠。".GetLanguage();
            Assert.Equal(Language.Chinese, actual);

            actual = "25 Lord, save us! Lord, grant us success!25耶和華啊，求你拯救！耶和華啊，求你使我們亨通！".GetLanguage();
            Assert.Equal(Language.Mixed, actual);

            var text = "1Ascribe to the LORD , O mighty ones, ascribe to the LORD glory and strength. 2Ascribe to the LORD the glory due his name; worship the LORD in the splendor of his holiness.3The voice of the LORD is over the waters; the God of glory thunders, the LORD thunders over the mighty waters.4The voice of the LORD is powerful; the voice of the LORD is majestic.5	The voice of the LORD breaks the cedars; the LORD breaks in pieces the cedars of Lebanon.6He makes Lebanon skip like a calf, Sirion like a young wild ox.7	The voice of the LORD strikes with flashes of lightning.8The voice of the LORD shakes the desert; the LORD shakes the Desert of Kadesh.9The voice of the LORD twists the oaks and strips the forests bare. And in his temple all cry, \"Glory!\"10The LORD sits enthroned over the flood; the LORD is enthroned as King forever.11The LORD gives strength to his people; the LORD blesses his people with peace.";
            actual = text.GetLanguage();

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
            var actual = _source.FilterByLanguages();

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
            var actual = source.FilterByLanguages();

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

        [Fact]
        public void Test_BreakByVerseNumbers()
        {
            string source = "1神的眾子阿，你們要將榮耀能力，歸給耶和華，歸給耶和華。12. 要將耶和華的名所當得的榮耀歸給他，以聖潔的妝飾敬拜耶和華。123. 耶和華的聲音發在水上，榮耀的神打雷，耶和華打雷在大水之上。";
            var expected = new [] 
            {
                "1神的眾子阿，你們要將榮耀能力，歸給耶和華，歸給耶和華。",
                "12. 要將耶和華的名所當得的榮耀歸給他，以聖潔的妝飾敬拜耶和華。",
                "123. 耶和華的聲音發在水上，榮耀的神打雷，耶和華打雷在大水之上。"
            };

            var actual = source.SplitByVerseNumbers().ToArray();
            for (int i = 0; i < expected.Length; i ++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }

        [Fact]
        public void Mixed_languages()
        {
            string input = "25 Lord, save us! Lord, grant us success!25耶和華啊，求你拯救！耶和華啊，求你使我們亨通！";
            Assert.Equal(Language.Mixed, input.GetLanguage());
            input = "25 Lord, save us! Lord, grant us success!";
            Assert.Equal(Language.English, input.GetLanguage());
            input = "25耶和華啊，求你拯救！耶和華啊，求你使我們亨通！";
            Assert.Equal(Language.Chinese, input.GetLanguage());
        }

        [Fact]
        public void Test_the_dotnet_core_api()
        {
            Assert.True(Char.IsPunctuation('，'));
            Assert.True(Char.IsPunctuation('。'));
            Assert.True(Char.IsPunctuation('！'));
            Assert.True(Char.IsPunctuation('：'));
            Assert.True(Char.IsPunctuation('“'));
            Assert.True(Char.IsPunctuation('”'));
            Assert.True(Char.IsPunctuation('【'));

            var input = "耶和華啊，求你拯救！耶和華啊，求你使我們亨通！";
            foreach(var c in input)
            {
                Assert.True(Char.IsPunctuation(c) || Char.IsLetter(c));
            }
        }

        [Fact]
        public void Default_Char()
        {
            Assert.Equal('\0', default(char));
        }
    }
}
