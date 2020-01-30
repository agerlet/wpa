using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Parsers
{
    abstract class GroupingParserBase : ParserBase<IGrouping<int, Element>>
    {
        public GroupingParserBase(Mixers.MixerFactory mixerFactory) 
            : base(mixerFactory) { }

        public override void Parse(IGrouping<int, Element> group)
        {
            var title = group.Get(ElementType.Title);
            var content = group.Get(ElementType.Content);

            content = Adjust(content);

            var main = content.Get(Language.English).ToList();
            var overlay = content.Get(Language.Chinese).ToList();

            Adjust(main);
            Adjust(overlay);

            var mixer = MixerFactory.GetMixer(main, overlay);
            mixer.Mix(main, overlay);

            Elements.Clear();
            Elements.AddRange(title);
            Elements.AddRange(main);
        }

        protected virtual void Adjust(List<Element> verses) { }

        protected virtual IEnumerable<Element> Adjust(IEnumerable<Element> verses)
        {
            if (verses == null || !verses.Any()) return verses;
            
            var languagePattern = GetLanguagePattern(verses.GroupBy(_ => _.TitleId).First());
            var languageChanged = LanguageChanged(languagePattern);
            var numOfVerse = verses.Count();

            if (numOfVerse % (languageChanged + 1) == 0) 
                return verses;
            
            var elements = new List<Element>();
            var rest = verses.ToList();
            rest.Reverse();
            var entry = rest.First();
            foreach (var element in rest.Skip(1))
            {
                if (!Equals(
                    entry.Verses.Select(_ => _.Language).Distinct().Single(),
                    element.Verses.Select(_ => _.Language).Distinct().Single()))
                {
                    elements.Insert(0, entry);
                    entry = element;
                    continue;
                }
                entry.Input = string.Concat(element.Input, Environment.NewLine, entry.Input);
                var verse = entry.Verses.First();
                verse.Content = string.Concat(element.Verses.First().Content, Environment.NewLine, verse.Content);
                entry.Verses = new []{verse};
            }
            elements.Insert(0, entry);
            
            return elements;
        }
        
        protected List<Language> GetLanguagePattern(IGrouping<int, Element> group)
        {
            return group
                .Get(ElementType.Content)
                .Select(element => element.Verses.First().Language).ToList();
        }

        protected int LanguageChanged(IEnumerable<Language> pattern)
        {
            var changed = 0;
            Language? lastLanguage = default(Language?);
            foreach(var l in pattern)
            {    
                if (!lastLanguage.HasValue)
                    lastLanguage = l;
                
                if (lastLanguage.HasValue && lastLanguage.Value != l)
                {
                    lastLanguage = l;
                    changed++;
                }
            }
            return changed;
        }    
    }
}