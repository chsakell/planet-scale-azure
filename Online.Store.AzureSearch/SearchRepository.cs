using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Online.Store.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.AzureSearch
{
    public abstract class SearchRepository : ISearchRepository
    {
        protected SearchServiceClient _serviceClient;
        protected string _documentDbEndpoint;
        protected string _documentDbAccountKey;
        protected string _documentDbDatabase;

        public async Task CreateOrUpdateDocumentDbDataSourceAsync(string dataSourceName, string dataSourceColletion)
        {
            string connectionString = string.Format("AccountEndpoint={0};AccountKey={1};Database={2}",
                _documentDbEndpoint, _documentDbAccountKey, _documentDbDatabase);
            DataSourceCredentials credentials = new DataSourceCredentials(connectionString);
            DataContainer container = new DataContainer(dataSourceColletion);
            DataSource docDbSource = new DataSource(dataSourceName, DataSourceType.DocumentDb, credentials, container);
            await _serviceClient.DataSources.CreateOrUpdateAsync(docDbSource);
        }

        public async Task CreateOrUpdateIndexAsync<T>(string indexName, string suggesterName, List<string> suggesterFields)
        {
            List<Suggester> suggesters = new List<Suggester>();
            suggesters.Add(new Suggester(suggesterName, SuggesterSearchMode.AnalyzingInfixMatching, suggesterFields));
            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<T>(),
                Suggesters = suggesters
            };

            await _serviceClient.Indexes.CreateOrUpdateAsync(definition);
        }

        public async Task CreateOrUpdateIndexerAsync(string indexerName, string dataSource, string index)
        {
            try
            {
                var indexers = await _serviceClient.Indexers.ListAsync();

                foreach (var indexer in indexers.Indexers) {
                    await _serviceClient.Indexers.DeleteAsync(indexer.Name);
                }
            }
            catch { }
            await _serviceClient.Indexers.CreateAsync(new Indexer(indexerName, dataSource, index));
        }

        public async Task<List<T>> SearchAsync<T>(string searchIndex, string term, List<string> returnFields) where T : class
        {
            SearchParameters parameters;
            DocumentSearchResult<T> results;

            ISearchIndexClient indexClient = _serviceClient.Indexes.GetClient(searchIndex);

            parameters =
                new SearchParameters()
                {
                    Select = returnFields
                };

            results = await indexClient.Documents.SearchAsync<T>(term, parameters);

            return results.Results.Select(r => r.Document).ToList();
        }
    }
}
