using System;
using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Mixers
{
    class SingleVerseNumberMixer : EqualLengthMixer
    {
        public override bool CanHandle(List<Element> main, List<Element> addon)
        {
            if (!main.Any() || !addon.Any()) return false;
            if (main.Count == addon.Count) return false;
            return main.Count > 1 && addon.Count == 1 || main.Count == 1 && addon.Count > 1;
        }

        public override void Mix(List<Element> main, List<Element> addon)
        {
            if (main.Count > 1 && addon.Count == 1)
            {
                Collapse(main);
            }
            else if (addon.Count > 1 && main.Count == 1)
            {
                Collapse(addon);
            }
            
            base.Mix(main, addon);
        }

        private void Collapse(List<Element> main)
        {
            var first = main.First();
            foreach(var element in main.Skip(1))
            {
                first.Input += " " + element.Input;
                first.Verses.First().Content += " " + string.Join(" ", 
                    element.Verses.Select(verse => verse.Content));
            }
            main.Clear();
            main.Add(first);
        }
    }
}