using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Acme.BookStore.Authors;
using Acme.BookStore.Permissions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Json;
using Volo.Abp.Uow;

namespace Acme.BookStore.Books
{
    [Authorize(BookStorePermissions.Books.Default)]
    public class BookAppService :
        CrudAppService<
            Book, //The Book entity
            BookDto, //Used to show books
            Guid, //Primary key of the book entity
            DataSourceLoadOptions, //Used for paging/sorting
            BookDto>, //Used to create/update a book
        IBookAppService //implement the IBookAppService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IJsonSerializer _jsonSerializer;

        public BookAppService(
            IBookRepository bookRepository,
            IJsonSerializer jsonSerializer,
            IAuthorRepository authorRepository)
            : base(bookRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _jsonSerializer = jsonSerializer;
            GetPolicyName = BookStorePermissions.Books.Default;
            GetListPolicyName = BookStorePermissions.Books.Default;
            CreatePolicyName = BookStorePermissions.Books.Create;
            UpdatePolicyName = BookStorePermissions.Books.Edit;
            DeletePolicyName = BookStorePermissions.Books.Create;
        }

        public override async Task<BookDto> GetAsync(Guid id)
        {
            //Get the IQueryable<Book> from the repository
            var queryable = await Repository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from book in queryable
                        join author in await _authorRepository.GetQueryableAsync() on book.AuthorId equals author.Id
                        where book.Id == id
                        select new { book, author };

            //Execute the query and get the book with author
            var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (queryResult == null)
            {
                throw new EntityNotFoundException(typeof(Book), id);
            }

            var bookDto = ObjectMapper.Map<Book, BookDto>(queryResult.book);
            bookDto.AuthorName = queryResult.author.Name;
            return bookDto;
        }

        public async Task<LoadResult> GetDXListAsync(string bookSearchValues, DataSourceLoadOptions loadOptions)
        {
            BookSearchDto bookSearchDto = new BookSearchDto();
            JsonConvert.PopulateObject(bookSearchValues, bookSearchDto);
            //Get the IQueryable<Book> from the repository
            var queryable = await Repository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from book in queryable
                        join author in await _authorRepository.GetQueryableAsync() on book.AuthorId equals author.Id
                        select new
                        {
                            Id = book.Id,
                            AuthorId = author.Id,
                            AuthorName = author.Name,
                            Name = book.Name,
                            Type = book.Type,
                            PublishDate = book.PublishDate,
                            Price = book.Price
                        };

            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        public async Task<LoadResult> GetAuthorLookupAsync(DataSourceLoadOptions loadOptions)
        {
            var authors = await _authorRepository.GetListAsync();

            return DataSourceLoader.Load(authors, loadOptions);

            //return new ListResultDto<AuthorLookupDto>(
            //    ObjectMapper.Map<List<Author>, List<AuthorLookupDto>>(authors)
            //);
        }

        private static string NormalizeSorting(string sorting)
        {
            if (sorting.IsNullOrEmpty())
            {
                return $"book.{nameof(Book.Name)}";
            }

            if (sorting.Contains("authorName", StringComparison.OrdinalIgnoreCase))
            {
                return sorting.Replace(
                    "authorName",
                    "author.Name",
                    StringComparison.OrdinalIgnoreCase
                );
            }

            return $"book.{sorting}";
        }

        public async Task UpdateAsync(Guid key, string values)
        {
            Book book = await _bookRepository.GetAsync(key);
            JsonConvert.PopulateObject(values, book);
            await _bookRepository.UpdateAsync(book);
        }
    }
}
