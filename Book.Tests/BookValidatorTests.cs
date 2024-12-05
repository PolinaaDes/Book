/*using Book.Commands;
using Book.Data;

namespace Book.Tests;

public class BookValidatorTests
{
    
    [Fact]
    public void ValidateBook_ShouldThrowArgumentNullException_WhenBookIsNull()
    {
        // Arrange
        var validator = new BookValidator();
        var catalog = new List<Books>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => validator.ValidateBookAsync(null, catalog));
    }

    [Fact]
    public void ValidateBook_ShouldThrowArgumentNullException_WhenCatalogIsNull()
    {
        // Arrange
        var validator = new BookValidator();
        var book = new Books("Title", "Author", "ISBN", "Year", new List<string> { "Keyword" }, "Description");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => validator.ValidateBookAsync(book, null));
    }

    [Fact]
    public void ValidateBook_ShouldThrowDuplicateBookException_WhenISBNExists()
    {
        // Arrange
        var validator = new BookValidator();
        var catalog = new List<Books>
        {
            new Books("Existing Book", "Author", "978-2-266-11156-0", "2021", new List<string> { "Keyword" }, "Description")
        };
        var book = new Books("New Book", "Author", "978-2-266-11156-0", "2022", new List<string> { "Keyword" }, "Description");

        // Act & Assert
        Assert.Throws<DuplicateBookException>(() => validator.ValidateBook(book, catalog));
    }
}*/