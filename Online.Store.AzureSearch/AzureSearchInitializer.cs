using Microsoft.Extensions.Configuration;
using Online.Store.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.AzureSearch
{
    public class AzureSearchInitializer
    {
        static string _productsDataSource = "products-datasource";
        static string _dataSourceCollection = "Items";
        static string _productIndex = "product-index";
        static string _productSuggester = "product-suggester";
        static string _productsIndexer = "products-indexer";
        public static async Task Initialize(IConfiguration configuration)
        {
            SearchStoreRepository _searchStoreRepository = new SearchStoreRepository(configuration);

            await _searchStoreRepository.CreateOrUpdateDocumentDbDataSourceAsync(_productsDataSource, _dataSourceCollection);

            await _searchStoreRepository.CreateOrUpdateIndexAsync<ProductInfo>(_productIndex, _productSuggester, new List<string> { "title", "description" });

            await _searchStoreRepository.CreateOrUpdateIndexerAsync(_productsIndexer, _productsDataSource, _productIndex);
        }
    }
}
