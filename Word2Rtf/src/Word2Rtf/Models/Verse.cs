using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Word2Rtf.Tests")]
namespace Word2Rtf.Models
{
    internal class Verse
    {
        public Language Language { get; set; }
        public string Content { get; set; }

    }
}