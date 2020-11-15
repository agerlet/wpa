namespace Word2Rtf.Models
{
    public class Version
    {
        public string Name { get; set; }
        public Language Language { get; set; }
        public NameFormat Format { get; set; }
        public Chinese? Chinese { get; set; }
    }
}