using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;
using Microsoft.Extensions.Configuration;
using Online.Store.ViewModels;
using AutoMapper;
using Online.Store.Core.DTOs;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {
        private IAzureSearchService _searchService;
        private string _cdnEndpoint;

        public SearchController(IAzureSearchService searchService)
        {
            _searchService = searchService;
        }

        // GET: api/Search/term
        [HttpGet("products/", Name = "Search")]
        public async Task<IEnumerable<SearchResultViewModel>> Get(string term)
        {
            var products = new List<SearchResultViewModel>();
            var productsSearch = await _searchService.SearchProductsAsync(term);

            products = Mapper.Map<IEnumerable<ProductInfo>, List<SearchResultViewModel>>(productsSearch);

            //if (productsSearch != null && productsSearch.value != null)
            //{
            //    foreach (var item in productsSearch.value)
            //    {
            //        string productImage = this._cdnEndpoint + "product-images/" + item.model + "/1.jpg";
            //        products.Add(new SearchResultViewModel()
            //        {
            //            Id = item.id,
            //            Title = item.title,
            //            Description = item.description,
            //            Image = productImage
            //        });
            //    }
            //}

            return products;
        }
        
    }
}
