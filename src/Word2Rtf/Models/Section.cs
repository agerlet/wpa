using System.Collections.Generic;

namespace Word2Rtf.Models
{
    internal class Section
    {
        public ElementType SectionType { get; set; }
        public List<Verse> Instances { get; set; }
    }
}