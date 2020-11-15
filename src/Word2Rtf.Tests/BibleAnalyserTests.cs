using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Word2Rtf.Analysers;
using Word2Rtf.Exceptions;
using Word2Rtf.Models;
using Xunit;

namespace Word2Rtf.Tests
{
    public class BibleAnalyserTests
    {
        const string Source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26；Luke路2:10b-11,14";

        private BibleAnalyser _bibleAnalyser;
        
        public BibleAnalyserTests()
        {
            var booksJson = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Dictionary", "book-names.json"));
            var booksSettings = JsonConvert.DeserializeObject<BooksSettings>(booksJson);
            var books = Options.Create(booksSettings);
            
            var sectionsJson = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Dictionary", "sections.json"));
            var sectionsSettings = JsonConvert.DeserializeObject<SectionsSettings>(sectionsJson);
            var sections = Options.Create(sectionsSettings);
            
            _bibleAnalyser = new BibleAnalyser(new Bible(books, sections));
        }

        [Fact]
        public void Test_SplitByLanguage_BibleVerse()
        {
            var actual = _bibleAnalyser.FilterByLanguages(Source);

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
            var results = _bibleAnalyser.FilterByLanguages("【Responsive Reading啟應讀經】Psalm詩篇 8");
            Assert.Equal($"啟應讀經{Environment.NewLine}詩篇 8", results[1].Content);
        }

        [Fact]
        public void Should_not_repeat_chinese_title_When_given_unknown_wrong_charactors()
        {
            var results = _bibleAnalyser.FilterByLanguages("【Proclamation宣告】Revelation啓示錄 4:11");
            Assert.Equal($"宣告{Environment.NewLine}啓示錄 4:11", results[1].Content);
        }

        [Fact]
        public void Should_use_knowned_Chinese_When_Simplify_Chinese_passed_in()
        {
            var results = _bibleAnalyser.FilterByLanguages("【Scripture Reading證道經文】Romans罗马书1:18-32");
            Assert.Equal($"證道經文{Environment.NewLine}羅馬書 1:18-32", results[1].Content);            
        }

        [Fact]
        public void Should_find_verse_numbers_when_unknown_book_name_is_given()
        {
            var results = _bibleAnalyser.GetVerseNumbers("【Scripture Reading證道經文】Romans罗马书1:18-32").ToArray();
            Assert.Equal($"1:18-32", results[0]);
        }

        [Fact]
        public void Should_return_chapter_number()
        {
            Assert.Equal("8", _bibleAnalyser.GetVerseNumbers("【Responsive Reading啟應讀經】Psalm詩篇 8").First());
        }

        [Fact]
        public void Should_split_song_title_by_language()
        {
            var input = "【序樂/Prelude】SB#636我們頌揚多年祝福/Sing We Many Years of Blessing";
            var result = _bibleAnalyser.FilterByLanguages(input);
            
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
            var actual = _bibleAnalyser.FilterByLanguages(source);

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
            Assert.True(_bibleAnalyser.IsBibleReadingTitle(Source));
        }     
        
        [Fact]
        public void Test_Is_NOT_BibleReadingTitle()
        {
            Assert.False(_bibleAnalyser.IsBibleReadingTitle("【唱詩/Song】因著十架愛/Love From The Cross"));
        }

        [Fact]
        public void Should_recognise_When_section_is_suppose_to_be_a_bible_verse()
        {
            var result = _bibleAnalyser.IsBibleReadingTitle("【Call To Worship宣告】Something 2:20b");
            Assert.True(result);
        }

        [Fact]
        public void Test_GetEnglishAndVerseNumber()
        {
            var source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26 ;Luke路2:10b-11,14";
            var expected = new [] { "Call to worship", "Song of Songs50:10,23-26", "Luke 2:10b-11,14" };
            var actual = _bibleAnalyser.GetEnglishAndVerseNumber(source).ToArray();
            for(var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }

        [Fact]
        public void Should_not_break_Song_of_Songs()
        {
            var source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26 ;Luke路2:10b-11,14";
            var actual = _bibleAnalyser.GetEnglishAndVerseNumber(source).ToArray();
            Assert.Contains("Song of Songs", actual[1]);
        }

        [Fact]
        public void Should_not_be_bible_title_When_song_name_provided()
        {
            Assert.False(_bibleAnalyser.IsBibleReadingTitle("【序樂/Prelude】SB#636我們頌揚多年祝福/Sing We Many Years of Blessing"));
        }

        [Fact]
        public void Test_GetVerseNumbers()
        {
            string source = "【宣告/Call to worship】 詩篇 Song of Songs50:10,23——26;Luke路2:10b-11,14";
            var expected = new [] { "50:10,23-26", "2:10b-11,14" };
            var actual = _bibleAnalyser.GetVerseNumbers(source).ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Should_return_2_languages_For_complex_verses()
        {
            var input = "【Responsive Reading啓應讀經】Isaiah以賽亞書53：4-6，Romans羅馬書5：6、8，2 Corinthians哥林多後書5：21，1 Peter彼得前書2：24-25";
            var result = _bibleAnalyser.FilterByLanguages(input);
            
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
            var result = _bibleAnalyser.FilterByLanguages("【啟應經文】約翰福音 3:1-8,16, 哥林多後書 5:17 John 3:1-8,16, 2Cor 5:17");
            Assert.Contains("Responsive Reading", result[0].Content);
        }

        [Fact]
        public void Should_return_english_book_names_with_verse_numbers()
        {
            var source = "【啟應經文】約翰福音 3:1-8,16, 哥林多後書 5:17 John 3:1-8,16, 2Cor 5:17";
            var result = _bibleAnalyser.GetEnglishAndVerseNumber(source).ToArray();
            Assert.Equal(2, result.Count());
            Assert.Equal("John 3:1-8,16", result[0]);
            Assert.Equal("2Cor 5:17", result[1]);
        }
        
        [Fact]
        public void Should_return_1_john_names_with_verse_numbers()
        {
            var source = "【Scripture 經文】1 John約翰一書1：9";
            var result = _bibleAnalyser.GetEnglishAndVerseNumber(source).ToArray();
            Assert.Equal(2, result.Length);
            Assert.Equal("Scripture", result[0]);
            Assert.Equal("1 John 1:9", result[1]);
        }

        [Fact]
        public void Should_deduplicate_verse_numbers()
        {
            var result = _bibleAnalyser.GetVerseNumbers("【啟應經文】約翰福音 3:1-8,16, 哥林多後書 5:17 John 3:1-8,16, 2Cor 5:17").ToArray();
            Assert.Equal(2, result.Length);
            Assert.Equal("3:1-8,16", result[0]);
            Assert.Equal("5:17", result[1]);
        }

        [Fact]
        public void Should_avoid_break_1_John()
        {
            var result = _bibleAnalyser.GetVerseNumbers("【Scripture 經文】1 John約翰一書1：9").ToArray();
            Assert.Single(result);
            Assert.Equal("1:9", result[0]);
        }
        
        [Fact]
        public void Should_throw_ImbalancedLanguagesException_When_part_language_is_missing()
        {
            Assert.ThrowsAny<ImbalancedLanguagesException>(() => 
                _bibleAnalyser.FilterByLanguages("【宣告/Proclaim】Something 50:23"));
        }

        [Fact]
        public void Should_do_nothing_When_balanced_terms_passed_in()
        {
            var cTerms = new List<string>() { "宣告", "詩篇" };
            var eTerms = new List<string>() { "Proclaim", "Psalm" };
            var cTermsLength = cTerms.Count();
            var eTermsLength = eTerms.Count();
            _bibleAnalyser.BalanceLanguages(cTerms, eTerms);
            
            Assert.Equal(cTermsLength, cTerms.Count);
            Assert.Equal(eTermsLength, eTerms.Count);
        }

        [Fact]
        public void Should_insert_the_missing_Chinese_part()
        {
            var cTerms = new List<string>() { "宣告" };
            var eTerms = new List<string>() { "Proclaim", "Psalm" };
            _bibleAnalyser.BalanceLanguages(cTerms, eTerms);
            
            Assert.Equal(cTerms.Count(), eTerms.Count());
        }

        [Fact]
        public void Should_insert_the_missing_English_part()
        {
            var cTerms = new List<string>() { "宣告", "詩篇" };
            var eTerms = new List<string>() { "Proclaim" };
            _bibleAnalyser.BalanceLanguages(cTerms, eTerms);
            
            Assert.Equal(cTerms.Count(), eTerms.Count());
        }

        [Fact]
        public void Should_not_throw_exception_When_the_missing_part_cannot_find_counterpart()
        {
            var cTerms = new List<string>() { "宣告" };
            var eTerms = new List<string>() { "Proclaim", "Cannot find" };
            _bibleAnalyser.BalanceLanguages(cTerms, eTerms);
            
            Assert.Single(cTerms);
            Assert.Equal(2, eTerms.Count());
        }

        [Fact]
        public void Should_keep_single_quote_in_English()
        {
            var result = _bibleAnalyser.FilterByLanguages("【Song詩歌】This is My Father's World 这是天父世界");
            Assert.Contains("'", result[0].Content);
        }

        [Fact]
        public void Should_NOT_put_single_quote_in_Chinese()
        {
            var result = _bibleAnalyser.FilterByLanguages("【Song詩歌】This is My Father's World 这是天父世界");
            Assert.DoesNotContain("'", result[1].Content);
        }

        [Fact]
        public void Should_return_bible_verse_number_with_incorrect_spelled_Psalm()
        {
            var source = "【Bible Reading啟應讀經】Pslam 詩篇 118:21-29";
            var actual = _bibleAnalyser.FilterByLanguages(source);
            Assert.Equal($"Bible Reading{Environment.NewLine}Pslam 118:21-29", actual[0].Content);
            Assert.Equal($"啟應讀經{Environment.NewLine}詩篇 118:21-29", actual[1].Content);
        }
    }
}