using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Word2Rtf.Models
{
    public enum Language
    {
        Chinese,
        English,
        Mixed
    }

    [Flags]
    public enum ElementType
    {
        Title = 0,
        Content = 1,
        Lyrics = 1 << 1,
        BibleVerses = 1 << 2,
        YouTubeLink = 1 << 3,
    }

    public static class BookNames
    {
        static BookNames()
        {
            English = new List<string>
            {
                "Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy", "Joshua", "Judges", "Ruth", "1 Samuel",
                "2 Samuel", "1 Kings", "2 Kings", "1 Chronicles", "2 Chronicles", "Ezra", "Nehemiah", "Esther", "Job",
                "Psalm", "Proverbs", "Ecclesiastes", "Song of Songs", "Isaiah", "Jeremiah", "Lamentations", "Ezekiel",
                "Daniel", "Hosea", "Joel", "Amos", "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk", "Zephaniah",
                "Haggai", "Zechariah", "Malachi", "Matthew", "Mark", "Luke", "John", "Acts", "Romans", "1 Corinthians",
                "2 Corinthians", "Galatians", "Ephesians", "Philippians", "Colossians", "1 Thessalonians",
                "2 Thessalonians", "1 Timothy", "2 Timothy", "Titus", "Philemon", "Hebrews", "James", "1 Peter",
                "2 Peter", "1 John", "2 John", "3 John", "Jude", "Revelation",
                /*---------------------------------------------------------------------------------------------------*/
                "Hebrew",
            };
            EnglishShortName = new List<string>
            {
                "Gen", "Exod", "Lev", "Num", "Deut", "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr",
                "2Chr", "Ezra", "Neh", "Esth", "Job", "Ps", "Prov", "Eccl", "Song", "Isa", "Jer", "Lam", "Ezek", "Dan",
                "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal", "Matt",
                "Mark", "Luke", "John", "Acts", "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess",
                "1Tim", "2Tim", "Titus", "Phlm", "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev"
            };
            Chinese = new List<string>
            {
                "創世記", "出埃及記", "利未記", "民數記", "申命記", "約書亞記", "士師記", "路得記", "撒母耳記上", "撒母耳記下", "列王紀上", "列王紀下", "歷代志上",
                "歷代志下", "以斯拉記", "尼希米記", "以斯帖記", "約伯記", "詩篇", "箴言", "傳道書", "雅歌", "以賽亞書", "耶利米書", "耶利米哀歌", "以西結書", "但以理書",
                "何西阿書", "約珥書", "阿摩司書", "俄巴底亞書", "約拿書", "彌迦書", "那鴻書", "哈巴谷書", "西番雅書", "哈該書", "撒迦利亞書", "瑪拉基書", "馬太福音",
                "馬可福音", "路加福音", "約翰福音", "使徒行傳", "羅馬書", "哥林多前書", "哥林多後書", "加拉太書", "以弗所書", "腓立比書", "歌羅西書", "帖撒羅尼迦前書",
                "帖撒羅尼迦後書", "提摩太前書", "提摩太後書", "提多書", "腓利門書", "希伯來書", "雅各書", "彼得前書", "彼得後書", "約翰一書", "約翰二書", "約翰三書",
                "猶太書", "啟示錄",
                /*---------------------------------------------------------------------------------------------------*/
                "歌林多前書", "歌林多後書", "啓示錄",
            };
            ChineseShortName = new List<string>
            {
                "創", "出", "利", "民", "申", "書", "士", "得", "撒上", "撒下", "王上", "王下", "代上", "代下", "拉", "尼", "斯", "伯", "詩",
                "箴", "傳", "歌", "賽", "耶", "哀", "結", "但", "何", "珥", "摩", "俄", "拿", "彌", "鴻", "哈", "番", "該", "亞", "瑪", "太",
                "可", "路", "約", "徒", "羅", "林前", "林後", "加", "弗", "腓", "西", "帖前", "帖後", "提前", "提後", "多", "門", "來", "雅",
                "彼前", "彼後", "約一", "約二", "約三", "猶", "啟",
            };
            ChineseSimplified = new List<string>
            {
                "创世记", "出埃及记", "利未记", "民数记", "申命记", "约书亚记", "士师记", "路得记", "撒母耳记上", "撒母耳记下", "列王纪上", "列王纪下", "历代志上",
                "历代志下", "以斯拉记", "尼希米记", "以斯帖记", "约伯记", "诗篇", "箴言", "传道书", "雅歌" , "以赛亚书", "耶利米书", "耶利米哀歌", "以西结书", "但以理书",
                "何西阿书", "约珥书", "阿摩司书", "俄巴底亚书", "约拿书", "弥迦书", "那鸿书", "哈巴谷书" , "西番雅书", "哈该书", "撒迦利亚书", "玛拉基书", "马太福音",
                "马可福音", "路加福音", "约翰福音", "使徒行传", "罗马书", "哥林多前书", "哥林多后书", "加拉太书" , "以弗所书", "腓立比书", "歌罗西书", "帖撒罗尼迦前书",
                "帖撒罗尼迦后书", "提摩太前书", "提摩太后书", "提多书", "腓利门书", "希伯来书", "雅各书" , "彼得前书", "彼得后书", "约翰一书", "约翰二书", "约翰三书",
                "犹太书", "启示录",
                /*---------------------------------------------------------------------------------------------------*/
                "歌林多前书", "歌林多后书"
            };
        }

        public static List<string> English { get; internal set; }

        public static List<string> EnglishShortName { get; internal set; }

        public static List<string> Chinese { get; internal set; }

        public static List<string> ChineseShortName { get; internal set; }
        
        public static List<string> ChineseSimplified { get; internal set; }
    }

    internal class Sections
    {
        public static List<Section> Names { get; set; }
    }
}