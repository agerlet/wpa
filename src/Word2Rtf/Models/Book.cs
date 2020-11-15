namespace Word2Rtf.Models
{
    public class BooksSettings
    {
        public Book[] Books { get; set; }
    }

    public class Book
    {
        public Version[] Versions { get; set; }
    }
}