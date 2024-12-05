using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Xunit;

namespace Book.Tests
{
    public class DatabaseConnectionTests
    {
        private BookContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BookContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Уникальная база для каждого теста
                .Options;
            return new BookContext(options);
        }

        [Fact]
        public async Task SaveBooksAsync_ValidBook_SavesToDatabase()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var databaseConnection = new DatabaseConnection(context);

            var newBook = new Books("Test Title", "Test Author", "123-456-789", "2024", new List<string> { "Keyword1" }, "Test Description");

            // Act
            await databaseConnection.SaveBooksAsync(newBook);

            // Assert
            var savedBook = await context.Books.FirstOrDefaultAsync(b => b.ISBN == "123-456-789");
            Assert.NotNull(savedBook);
            Assert.Equal("Test Title", savedBook.Title);
            Assert.Equal("Test Author", savedBook.Author);
        }

        [Fact]
        public async Task SaveBooksAsync_DuplicateISBN_DoesNotSaveDuplicate()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var databaseConnection = new DatabaseConnection(context);

            var existingBook = new Books("Existing Title", "Existing Author", "123-456-789", "2024", new List<string> { "Keyword1" }, "Existing Description");
            await context.Books.AddAsync(existingBook);
            await context.SaveChangesAsync();

            var newBook = new Books("New Title", "New Author", "123-456-789", "2025", new List<string> { "Keyword2" }, "New Description");

            // Act
            await databaseConnection.SaveBooksAsync(newBook);

            // Assert
            var books = await context.Books.Where(b => b.ISBN == "123-456-789").ToListAsync();
            Assert.Single(books); // Ожидается, что будет только одна запись
            Assert.Equal("Existing Title", books.First().Title); // Существующая запись остается
        }

        [Fact]
        public async Task LoadBooksAsync_ReturnsAllBooks()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var databaseConnection = new DatabaseConnection(context);

            var books = new List<Books>
            {
                new Books("Title1", "Author1", "ISBN1", "2020", new List<string> { "Keyword1" }, "Description1"),
                new Books("Title2", "Author2", "ISBN2", "2021", new List<string> { "Keyword2" }, "Description2"),
            };
            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();

            // Act
            var loadedBooks = await databaseConnection.LoadBooksAsync();

            // Assert
            Assert.Equal(2, loadedBooks.Count);
            Assert.Contains(loadedBooks, b => b.Title == "Title1");
            Assert.Contains(loadedBooks, b => b.Title == "Title2");
        }
    }
}
