using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;
using Microsoft.Extensions.Configuration;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {
        private SearchAppServices _searchService;

        public SearchController(IConfiguration configuration)
        {
            _searchService = new SearchAppServices(configuration["SearchService:Name"], configuration["SearchService:ApiKey"]);
        }

        // GET: api/Search/5
        [HttpGet("{id}", Name = "Search")]
        public string Get(string id)
        {
            var resultProduct = _searchService.SearchProduct(id, null);

            return string.Empty;
        }
        
    }
}
