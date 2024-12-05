using System.Collections;
using Book.Interfaces;

namespace Book.Data
{
    // Класс книги, реализующий интерфейс ISearchable
    public class Books : ISearchable, IEnumerable<string>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Year { get; set; }
        public List<string> Keywords { get; set; }
        public string Description { get; set; }
        
        public Books() {}

        public Books(string title, string author, string isbn, string year, List<string> keywords, string description)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            Year = year;
            Keywords = keywords;
            Description = description;
        }

        public override string ToString()
        {
            string keywordFormatted = string.Join(", ", Keywords);
            return $"Title: {Title}\nAuthor: {Author}\nISBN: {ISBN}\nYear: {Year}\nKeywords: {keywordFormatted}\nDescription: {Description}\n";
        }

        public IEnumerator<string> GetEnumerator() => Keywords.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}