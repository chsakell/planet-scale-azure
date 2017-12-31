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

            return products;
        }
        
    }
}
