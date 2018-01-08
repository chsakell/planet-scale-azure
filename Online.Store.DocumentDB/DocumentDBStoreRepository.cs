using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Online.Store.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.DocumentDB
{
    public class DocumentDBStoreRepository : DocumentDBRepositoryBase
    {
        private List<string> ConnectionPolicies;
        public DocumentDBStoreRepository(IConfiguration configuration)
        {
            Endpoint = configuration["DocumentDB:Endpoint"];
            Key = configuration["DocumentDB:Key"];
            DatabaseId = configuration["DocumentDB:DatabaseId"];

            string policies = configuration["DocumentDB:ConnectionPolicies"];
            if(!string.IsNullOrEmpty(policies))
            {
                ConnectionPolicies = policies.Split(",").ToList();
            }
        }

        public override async Task<DocumentCollection> InitAsync(string collectionId)
        {
            ConnectionPolicy connectionPolicy = new ConnectionPolicy();

            //Setting read region selection preference
            foreach (var policy in ConnectionPolicies)
            {
                connectionPolicy.PreferredLocations.Add(policy);
            }

            // Check Microsoft.Azure.Documents.LocationNames
            //connectionPolicy.PreferredLocations.Add(LocationNames.WestCentralUS); // first preference
            //connectionPolicy.PreferredLocations.Add(LocationNames.EastUS); // second preference

            if (_client == null)
                _client = new DocumentClient(new Uri(Endpoint), Key, connectionPolicy);

            if (CollectionId != collectionId)
            {
                CollectionId = collectionId;
                _collection = await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }

            return _collection;
        }
    }
}
