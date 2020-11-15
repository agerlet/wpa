using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Word2Rtf.Models
{
    public class Bible
    {
        public Bible(IOptions<BooksSettings> books, IOptions<SectionsSettings> sections)
        {
            Books = books.Value.Books;
            Sections = sections.Value.Sections;
        }
        
        public Section[] Sections { get; }
        public Book[] Books { get; }
        
        public string[] FullEnglishBookNames =>
            Books.SelectMany(name => name.Versions.Where(FullEnglishFilter).Select(_ => _.Name)).ToArray();

        public string[] ShortEnglishBookNames =>
            Books.SelectMany(name => name.Versions.Where(ShortEnglishFilter).Select(_ => _.Name)).ToArray();

        public string[] TranditionChineseBookNames =>
            Books.SelectMany(name => name.Versions.Where(FullChineseFilter).Select(_ => _.Name)).ToArray();

        public string[] ShortChineseBookNames =>
            Books.SelectMany(name => name.Versions.Where(ShortChineseFilter).Select(_ => _.Name)).ToArray();

        public static Func<Version, bool> FullChineseFilter = _ =>
            _.Language == Language.Chinese &&
            _.Format == NameFormat.Full &&
            _.Chinese == Chinese.Traditional;
        
        public static Func<Version, bool> ShortChineseFilter = _ =>
            _.Language == Language.Chinese &&
            _.Format == NameFormat.Short;
        
        public static Func<Version, bool> FullEnglishFilter = _ =>
            _.Language == Language.English &&
            _.Format == NameFormat.Full;
        
        public static Func<Version, bool> ShortEnglishFilter = _ =>
            _.Language == Language.English &&
            _.Format == NameFormat.Short;

        public static Func<Version, bool> SimplifiedChineseFilter = _ =>
            _.Format == NameFormat.Full &&
            _.Chinese == Chinese.Simplified &&
            _.Language == Language.Chinese;
    }
}