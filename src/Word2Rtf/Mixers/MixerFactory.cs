using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Mixers
{
    internal class MixerFactory
    {
        readonly IMixer _theLastMixer;
        readonly List<IMixer> _mixers;
        public MixerFactory()
        {
            _mixers = new List<IMixer>
            {
                new EqualLengthMixer(),
                new SingleVerseNumberMixer(),
            };

            _theLastMixer = new LastMixer();
        }

        public IMixer GetMixer(List<Element> main, List<Element> addon)
        {
            var mixers = _mixers.Where(m => m.CanHandle(main, addon));
            if (!mixers.Any()) return _theLastMixer;
            if (mixers.Count() == 1) return mixers.First();
            Debug.WriteLine("Multiple mixers found.");
            return mixers.First();
        }
    }
}