using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;
using Microsoft.Extensions.Configuration;
using Online.Store.ViewModels;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {
        private SearchAppServices _searchService;
        private string _cdnEndpoint;

        public SearchController(IConfiguration configuration)
        {
            _searchService = new SearchAppServices(configuration["SearchService:Name"], configuration["SearchService:ApiKey"]);
            this._cdnEndpoint = configuration["CDN:Endpoint"];
        }

        // GET: api/Search/term
        [HttpGet("products/", Name = "Search")]
        public async Task<IEnumerable<SearchResultViewModel>> Get(string term)
        {
            var products = new List<SearchResultViewModel>();
            var productsSearch = _searchService.SearchProduct(term, null);

            if (productsSearch != null && productsSearch.value != null)
            {
                foreach (var item in productsSearch.value)
                {
                    string productImage = this._cdnEndpoint + "product-images/" + item.model + "/1.jpg";
                    products.Add(new SearchResultViewModel()
                    {
                        Id = item.id,
                        Title = item.title,
                        Description = item.description,
                        Image = productImage
                    });
                }
            }

            return products;
        }
        
    }
}
