using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.DocumentDB
{
    public class DocumentDBStoreRepository : DocumentDBRepositoryBase<DocumentDBStoreRepository>, IDocumentDBRepository<DocumentDBStoreRepository>
    {
        public DocumentDBStoreRepository(string endpoint, string key)
        {
            //Endpoint = configuration["DocumentDBEndpoint"];
            //Key = configuration["DocumentDBKey"];
            DatabaseId = "Store";
        }

        public override async Task InitAsync(string collectionId)
        {
            if (client == null)
                client = new DocumentClient(new Uri(Endpoint), Key);

            if (CollectionId != collectionId)
            {
                CollectionId = collectionId;
                collection = await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
        }
    }
}
