using Acme.BookStore.Books;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Acme.BookStore.Web.Pages
{
    public class IndexModel : BookStorePageModel
    {
        IBookAppService _bookAppService;

        public IndexModel(IBookAppService bookAppService)
        {
            _bookAppService = bookAppService;
        }

        public void OnGet()
        {
        }
    }
}