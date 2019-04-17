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

        [Fact]
        public void BibleVerses_BibleReading()
        {
            var input = new []
            {
                "【Bible Reading 讀經】Hebrews希伯來書 1:1-4",
                "In the past God spoke to our ancestors through the prophets at many times and in various ways, 2 but in these last days he has spoken to us by his Son, whom he appointed heir of all things, and through whom also he made the universe. 3 The Son is the radiance of God’s glory and the exact representation of his being, sustaining all things by his powerful word. After he had provided purification for sins, he sat down at the right hand of the Majesty in heaven. 4 So he became as much superior to the angels as the name he has inherited is superior to theirs.",
                "古時候，上帝藉着眾先知多次多方向列祖說話，末世，藉着 祂兒子向我們說話，又立祂為承受萬有的，也藉着祂創造宇 宙。祂是上帝榮耀的光輝，是上帝本體的真像，常用祂大能 的命令托住萬有。祂洗淨了人的罪，就坐在高天至大者的右 邊。祂所承受的名比天使的名更尊貴，所以祂遠比天使崇高。 "
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(2, elements.Count());
        }

        [Fact]
        public void Special_bible_verses_booking_info_in_second_line()
        {
            var input = new [] 
            {
                "【Offering for Kenya's Ambulance Project為肯尼亞購買救護車項目捐獻】",
                "2 Corinthians 9:6-9/哥林多後書9:6-9",
                "Remember this: Whoever sows sparingly will also reap sparingly, and whoever sows generously will also reap generously. Each of you should give what you have decided in your heart to give, not reluctantly or under compulsion, for God loves a cheerful giver. And God is able to bless you abundantly, so that in all things at all times, having all that you need, you will abound in every good work.As it is written: “They have freely scattered their gifts to the poor; their righteousness endures forever.”",
                "還有，少種的少收，多種的多收。 各人要照著心裡所決定的捐輸，不要為難，不必勉強，因為捐得樂意的人，是　神所喜愛的。 神能把各樣的恩惠多多地加給你們，使你們凡事常常充足，多作各樣的善事。 如經上所說：“他廣施錢財，賙濟窮人；他的仁義，存到永遠。”"
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(4, elements.Count());
        }

        [Fact]
        public void Lyrics_with_multiline_matching_issue()
        {
            var input = new [] 
            {
                "【Hymn唱詩】SASB82 Joy To The World《普世歡騰》",
                "Joy to the world!",
                "The Lord is come;",
                "Let earth receive her King,",
                "普世歡勝！救主下降，",
                "大地接祂為王；",
                "Let every heart prepare Him room",
                "And heaven and nature sing,",
                "And heaven and nature sing,",
                "And heaven,",
                "And heaven and nature sing.",
                "惟願眾心預備地方，",
                "諸天萬物歌唱，",
                "諸天萬物歌唱，",
                "諸天，諸天萬物歌唱。",
                "Joy to the world!",
                "The Saviour reigns;", 
                "Let men their songs employ,", 
                "普世歡勝！主治萬方，", 
                "萬民高聲歌唱；", 
                "While fields and floods,", 
                "Rocks, hills and plains", 
                "Repeat the sounding joy,", 
                "Repeat the sounding joy,", 
                "Repeat, repeat the sounding joy.", 
                "沃野、洪濤、山谷、平原，", 
                "響應歌聲嘹亮，", 
                "響應歌聲嘹亮，", 
                "響應，響應歌聲嘹亮。", 
                "He rules the world", 
                "With truth and grace,", 
                "And makes the nations prove", 
                "主藉真理恩治萬方，", 
                "要使萬邦證明；", 
                "The glories of His righteousness", 
                "And wonders of His love,", 
                "And wonders of His love,", 
                "And wonders, wonders of His love.", 
                "上主公義無限榮光，", 
                "主愛奇妙莫名，", 
                "主愛奇妙莫名，", 
                "主愛，主愛奇妙莫名。"
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(7, elements.Count());

            Assert.True(elements[3].Pass);
            Assert.Equal(2, elements[3].Verses.Count());
            Assert.Equal("Joy to the world!\nThe Saviour reigns;\nLet men their songs employ,", elements[3].Verses.First().Content);
            Assert.Equal("普世歡勝！主治萬方，\n萬民高聲歌唱；", elements[3].Verses.Skip(1).First().Content);
        }

        [Fact]
        public void Scripture_with_multiline_matching_issue()
        {
            var input = new [] 
            {
                "【Scripture證道經文】Isaiah 以賽亞書9:2-7",
                "The people walking in darkness have seen a great light; on those living in the land of deep darkness a light has dawned. You have enlarged the nation and increased their joy; they rejoice before you as people rejoice at the harvest,",
                "as warriors rejoice when dividing the plunder.",
                "在 黑 暗 中 行 走 的 百 姓 看 見 了 大 光 ， 住 在 死 蔭 之 地 的 人 有 光 照 耀 他 們 。3 你 使 這 國 民 繁 多 ， 加 增 他 們 的 喜 樂 ； 他 們 在 你 面 前 歡 喜 ， 好 像 收 割 的 歡 喜 ， 像 人 分 擄 物 那 樣 的 快 樂 。",
                "For as in the day of Midian’s defeat, you have shattered the yoke that burdens them, the bar across their shoulders, the rod of their oppressor.5 Every warrior’s boot used in battle and every garment rolled in blood will be destined for burning,",
                " will be fuel for the fire.",
                " 因 為 他 們 所 負 的 重 軛 和 肩 頭 上 的 杖 ， 並 欺 壓 他 們 人 的 棍 ， 你 都 已 經 折 斷 ， 好 像 在 米 甸 的 日 子 一 樣 。戰 士 在 亂 殺 之 間 所 穿 戴 的 盔 甲 ， 並 那 滾 在 血 中 的 衣 服 ， 都 必 作 為 可 燒 的 ， 當 作 火 柴 。",
                "For to us a child is born, to us a son is given, and the government will be on his shoulders. And he will be called Wonderful Counselor, Mighty God, Everlasting Father, Prince of Peace.",
                "因 有 一 嬰 孩 為 我 們 而 生 ； 有 一 子 賜 給 我 們 。 政 權 必 擔 在 他 的 肩 頭 上 ； 他 名 稱 為 奇 妙 策 士 、 全 能 的 神 、 永 在 的 父 、 和 平 的 君 。",
                "7 Of the greatness of his government and peace there will be no end. He will reign on David’s throne and over his kingdom, establishing and upholding it with justice and righteousness  from that time on and forever. The zeal of the Lord Almighty will accomplish this.",
                "7 他 的 政 權 與 平 安 必 加 增 無 窮 。 他 必 在 大 衛 的 寶 座 上 治 理 他 的 國 ， 以 公 平 公 義 使 國 堅 定 穩 固 ， 從 今 直 到 永 遠 。 萬 軍 之 耶 和 華 的 熱 心 必 成 就 這 事 。"
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(5, elements.Count());

            Assert.True(elements[3].Pass);
            Assert.Equal(2, elements[3].Verses.Count());
            Assert.Equal("For to us a child is born, to us a son is given, and the government will be on his shoulders. And he will be called Wonderful Counselor, Mighty God, Everlasting Father, Prince of Peace.", elements[3].Verses.First().Content);
            Assert.Equal("因 有 一 嬰 孩 為 我 們 而 生 ； 有 一 子 賜 給 我 們 。 政 權 必 擔 在 他 的 肩 頭 上 ； 他 名 稱 為 奇 妙 策 士 、 全 能 的 神 、 永 在 的 父 、 和 平 的 君 。", elements[3].Verses.Skip(1).First().Content);
        }

        [Fact]
        public void Verses_mixed_languages()
        {
            var input = new [] 
            {
                "【Call To Worshi宣告】Psalm詩篇118：21-29",
                "I will give you thanks, for you answered me; you have become my salvation. 我要稱謝你，因為你已經應允我，又成了我的拯救！The stone the builders rejected has become the cornerstone; 匠人所棄的石頭已成了房角的頭塊石頭。",
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(3, elements.Count());

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("I will give you thanks, for you answered me; you have become my salvation.", elements[1].Verses.First().Content);
            Assert.Equal("我要稱謝你，因為你已經應允我，又成了我的拯救！", elements[1].Verses.Skip(1).First().Content);
        }

        [Fact]
        public void Scripture_mixed_languages_with_verse_number()
        {
            var input = new [] 
            {
                "【Scripture證道經文】Matthew 馬太福音 26：6-13",
                "6 While Jesus was in Bethany in the home of Simon the Leper, 6耶穌在伯大尼長大痲瘋的西門家裡，"
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(2, elements.Count());

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("6 While Jesus was in Bethany in the home of Simon the Leper,", elements[1].Verses.First().Content);
        }

        [Fact]
        public void Verses_mixed_languages_in_one_line()
        {
            var input = new [] 
            {
                "【Call To Worshi宣告】Psalm詩篇118：21-29",
                "21 I will give you thanks, for you answered me; you have become my salvation. 21我要稱謝你，因為你已經應允我，又成了我的拯救！22 The stone the builders rejected has become the cornerstone; 22匠人所棄的石頭已成了房角的頭塊石頭。23 the Lord has done this, and it is marvelous in our eyes.23這是耶和華所作的，在我們眼中看為希奇。24 The Lord has done it this very day; let us rejoice today and be glad. 24這是耶和華所定的日子，我們在其中要高興歡喜！",
                "25 Lord, save us! Lord, grant us success!25耶和華啊，求你拯救！耶和華啊，求你使我們亨通！26 Blessed is he who comes in the name of the Lord. From the house of the Lord we bless you.26奉耶和華名來的是應當稱頌的！我們從耶和華的殿中為你們祝福！27 The Lord is God, and he has made his light shine on us. With boughs in hand, join in the festal procession up to the horns of the altar. 27耶和華是神；他光照了我們。理當用繩索把祭牲拴住，牽到壇角那裡。28 You are my God, and I will praise you; ou are my God, and I will exalt you.28你是我的神，我要稱謝你！你是我的神，我要尊崇你！29 Give thanks to the Lord, for he is good; his love endures forever.29你們要稱謝耶和華，因他本為善；他的慈愛永遠長存！"
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(10, elements.Count());

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("21 I will give you thanks, for you answered me; you have become my salvation.", elements[1].Verses.First().Content);
            Assert.Equal("21我要稱謝你，因為你已經應允我，又成了我的拯救！", elements[1].Verses.Skip(1).First().Content);
        }

        [Fact]
        public void Verses_mixed_languages_in_responsive_reading()
        {
            var input = new [] 
            {
                "【Responsive Reading啓應讀經】Isaiah以賽亞書53：4-6，Romans羅馬書5：6、8，2 Corinthians哥林多後書5：21，1 Peter彼得前書2：24-25",
                "(L) Surely he took up our pain and bore our suffering, yet we considered him punished by God,  stricken by him, and afflicted.（領）他誠然擔當我們的憂患，背負我們的痛苦；我們卻以為他受責罰，被神擊打苦待了。",
                "(C) But he was pierced for our transgressions, he was crushed for our iniquities; the punishment that brought us peace was on him, and by his wounds we are healed.（眾）哪知他為我們的過犯受害，為我們的罪孽壓傷。因他受的刑罰，我們得平安；因他受的鞭傷，我們得醫治。"
            };
            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(3, elements.Count());

            Assert.True(elements[1].Pass);
            Assert.Equal(2, elements[1].Verses.Count());
            Assert.Equal("(L) Surely he took up our pain and bore our suffering, yet we considered him punished by God,  stricken by him, and afflicted.", elements[1].Verses.First().Content);
            Assert.Equal("(領) 他誠然擔當我們的憂患，背負我們的痛苦；我們卻以為他受責罰，被神擊打苦待了。", elements[1].Verses.Skip(1).First().Content);

        }
    }
}