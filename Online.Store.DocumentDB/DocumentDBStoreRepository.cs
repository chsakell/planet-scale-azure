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
    public class DocumentDBStoreRepository : DocumentDBRepositoryBase<DocumentDBStoreRepository>, IDocumentDBRepository<DocumentDBStoreRepository>
    {
        public DocumentDBStoreRepository(IConfiguration configuration)
        {
            Endpoint = configuration["DocumentDB:Endpoint"];
            Key = configuration["DocumentDB:Key"];
            DatabaseId = configuration["DocumentDB:DatabaseId"];
        }

        public override async Task<DocumentCollection> InitAsync(string collectionId)
        {
            if (_client == null)
                _client = new DocumentClient(new Uri(Endpoint), Key);

            if (CollectionId != collectionId)
            {
                CollectionId = collectionId;
                _collection = await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }

            return _collection;
        }
    }
}
