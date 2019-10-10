using System;

namespace Word2Rtf.Models
{
    public enum Language
    {
        Chinese,
        English,
        Mixed,
    }

    public enum Chinese
    {
        Simplified,
        Traditional,
    }

    [Flags]
    public enum ElementType
    {
        Title = 0,
        Content = 1,
        Lyrics = 1 << 1,
        BibleVerses = 1 << 2,
        YouTubeLink = 1 << 3,
    }

    public enum NameFormat
    {
        Full,
        Short,
    }
}