using System;
using System.Collections.Generic;
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

            var mixer = _mixerFactory.GetMixer(main, overlay);
            mixer.Mix(main, overlay);

            Elements.Clear();
            Elements.AddRange(title);
            Elements.AddRange(main);
        }

        internal virtual void Adjust(List<Element> verses) { }

        internal virtual IEnumerable<Element> Adjust(IEnumerable<Element> verses) 
        {
            return verses;
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
        }    }
}