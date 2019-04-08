using System.Collections.Generic;
using Word2Rtf.Models;

namespace Word2Rtf.Mixers
{
    class MergeThemMixer : IMixer
    {
        public bool CanHandle(List<Element> main, List<Element> addon)
        {
            return true;
        }

        public void Mix(List<Element> main, List<Element> addon)
        {
            main.AddRange(addon);
        }
    }
}