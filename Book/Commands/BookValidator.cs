﻿using Book.Interfaces;
using Book.Data;
using Microsoft.EntityFrameworkCore;

namespace Book.Commands
{
    public class BookValidator : IValidator
    {
        // Метод для валидации книги
        public async Task ValidateBookAsync(Books book, IDataRepository repository)
        {
            // Проверка на null
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book), "Ошибка: Книга не может быть null.");
            }

            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository), "Ошибка: Каталог не может быть null.");
            }
            
            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author) ||
                string.IsNullOrWhiteSpace(book.ISBN) || book.Keywords == null || book.Keywords.Count == 0 ||
                string.IsNullOrWhiteSpace(book.Description))
            {
                throw new InvalidException("Ошибка: Все поля должны быть заполнены.");
            }

            var existingBooks = await repository.SearchBooksByISBNAsync(book.ISBN);
            if (existingBooks.Any(b => b.ISBN.Equals(book.ISBN, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateBookException("Ошибка: Книга с таким ISBN уже существует в базе данных.");
            }
        }
    }
}