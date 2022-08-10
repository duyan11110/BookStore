using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.BookStore.Books
{
    public class BookSearchDto
    {
        public BookType? BookType { get; set; }
        public Guid? AuthorId { get; set; }
    }
}
