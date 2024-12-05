using Book.Commands;
using Book.Data;

namespace Book.Tests
{
    public class BookSearcherTests
    {
        [Fact]
        public void SearchByTitle_ShouldReturnMatchingBooks()
        {
            // Arrange
            var catalog = new List<Books>
            {
                new Books("C# Programming", "Author A", "978-2-266-11156-0", "2022", new List<string> { "Programming" },
                    "Description 1"),
                new Books("Python Programming", "Author B", "978-2-266-10000-1", "2021", new List<string> { "Coding" },
                    "Description 2")
            };

            var searcher = new BookSearcher();

            // Act
            var results = searcher.SearchByTitle(catalog, "Programming");

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Contains(results, b => b.Title == "C# Programming");
            Assert.Contains(results, b => b.Title == "Python Programming");
        }
    }
}