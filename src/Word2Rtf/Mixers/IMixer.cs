using System.Collections.Generic;
using Word2Rtf.Models;

namespace Word2Rtf.Mixers
{
    public interface IMixer
    {
        bool CanHandle(List<Element> main, List<Element> addon);
        void Mix(List<Element> main, List<Element> addon);
    }
}