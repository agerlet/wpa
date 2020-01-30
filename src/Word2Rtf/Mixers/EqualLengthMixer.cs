using System.Collections.Generic;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Mixers
{
    class EqualLengthMixer : IMixer
    {
        public virtual bool CanHandle(List<Element> main, List<Element> addon)
        {
            return main.Count == addon.Count;
        }

        public virtual void Mix(List<Element> main, List<Element> addon)
        {
            for (int i = 0; i < main.Count(); i++)
            {
                main[i].Input = $"{main[i].Input}\n{addon[i].Input}";
                var verses = main[i].Verses.ToList();
                verses.AddRange(addon[i].Verses);
                main[i].Verses = verses; 
            }
        }
    }
}
