using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.AzureSearch
{
    public interface ISearchRepository
    {
        Task CreateOrUpdateDocumentDbDataSourceAsync(string dataSourceName, string dataSourceColletion);
        Task CreateOrUpdateIndexAsync<T>(string indexName, string suggesterName, List<string> suggesterFields);
        Task<List<T>> SearchAsync<T>(string searchIndex, string term, List<string> returnFields) where T : class;
    }
}
