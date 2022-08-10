using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Books
{
    public interface IBookRepository : IRepository<Book, Guid>
    {
        Task<LoadResult> GetListAsync(DataSourceLoadOptions loadOptions);
    }
}
