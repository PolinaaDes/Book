using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Book.Commands;
using Book.Data;
using Book.Interfaces;

/*namespace Book.Tests
{
    public class BookManagerTests
    {
        [Fact]
        public async Task AddBookAsync_ShouldAddBookAndSave()
        {
            // Arrange
            var repository = new 
            var mockValidator = new Mock<IValidator>();
            var mockDatabaseConnection = new Mock<IDataRepository>();

            mockDatabaseConnection
                .Setup(fc => fc.SaveBooksAsync(It.IsAny<Books>()))
                .Returns(Task.CompletedTask);

            mockValidator
                .Setup(v => v.ValidateBookAsync(It.IsAny<Books>(), catalog))
                .Callback<Books, List<Books>>((book, bookCatalog) =>
                {
                    if (book == null)
                        throw new ArgumentNullException(nameof(book));
                });

            var bookManager = new BookManager(catalog, mockValidator.Object, mockDatabaseConnection.Object);

            // Имитация ввода данных
            string inputData = "Название книги\nАвтор книги\n978-2-266-11156-0\n2023\nключевое слово, другой ключ\nОписание книги\n";
            Console.SetIn(new System.IO.StringReader(inputData));

            // Act
            await bookManager.AddBookAsync(); // Ожидаем выполнения асинхронного метода

            // Assert
            Assert.Single(catalog); // Убедиться, что книга добавлена
            Assert.Equal("Название книги", catalog[0].Title);
            Assert.Equal("Автор книги", catalog[0].Author);
            Assert.Equal("978-2-266-11156-0", catalog[0].ISBN);

            // Проверка, что методы зависимостей вызваны
            mockDatabaseConnection.Verify(fc => fc.SaveBooksAsync(It.IsAny<Books>()), Times.Once);
            mockValidator.Verify(v => v.ValidateBook(It.IsAny<Books>(), catalog), Times.Once);
        }
    }
}
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Book.Tests
{
    public class BookManagerTests
    {
        private readonly Mock<IValidator> _mockValidator;
        private readonly Mock<IDataRepository> _mockRepository;
        private readonly BookManager _bookManager;

        public BookManagerTests()
        {
            _mockValidator = new Mock<IValidator>();
            _mockRepository = new Mock<IDataRepository>();
            _bookManager = new BookManager(_mockValidator.Object, _mockRepository.Object);
        }

        [Fact]
        public async Task AddBookAsync_ValidBook_AddsBookSuccessfully()
        {
            // Arrange
            var book = new Books("Test Title", "Test Author", "123-456-789", "2024", new List<string> { "Test" }, "Test Description");

            _mockValidator.Setup(v => v.ValidateBookAsync(It.IsAny<Books>(), _mockRepository.Object))
                          .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveBooksAsync(It.IsAny<Books>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _bookManager.AddBookAsync();

            // Assert
            _mockValidator.Verify(v => v.ValidateBookAsync(It.IsAny<Books>(), _mockRepository.Object), Times.Once);
            _mockRepository.Verify(r => r.SaveBooksAsync(It.IsAny<Books>()), Times.Once);
        }

        [Fact]
        public async Task AddBookAsync_InvalidBook_ThrowsInvalidDataException()
        {
            // Arrange
            _mockValidator.Setup(v => v.ValidateBookAsync(It.IsAny<Books>(), _mockRepository.Object))
                          .ThrowsAsync(new InvalidDataException("Invalid book data"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => _bookManager.AddBookAsync());
        }

        [Fact]
        public async Task AddBookAsync_DuplicateBook_ThrowsDuplicateBookException()
        {
            // Arrange
            _mockValidator.Setup(v => v.ValidateBookAsync(It.IsAny<Books>(), _mockRepository.Object))
                          .ThrowsAsync(new DuplicateBookException("Duplicate book"));

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateBookException>(() => _bookManager.AddBookAsync());
        }

        [Theory]
        [InlineData("1", "Test Title")]
        [InlineData("2", "Test Author")]
        [InlineData("3", "123-456-789")]
        [InlineData("4", "Keyword")]
        public async Task FindBookAsync_ValidChoice_ReturnsBooks(string choice, string query)
        {
            // Arrange
            var books = new List<Books>
            {
                new Books("Test Title", "Test Author", "123-456-789", "2024", new List<string> { "Test" }, "Test Description")
            };

            switch (choice)
            {
                case "1":
                    _mockRepository.Setup(r => r.SearchBooksByTitleAsync(query)).ReturnsAsync(books);
                    break;
                case "2":
                    _mockRepository.Setup(r => r.SearchBooksByAuthorAsync(query)).ReturnsAsync(books);
                    break;
                case "3":
                    _mockRepository.Setup(r => r.SearchBooksByISBNAsync(query)).ReturnsAsync(books);
                    break;
                case "4":
                    _mockRepository.Setup(r => r.SearchBooksByKeywordAsync(query)).ReturnsAsync(books);
                    break;
            }

            // Симуляция ввода через Console
            Console.SetIn(new System.IO.StringReader($"{choice}\n{query}\n"));

            // Act
            await _bookManager.FindBookAsync();

            // Assert
            switch (choice)
            {
                case "1":
                    _mockRepository.Verify(r => r.SearchBooksByTitleAsync(query), Times.Once);
                    break;
                case "2":
                    _mockRepository.Verify(r => r.SearchBooksByAuthorAsync(query), Times.Once);
                    break;
                case "3":
                    _mockRepository.Verify(r => r.SearchBooksByISBNAsync(query), Times.Once);
                    break;
                case "4":
                    _mockRepository.Verify(r => r.SearchBooksByKeywordAsync(query), Times.Once);
                    break;
            }
        }


        [Fact]
        public async Task FindBookAsync_InvalidChoice_DisplaysError()
        {
            // Simulate invalid choice in Console input
            Console.SetIn(new System.IO.StringReader("5\n"));

            // Act
            await _bookManager.FindBookAsync();

            // Assert
            // Since the Console output cannot be easily asserted, focus on ensuring no repository method was called
            _mockRepository.Verify(r => r.SearchBooksByTitleAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.SearchBooksByAuthorAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.SearchBooksByISBNAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.SearchBooksByKeywordAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task FindBookAsync_NoResults_DisplaysMessage()
        {
            // Arrange
            _mockRepository.Setup(r => r.SearchBooksByTitleAsync(It.IsAny<string>())).ReturnsAsync(new List<Books>());

            // Simulate Console input for choice and query
            Console.SetIn(new System.IO.StringReader("1\nNo Results\n"));

            // Act
            await _bookManager.FindBookAsync();

            // Assert
            // Ensure repository was called
            _mockRepository.Verify(r => r.SearchBooksByTitleAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
