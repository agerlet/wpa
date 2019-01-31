using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    internal class LyricsParser : ParserBase<IGrouping<int, Element>>
    {
        public LyricsParser() : base() { }

        public override bool CanHandle(IGrouping<int, Element> group)
        {
            var input = group.First().Input;
            var isHymn = input.Contains("Hymn");
            var isSong = input.Contains("Song");
            return isHymn || isSong;
        }

        public override void Parse(IGrouping<int, Element> input)
        {
            var title = input.Where(element => 
                element.ElementType == ElementType.Title);
            var content = input.Where(element => 
                element.ElementType == ElementType.Content);

            var english = content.Where(element => 
                element.Verses.First().Language == Language.English)
                .ToList();
            var chinese = content.Where(element => 
                element.Verses.First().Language == Language.Chinese)
                .ToList();
            
            for (int i = 0; i < english.Count(); i++)
            {
                english[i].Input = $"{english[i].Input}\n{chinese[i].Input}";
                var verses = english[i].Verses.ToList();
                verses.AddRange(chinese[i].Verses);
                english[i].Verses = verses; 
            }

            Elements.Clear();
            Elements.AddRange(title);
            Elements.AddRange(english);
        }
    }
}