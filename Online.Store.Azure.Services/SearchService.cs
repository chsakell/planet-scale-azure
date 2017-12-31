using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Online.Store.Core.DTOs;
using Online.Store.AzureSearch;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class AzureSearchService : IAzureSearchService
    {
        private const string productIndexName = "product-index";
        ISearchRepository _searchRepository;

        public AzureSearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<List<ProductInfo>> SearchProductsAsync(string term)
        {
            return await _searchRepository.SearchAsync<ProductInfo>(productIndexName, term, 
                new List<string> { "id", "title", "description", "image" });
        }
    }

    public interface IAzureSearchService
    {
        Task<List<ProductInfo>> SearchProductsAsync(string term);
    }

}
