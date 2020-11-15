using System.Collections.Generic;

namespace Word2Rtf.Models
{
    public class SectionsSettings
    {
        public Section[] Sections { get; set; }
    }

    public class Section
    {
        public ElementType SectionType { get; set; }
        public List<Verse> Instances { get; set; }
    }
}