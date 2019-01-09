using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Word2Rtf.Tests")]
namespace Word2Rtf.Models
{
    internal class Element
    {
        public ElementType ElementType { get; set; }
        public string Input { get; set; }
        public IEnumerable<Verse> Verses { get; set; }
        public bool Pass
        {
            get
            {
                return Verses != null
                    && Verses.Count() >= 1
                    && Verses.All(v => !string.IsNullOrWhiteSpace(v.Content));
            }
        }
    } 
    
}