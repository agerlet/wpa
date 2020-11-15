using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Word2Rtf.Models;

namespace Word2Rtf.Mixers
{
    public class MixerFactory
    {
        private readonly IMixer _theLastMixer;
        private readonly List<IMixer> _mixers;
        public MixerFactory(IEnumerable<IMixer> mixers)
        {
            _mixers = mixers.SkipLast(1).ToList();
            _theLastMixer = mixers.Last();
        }

        public IMixer GetMixer(List<Element> main, List<Element> addon)
        {
            var mixers = _mixers.Where(m => m.CanHandle(main, addon));
            if (!mixers.Any()) return _theLastMixer;
            if (mixers.Count() == 1) return mixers.Single();
            Debug.WriteLine("Multiple mixers found.");
            return mixers.First();
        }
    }
}