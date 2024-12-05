using Book.Interfaces;
using Book.Data;

namespace Book.Commands
{

    public class BookSearcher
    {
        public List<Books> SearchByTitle(List<Books> catalog, string title)
        {
            return catalog
                .Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)) // Фильтрация по названию
                .ToList(); // Преобразование результата в список
        }

        // Метод для поиска книг по имени автора
        public List<Books> SearchByAuthor(List<Books> catalog, string author)
        {
            return catalog // Возврат списка книг
                .Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)) // Фильтрация по автору
                .ToList(); // Преобразование результата в список
        }

        // Метод для поиска книги по ISBN
        public List<Books> SearchByISBN(List<Books> catalog, string isbn)
        {
            return catalog // Возврат списка книг
                .Where(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase)) // Фильтрация по ISBN
                .ToList(); // Преобразование результата в список
        }

        // Метод для поиска книг по ключевым словам
        public List<Books> SearchByKeywords(List<Books> catalog, string searchQuery)
        {
            return catalog // Возврат списка книг
                .Select(book => new
                {
                    Book = book, // Запись книги
                    Matches =
                        book.Keywords.Count(k =>
                            k.Contains(searchQuery,
                                StringComparison.OrdinalIgnoreCase)) + // Количество совпадений ключевых слов
                        (book.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                            ? 1
                            : 0) // Учет аннотации
                })
                .Where(x => x.Matches > 0) // Фильтрация книг с найденными совпадениями
                .OrderByDescending(x => x.Matches) // Сортировка по количеству совпадений
                .Select(x => x.Book) // Возврат списка книг
                .ToList(); // Преобразование результата в список
        }
    }
}