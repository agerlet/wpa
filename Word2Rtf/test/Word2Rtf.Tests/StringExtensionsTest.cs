using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Word2Rtf;
using Word2Rtf.Exceptions;
using Word2Rtf.Models;

namespace Word2Rtf.Tests
{
    public class StringExtensionTest
    {
        const string Source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26；Luke路2:10b-11,14";

        public StringExtensionTest()
        {
            new Function();
        }

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
            var actual = Source.Purify();
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
            var actual = Source.FilterByLanguages();

            var expectedLength = 2;
            var expectedVerse1Language = Language.English;
            var expectedVerse2Language = Language.Chinese;
            var expectedVerse1Content = $"Call to worship{Environment.NewLine}Song of Songs50:10,23-26{Environment.NewLine}Luke 2:10b-11,14";
            var expectedVerse2Content = $"宣告{Environment.NewLine}詩篇 50:10,23-26{Environment.NewLine}路 2:10b-11,14";

            Assert.Equal(expectedLength, actual.Length);
            Assert.Equal(expectedVerse1Language, actual[0].Language);
            Assert.Equal(expectedVerse2Language, actual[1].Language);
            Assert.Equal(expectedVerse1Content, actual[0].Content);
            Assert.Equal(expectedVerse2Content, actual[1].Content);
        }

        [Fact]
        public void Should_not_repeat_chinese_title()
        {
            var results = "【Responsive Reading啟應讀經】Psalm詩篇 8".FilterByLanguages();
            Assert.Equal($"啟應讀經{Environment.NewLine}詩篇 8", results[1].Content);
        }

        [Fact]
        public void Should_not_repeat_chinese_title_When_given_unknown_wrong_charactors()
        {
            var results = "【Proclamation宣告】Revelation啓示錄 4:11".FilterByLanguages();
            Assert.Equal($"宣告{Environment.NewLine}啓示錄 4:11", results[1].Content);
        }

        [Fact]
        public void Should_use_knowned_Chinese_When_Simplify_Chinese_passed_in()
        {
            var results = "【Scripture Reading證道經文】Romans罗马书1:18-32".FilterByLanguages();
            Assert.Equal($"證道經文{Environment.NewLine}羅馬書 1:18-32", results[1].Content);            
        }

        [Fact]
        public void Should_find_verse_numbers_when_unknown_book_name_is_given()
        {
            var results = "【Scripture Reading證道經文】Romans罗马书1:18-32".GetVerseNumbers().ToArray();
            Assert.Equal($"1:18-32", results[0]);
        }

        [Fact]
        public void Should_recognize_as_chinese()
        {
            Assert.True('罗'.IsChinese());
            Assert.True('羅'.IsChinese());
        }

        [Fact]
        public void Should_NOT_recognize_English_as_Chinese()
        {
            Assert.False("Romans".All(_ => _.IsChinese()));
        }

        [Fact]
        public void Should_NOT_recognize_bible_verses_as_Chinese()
        {
            Assert.False("1:18-32".All(_ => _.IsChinese()));
        }
        
        [Fact]
        public void Should_NOT_recognize_punctuation_as_Chinese()
        {
            Assert.False("、".All(_ => _.IsChinese()));
        }

        [Fact]
        public void Should_return_chapter_number()
        {
            Assert.Equal("8", "【Responsive Reading啟應讀經】Psalm詩篇 8".GetVerseNumbers().First());
        }

        [Fact]
        public void Should_split_song_title_by_language()
        {
            var input = "【序樂/Prelude】SB#636我們頌揚多年祝福/Sing We Many Years of Blessing";
            var result = input.FilterByLanguages();
            
            Assert.Equal(2, result.Count());
            
            Assert.Equal($"Prelude{Environment.NewLine}SB#636 Sing We Many Years of Blessing", result[0].Content);
            Assert.Equal(Language.English, result[0].Language);
            
            Assert.Equal($"序樂{Environment.NewLine}我們頌揚多年祝福", result[1].Content);
            Assert.Equal(Language.Chinese, result[1].Language);
        }

        [Fact]
        public void Test_SplitByLanguage_Song()
        {
            var source = "【唱詩/Song】因著十架愛/Love From The Cross";
            var actual = source.FilterByLanguages();

            var expectedLength = 2;
            var expectedVerse1Language = Language.English;
            var expectedVerse2Language = Language.Chinese;
            var expectedVerse1Content = $"Song{Environment.NewLine}Love From The Cross";
            var expectedVerse2Content = $"唱詩{Environment.NewLine}因著十架愛";

            Assert.Equal(expectedLength, actual.Length);
            Assert.Equal(expectedVerse1Language, actual[0].Language);
            Assert.Equal(expectedVerse2Language, actual[1].Language);
            Assert.Equal(expectedVerse1Content, actual[0].Content);
            Assert.Equal(expectedVerse2Content, actual[1].Content);
        }
        
        [Fact]
        public void Test_IsBibleReadingTitle()
        {
            Assert.True(Source.IsBibleReadingTitle());
            Assert.False("【唱詩/Song】因著十架愛/Love From The Cross".IsBibleReadingTitle());
        }

        [Fact]
        public void Should_recognise_When_section_is_suppose_to_be_a_bible_verse()
        {
            var result = "【Call To Worship宣告】Something 2:20b".IsBibleReadingTitle();
            Assert.True(result);
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
        public void Should_not_break_Song_of_Songs()
        {
            var source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26 ;Luke路2:10b-11,14";
            var actual = source.GetEnglishAndVerseNumber().ToArray();
            Assert.Contains("Song of Songs", actual[1]);
        }

        [Fact]
        public void Should_not_be_bible_title_When_song_name_provided()
        {
            Assert.False("【序樂/Prelude】SB#636我們頌揚多年祝福/Sing We Many Years of Blessing".IsBibleReadingTitle());
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
            var actual = source.GetVerseNumbers().ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_break_by_verse_numbers_For_same_language()
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
        public void Should_split_languages_when_inline_mixed_languages()
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

        [Fact]
        public void Should_return_2_languages_For_complex_verses()
        {
            var result = "【Responsive Reading啓應讀經】Isaiah以賽亞書53：4-6，Romans羅馬書5：6、8，2 Corinthians哥林多後書5：21，1 Peter彼得前書2：24-25"
                .FilterByLanguages();
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            
            Assert.Equal($"Responsive Reading{Environment.NewLine}Isaiah 53:4-6{Environment.NewLine}Romans 5:6、8{Environment.NewLine}2 Corinthians 5:21{Environment.NewLine}1 Peter 2:24-25", result[0].Content);
            Assert.Equal(Language.English, result[0].Language);
            
            Assert.Equal($"啓應讀經{Environment.NewLine}以賽亞書 53:4-6{Environment.NewLine}羅馬書 5:6、8{Environment.NewLine}哥林多後書 5:21{Environment.NewLine}彼得前書 2:24-25", result[1].Content);
            Assert.Equal(Language.Chinese, result[1].Language);
        }

        [Fact]
        public void Should_find_missing_english_counterpart()
        {
            var result = "【啟應經文】約翰福音 3:1-8,16, 哥林多後書 5:17 John 3:1-8,16, 2Cor 5:17".FilterByLanguages();
            Assert.Contains("Responsive Reading", result[0].Content);
        }

        [Fact]
        public void Should_return_english_book_names_with_verse_numbers()
        {
            var source = "【啟應經文】約翰福音 3:1-8,16, 哥林多後書 5:17 John 3:1-8,16, 2Cor 5:17";
            var result = source.GetEnglishAndVerseNumber().ToArray();
            Assert.Equal(2, result.Count());
            Assert.Equal("John 3:1-8,16", result[0]);
            Assert.Equal("2Cor 5:17", result[1]);
        }
        
        [Fact]
        public void Should_return_1_john_names_with_verse_numbers()
        {
            var source = "【Scripture 經文】1 John約翰一書1：9";
            var result = source.GetEnglishAndVerseNumber().ToArray();
            Assert.Equal(2, result.Length);
            Assert.Equal("Scripture", result[0]);
            Assert.Equal("1 John 1:9", result[1]);
        }

        [Fact]
        public void Should_deduplicate_verse_numbers()
        {
            var result = "【啟應經文】約翰福音 3:1-8,16, 哥林多後書 5:17 John 3:1-8,16, 2Cor 5:17".GetVerseNumbers().ToArray();
            Assert.Equal(2, result.Length);
            Assert.Equal("3:1-8,16", result[0]);
            Assert.Equal("5:17", result[1]);
        }

        [Fact]
        public void Should_avoid_break_1_John()
        {
            var result = "【Scripture 經文】1 John約翰一書1：9".GetVerseNumbers().ToArray();
            Assert.Single(result);
            Assert.Equal("1:9", result[0]);
        }
        
        [Fact]
        public void Should_throw_ImbalancedLanguagesException_When_part_language_is_missing()
        {
            Assert.ThrowsAny<ImbalancedLanguagesException>(() => "【宣告/Proclaim】Something 50:23".FilterByLanguages());
        }

        [Fact]
        public void Should_do_nothing_When_balanced_terms_passed_in()
        {
            var cTerms = new List<string>() { "宣告", "詩篇" };
            var eTerms = new List<string>() { "Proclaim", "Psalm" };
            var cTermsLength = cTerms.Count();
            var eTermsLength = eTerms.Count();
            StringExtensions.BalanceLanguages(cTerms, eTerms);
            
            Assert.Equal(cTermsLength, cTerms.Count);
            Assert.Equal(eTermsLength, eTerms.Count);
        }

        [Fact]
        public void Should_insert_the_missing_Chinese_part()
        {
            var cTerms = new List<string>() { "宣告" };
            var eTerms = new List<string>() { "Proclaim", "Psalm" };
            StringExtensions.BalanceLanguages(cTerms, eTerms);
            
            Assert.Equal(cTerms.Count(), eTerms.Count());
        }

        [Fact]
        public void Should_insert_the_missing_English_part()
        {
            var cTerms = new List<string>() { "宣告", "詩篇" };
            var eTerms = new List<string>() { "Proclaim" };
            StringExtensions.BalanceLanguages(cTerms, eTerms);
            
            Assert.Equal(cTerms.Count(), eTerms.Count());
        }

        [Fact]
        public void Should_not_throw_exception_When_the_missing_part_cannot_find_counterpart()
        {
            var cTerms = new List<string>() { "宣告" };
            var eTerms = new List<string>() { "Proclaim", "Cannot find" };
            StringExtensions.BalanceLanguages(cTerms, eTerms);
            
            Assert.Single(cTerms);
            Assert.Equal(2, eTerms.Count());
        }

        [Fact]
        public void Should_Filter_Out_Hash_When_Get_Chinese()
        {
            var input = "【序樂/Prelude】SB#636我們頌揚多年祝福/Sing We Many Years of Blessing";
            var result = input.GetChinese();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Should_keep_single_quote_in_English()
        {
            var result = "【Song詩歌】This is My Father's World 这是天父世界".FilterByLanguages();
            Assert.Contains("'", result[0].Content);
        }

        [Fact]
        public void Should_NOT_put_single_quote_in_Chinese()
        {
            var result = "【Song詩歌】This is My Father's World 这是天父世界".FilterByLanguages();
            Assert.DoesNotContain("'", result[1].Content);
        }

        [Fact]
        public void Should_return_section_json()
        {
            var json = JsonConvert.SerializeObject(C.Sections, Formatting.Indented, new StringEnumConverter());
            Assert.NotNull(json);
            Assert.NotEmpty(json);
        }

        [Fact]
        public void Should_return_bible_verse_number_with_incorrect_spelled_Psalm()
        {
            var source = "【Bible Reading啟應讀經】Pslam 詩篇 118:21-29";
            var actual = source.FilterByLanguages();
            Assert.Equal($"Bible Reading{Environment.NewLine}Pslam 118:21-29", actual[0].Content);
            Assert.Equal($"啟應讀經{Environment.NewLine}詩篇 118:21-29", actual[1].Content);
        }
    }
}
