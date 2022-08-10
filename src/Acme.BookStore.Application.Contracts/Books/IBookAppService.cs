using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Books
{
    public interface IBookAppService :
        ICrudAppService< //Defines CRUD methods
            BookDto, //Used to show books
            Guid, //Primary key of the book entity
            DataSourceLoadOptions, //Used for paging/sorting
            BookDto> //Used to create/update a book
    {
        Task<LoadResult> GetAuthorLookupAsync(DataSourceLoadOptions loadOptions);
        Task<LoadResult> GetDXListAsync(string bookSearchValues, DataSourceLoadOptions loadOptions);
        Task UpdateAsync(Guid key, string values);
    }
}