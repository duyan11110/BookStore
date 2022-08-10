using Acme.BookStore.EntityFrameworkCore;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.BookStore.Books
{
    public class EfCoreBookRepository : EfCoreRepository<BookStoreDbContext, Book, Guid>,
            IBookRepository
    {
        public EfCoreBookRepository(
            IDbContextProvider<BookStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<LoadResult> GetListAsync(DataSourceLoadOptions loadOptions)
        {
            var dbContext = await GetDbContextAsync();
            var query = from book in dbContext.Books
                        join author in dbContext.Authors on book.AuthorId equals author.Id
                        select new { book, author };
            return DataSourceLoader.Load(query, loadOptions);
        }
    }
}
