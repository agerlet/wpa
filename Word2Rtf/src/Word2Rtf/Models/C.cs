using System;
using System.Linq;

namespace Word2Rtf.Models
{
    internal class C
    {
        public static Section[] Sections { get; set; }
        public static Book[] Books { get; set; }
        
        public static string[] FullEnglishBookNames =>
            Books.SelectMany(name => name.Versions.Where(GetFullEnglish).Select(_ => _.Name)).ToArray();

        public static string[] ShortEnglishBookNames =>
            Books.SelectMany(name => name.Versions.Where(GetShortEnglish).Select(_ => _.Name)).ToArray();

        public static string[] TranditionChineseBookNames =>
            Books.SelectMany(name => name.Versions.Where(GetFullChinese).Select(_ => _.Name)).ToArray();

        public static string[] ShortChineseBookNames =>
            Books.SelectMany(name => name.Versions.Where(GetShortChinese).Select(_ => _.Name)).ToArray();

        public static Func<Version, bool> GetFullChinese = _ =>
            _.Language == Language.Chinese &&
            _.Format == NameFormat.Full &&
            _.Chinese == Chinese.Traditional;
        
        public static Func<Version, bool> GetShortChinese = _ =>
            _.Language == Language.Chinese &&
            _.Format == NameFormat.Short;
        
        public static Func<Version, bool> GetFullEnglish = _ =>
            _.Language == Language.English &&
            _.Format == NameFormat.Full;
        
        public static Func<Version, bool> GetShortEnglish = _ =>
            _.Language == Language.English &&
            _.Format == NameFormat.Short;

        public static Func<Version, bool> GetSimplifiedChinese = _ =>
            _.Format == NameFormat.Full &&
            _.Chinese == Chinese.Simplified &&
            _.Language == Language.Chinese;
    }

    internal class Book
    {
        public Version[] Versions { get; set; }
    }
    
    internal class Version
    {
        public string Name { get; set; }
        public Language Language { get; set; }
        public NameFormat Format { get; set; }
        public Chinese? Chinese { get; set; }
    }

}