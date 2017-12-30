using Microsoft.Azure.Search;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.AzureSearch
{
    public class SearchStoreRepository : SearchRepository
    {
        public SearchStoreRepository(IConfiguration configuration)
        {
            _documentDbEndpoint = configuration["DocumentDB:Endpoint"];
            _documentDbAccountKey = configuration["DocumentDB:Key"];
            _documentDbDatabase = configuration["DocumentDB:DatabaseId"];

            string _azureSearchServiceName = configuration["SearchService:Name"];
            string _azureSearchServiceKey = configuration["SearchService:ApiKey"];

            _serviceClient = new SearchServiceClient(_azureSearchServiceName, new SearchCredentials(_azureSearchServiceKey));

        }
    }
}
