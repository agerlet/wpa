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
                "【宣告/Proclaim】《詩篇/Psalm 50:23》;《希伯來書/Hebrew 13:15b》"
            };
            var element = ParserHandler.Parse(input).FirstOrDefault();
            Assert.NotNull(element);
            Assert.Equal(input.First(), element.Input);
            Assert.True(element.Pass);
            Assert.Equal(2, element.Verses.Count());
            Assert.Equal(ElementType.Title, element.ElementType);
            Assert.Equal("Proclaim\nPsalm 50:23\nHebrew 13:15b", element.Verses.First().Content);
            Assert.Equal(Language.English, element.Verses.First().Language);
            Assert.Equal("宣告\n詩篇 50:23\n希伯來書 13:15b", element.Verses.Last().Content);
            Assert.Equal(Language.Chinese, element.Verses.Last().Language);            
        }

        [Fact]
        public void Test_BracketParser_BibleVerses_ResponsiveReading()
        {
            var title = "【Responsive Reading啟應讀經】詩篇 Psalm 29";
            var chinese = "1神的眾子阿，你們要將榮耀能力，歸給耶和華，歸給耶和華。2. 要將耶和華的名所當得的榮耀歸給他，以聖潔的妝飾敬拜耶和華。3. 耶和華的聲音發在水上，榮耀的神打雷，耶和華打雷在大水之上。4. 耶和華的聲音大有能力，耶和華的聲音滿有威嚴。5. 耶和華的聲音震破香柏樹，耶和華震碎利巴嫩的香柏樹。6. 他也使之跳躍如牛犢，使利巴嫩和西連跳躍如野牛犢。7. 耶和華的聲音使火焰分岔。8. 耶和華的聲音震動曠野，耶和華震動加低斯的曠野。9. 耶和華的聲音驚動母鹿落胎，樹木也脫落淨光。凡在他殿中的，都稱說他的榮耀。10. 洪水泛濫之時，耶和華坐著為王，耶和華坐著為王，直到永遠。11\"耶和華必賜力量給他的百姓，耶和華必賜平安的福給他的百姓。\"";
            var english = "1Ascribe to the LORD , O mighty ones, ascribe to the LORD glory and strength. 2Ascribe to the LORD the glory due his name; worship the LORD in the splendor of his holiness.3The voice of the LORD is over the waters; the God of glory thunders, the LORD thunders over the mighty waters.4The voice of the LORD is powerful; the voice of the LORD is majestic.5	The voice of the LORD breaks the cedars; the LORD breaks in pieces the cedars of Lebanon.6He makes Lebanon skip like a calf, Sirion like a young wild ox.7	The voice of the LORD strikes with flashes of lightning.8The voice of the LORD shakes the desert; the LORD shakes the Desert of Kadesh.9The voice of the LORD twists the oaks and strips the forests bare. And in his temple all cry, \"Glory!\"10The LORD sits enthroned over the flood; the LORD is enthroned as King forever.11The LORD gives strength to his people; the LORD blesses his people with peace.";
            var input = new [] { title, chinese, english };

            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(12, elements.Count());

            Assert.True(elements[0].Pass);
            Assert.Equal(title, elements[0].Input);
            Assert.Equal(2, elements[0].Verses.Count());
            Assert.Equal("Responsive Reading\nPsalm 29", elements[0].Verses.First().Content);
            Assert.Equal("啟應讀經\n詩篇 29", elements[0].Verses.Skip(1).First().Content);

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("(L) 1Ascribe to the LORD , O mighty ones, ascribe to the LORD glory and strength.", elements[1].Verses.First().Content);
            Assert.Equal("(領) 1神的眾子阿，你們要將榮耀能力，歸給耶和華，歸給耶和華。", elements[1].Verses.Skip(1).First().Content);

            Assert.True(elements[2].Pass);
            Assert.Equal(2, elements[2].Verses.Count());
            Assert.Equal("(C) 2Ascribe to the LORD the glory due his name; worship the LORD in the splendor of his holiness.", elements[2].Verses.First().Content);
            Assert.Equal("(眾) 2. 要將耶和華的名所當得的榮耀歸給他，以聖潔的妝飾敬拜耶和華。", elements[2].Verses.Skip(1).First().Content);

            Assert.True(elements[3].Pass);
            Assert.Equal(2, elements[3].Verses.Count());
            Assert.Equal("(L) 3The voice of the LORD is over the waters; the God of glory thunders, the LORD thunders over the mighty waters.", elements[3].Verses.First().Content);
            Assert.Equal("(領) 3. 耶和華的聲音發在水上，榮耀的神打雷，耶和華打雷在大水之上。", elements[3].Verses.Skip(1).First().Content);

            Assert.True(elements[4].Pass);
            Assert.Equal(2, elements[4].Verses.Count());
            Assert.Equal("(C) 4The voice of the LORD is powerful; the voice of the LORD is majestic.", elements[4].Verses.First().Content);
            Assert.Equal("(眾) 4. 耶和華的聲音大有能力，耶和華的聲音滿有威嚴。", elements[4].Verses.Skip(1).First().Content);

            Assert.True(elements[5].Pass);
            Assert.Equal(2, elements[5].Verses.Count());
            Assert.Equal("(L) 5	The voice of the LORD breaks the cedars; the LORD breaks in pieces the cedars of Lebanon.", elements[5].Verses.First().Content);
            Assert.Equal("(領) 5. 耶和華的聲音震破香柏樹，耶和華震碎利巴嫩的香柏樹。", elements[5].Verses.Skip(1).First().Content);

            Assert.True(elements[6].Pass);
            Assert.Equal(2, elements[6].Verses.Count());
            Assert.Equal("(C) 6He makes Lebanon skip like a calf, Sirion like a young wild ox.", elements[6].Verses.First().Content);
            Assert.Equal("(眾) 6. 他也使之跳躍如牛犢，使利巴嫩和西連跳躍如野牛犢。", elements[6].Verses.Skip(1).First().Content);

            Assert.True(elements[7].Pass);
            Assert.Equal(2, elements[7].Verses.Count());
            Assert.Equal("(L) 7	The voice of the LORD strikes with flashes of lightning.", elements[7].Verses.First().Content);
            Assert.Equal("(領) 7. 耶和華的聲音使火焰分岔。", elements[7].Verses.Skip(1).First().Content);

            Assert.True(elements[8].Pass);
            Assert.Equal(2, elements[4].Verses.Count());
            Assert.Equal("(C) 8The voice of the LORD shakes the desert; the LORD shakes the Desert of Kadesh.", elements[8].Verses.First().Content);
            Assert.Equal("(眾) 8. 耶和華的聲音震動曠野，耶和華震動加低斯的曠野。", elements[8].Verses.Skip(1).First().Content);

            Assert.True(elements[9].Pass);
            Assert.Equal(2, elements[9].Verses.Count());
            Assert.Equal("(L) 9The voice of the LORD twists the oaks and strips the forests bare. And in his temple all cry, \"Glory!\"", elements[9].Verses.First().Content);
            Assert.Equal("(領) 9. 耶和華的聲音驚動母鹿落胎，樹木也脫落淨光。凡在他殿中的，都稱說他的榮耀。", elements[9].Verses.Skip(1).First().Content);

            Assert.True(elements[10].Pass);
            Assert.Equal(2, elements[10].Verses.Count());
            Assert.Equal("(C) 10The LORD sits enthroned over the flood; the LORD is enthroned as King forever.", elements[10].Verses.First().Content);
            Assert.Equal("(眾) 10. 洪水泛濫之時，耶和華坐著為王，耶和華坐著為王，直到永遠。", elements[10].Verses.Skip(1).First().Content);

            Assert.True(elements[11].Pass);
            Assert.Equal(2, elements[11].Verses.Count());
            Assert.Equal("(T) 11The LORD gives strength to his people; the LORD blesses his people with peace.", elements[11].Verses.First().Content);
            Assert.Equal("(合) 11\"耶和華必賜力量給他的百姓，耶和華必賜平安的福給他的百姓。\"", elements[11].Verses.Skip(1).First().Content);
        } 

        [Fact]
        public void Test_BracketParser_BibleVerses_ResponsiveReading_already_marked()
        {
            var input = new [] { 
                "【Responsive Reading啟應讀經】詩篇 Psalm 29", 
                "(L) 1Ascribe to the LORD , O mighty ones, ascribe to the LORD glory and strength.",
                "（領） 1神的眾子阿，你們要將榮耀能力，歸給耶和華，歸給耶和華。", 
                "(C) 2Ascribe to the LORD the glory due his name; worship the LORD in the splendor of his holiness.",
                "（眾）2. 要將耶和華的名所當得的榮耀歸給他，以聖潔的妝飾敬拜耶和華。"
                };

            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(3, elements.Count());

            Assert.True(elements[0].Pass);
            Assert.Equal("【Responsive Reading啟應讀經】詩篇 Psalm 29", elements[0].Input);
            Assert.Equal(2, elements[0].Verses.Count());
            Assert.Equal("Responsive Reading\nPsalm 29", elements[0].Verses.First().Content);
            Assert.Equal("啟應讀經\n詩篇 29", elements[0].Verses.Skip(1).First().Content);

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("(L) 1Ascribe to the LORD , O mighty ones, ascribe to the LORD glory and strength.", elements[1].Verses.First().Content);
            Assert.Equal("(領) 1神的眾子阿，你們要將榮耀能力，歸給耶和華，歸給耶和華。", elements[1].Verses.Skip(1).First().Content);

            Assert.True(elements[2].Pass);
            Assert.Equal(2, elements[2].Verses.Count());
            Assert.Equal("(C) 2Ascribe to the LORD the glory due his name; worship the LORD in the splendor of his holiness.", elements[2].Verses.First().Content);
            Assert.Equal("(眾) 2. 要將耶和華的名所當得的榮耀歸給他，以聖潔的妝飾敬拜耶和華。", elements[2].Verses.Skip(1).First().Content);
        }
        [Fact]
        public void BibleVersesParser_WithUnknownVerseNumbers() 
        {
            var input = new [] 
            {
                "【Call To Worship宣告】Luke路2:10b-11,14",
                "I bring you good news that will cause great joy for all the people. 11 Today in the town of David a Savior has been born to you; he is the Messiah, the Lord. “Glory to God in the highest heaven, and on earth peace to those on whom his favor rests.”",
                "我報給你們大喜的信息、 是關乎萬民的.因今天在大衛的城裡、 為你們生了救主、 就是主基督。在至高之處榮耀歸與上帝!在地上平安歸 與祂所喜悅的人!"
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(2, elements.Count());

            Assert.True(elements[0].Pass);
            Assert.Equal("【Call To Worship宣告】Luke路2:10b-11,14", elements[0].Input);
            Assert.Equal(2, elements[0].Verses.Count());
            Assert.Equal("Call To Worship\nLuke 2:10b-11,14", elements[0].Verses.First().Content);
            Assert.Equal("宣告\n路 2:10b-11,14", elements[0].Verses.Skip(1).First().Content);

            Assert.True(elements[1].Pass);
            Assert.Equal("I bring you good news that will cause great joy for all the people. 11 Today in the town of David a Savior has been born to you; he is the Messiah, the Lord. “Glory to God in the highest heaven, and on earth peace to those on whom his favor rests.”\n我報給你們大喜的信息、 是關乎萬民的.因今天在大衛的城裡、 為你們生了救主、 就是主基督。在至高之處榮耀歸與上帝!在地上平安歸 與祂所喜悅的人!", elements[1].Input);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("I bring you good news that will cause great joy for all the people. 11 Today in the town of David a Savior has been born to you; he is the Messiah, the Lord. “Glory to God in the highest heaven, and on earth peace to those on whom his favor rests.”", elements[1].Verses.First().Content);
            Assert.Equal("我報給你們大喜的信息、 是關乎萬民的.因今天在大衛的城裡、 為你們生了救主、 就是主基督。在至高之處榮耀歸與上帝!在地上平安歸 與祂所喜悅的人!", elements[1].Verses.Skip(1).First().Content);
        }
        [Fact]
        public void LyricsWithVersesParagraphsParser()
        {
            var input = new [] 
            {
                "【Hymn唱詩】Be Still,For the Presence of the Lord《靜默在至聖主跟前》",
                "Be still, for the presence of the Lord, the Holy one is here. ",
                "Come bow before him now, with reverence and fear.",
                "靜默在至聖的主跟前，主今親臨同在，",
                "這是神聖之地，主聖潔無瑕疵；",
                "In him no sin is found, we stand on holy ground.",
                "Be still, for the presence of the Lord, the Holy one is here. ",
                "你當尊他為聖，向他屈膝敬拜，",
                "肅靜在至聖的主跟前，主今親臨同在。"
            };

            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(3, elements.Count());

            Assert.True(elements[0].Pass);
            Assert.Equal(2, elements[0].Verses.Count());
            Assert.Equal("Hymn\nBe Still,For the Presence of the Lord", elements[0].Verses.First().Content);
            Assert.Equal("唱詩\n靜默在至聖主跟前", elements[0].Verses.Skip(1).First().Content);

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("Be still, for the presence of the Lord, the Holy one is here. \nCome bow before him now, with reverence and fear.", elements[1].Verses.First().Content);
            Assert.Equal("靜默在至聖的主跟前，主今親臨同在，\n這是神聖之地，主聖潔無瑕疵；", elements[1].Verses.Skip(1).First().Content);

            Assert.True(elements[2].Pass);
            Assert.Equal(2, elements[2].Verses.Count());
            Assert.Equal("In him no sin is found, we stand on holy ground.\nBe still, for the presence of the Lord, the Holy one is here. ", elements[2].Verses.First().Content);
            Assert.Equal("你當尊他為聖，向他屈膝敬拜，\n肅靜在至聖的主跟前，主今親臨同在。", elements[2].Verses.Skip(1).First().Content);

        }
        [Fact]
        public void LyricsWithLanguageParagraphsParser()
        {
            var input = new [] 
            {
                "【Hymn唱詩】Be Still,For the Presence of the Lord《靜默在至聖主跟前》",
                "Be still, for the presence of the Lord, the Holy one is here.",
                "Come bow before him now, with reverence and fear.",
                "In him no sin is found, we stand on holy ground.",
                "Be still, for the presence of the Lord, the Holy one is here.",
                "靜默在至聖的主跟前，主今親臨同在，",
                "這是神聖之地，主聖潔無瑕疵；",
                "你當尊他為聖，向他屈膝敬拜，",
                "肅靜在至聖的主跟前，主今親臨同在。"
            };

            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(5, elements.Count());

            Assert.True(elements[0].Pass);
            Assert.Equal(2, elements[0].Verses.Count());
            Assert.Equal("Hymn\nBe Still,For the Presence of the Lord", elements[0].Verses.First().Content);
            Assert.Equal("唱詩\n靜默在至聖主跟前", elements[0].Verses.Skip(1).First().Content);

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("Be still, for the presence of the Lord, the Holy one is here.", elements[1].Verses.First().Content);
            Assert.Equal("靜默在至聖的主跟前，主今親臨同在，", elements[1].Verses.Skip(1).First().Content);

            Assert.True(elements[2].Pass);
            Assert.Equal(2, elements[2].Verses.Count());
            Assert.Equal("Come bow before him now, with reverence and fear.", elements[2].Verses.First().Content);
            Assert.Equal("這是神聖之地，主聖潔無瑕疵；", elements[2].Verses.Skip(1).First().Content);

            Assert.True(elements[3].Pass);
            Assert.Equal(2, elements[3].Verses.Count());
            Assert.Equal("In him no sin is found, we stand on holy ground.", elements[3].Verses.First().Content);
            Assert.Equal("你當尊他為聖，向他屈膝敬拜，", elements[3].Verses.Skip(1).First().Content);

            Assert.True(elements[4].Pass);
            Assert.Equal(2, elements[4].Verses.Count());
            Assert.Equal("Be still, for the presence of the Lord, the Holy one is here.", elements[4].Verses.First().Content);
            Assert.Equal("肅靜在至聖的主跟前，主今親臨同在。", elements[4].Verses.Skip(1).First().Content);

        }
        [Fact]
        public void YouTubeLink_After_Title()
        {
            var input = new [] 
            {
                "【Mercy Seat Appeal恩座呼召】A Gift《一件禮物》",
                "https://www.youtube.com/watch?v=_Ayo8Yjuj88"
            };

            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(2, elements.Count());

            Assert.True(elements[0].Pass);
            Assert.Equal(2, elements[0].Verses.Count());
            Assert.Equal("Mercy Seat Appeal\nA Gift", elements[0].Verses.First().Content);
            Assert.Equal("恩座呼召\n一件禮物", elements[0].Verses.Skip(1).First().Content);

            Assert.True(elements[1].Pass);
            Assert.Single(elements[1].Verses);
            Assert.Equal("https://www.youtube.com/watch?v=_Ayo8Yjuj88", elements[1].Verses.First().Content);

        }
    }
}