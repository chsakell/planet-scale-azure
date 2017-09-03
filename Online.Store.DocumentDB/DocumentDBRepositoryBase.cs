using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.DocumentDB
{
    public abstract class DocumentDBRepositoryBase<DatabaseDB> : IDocumentDBRepository<DatabaseDB>
    {
        #region Repository Configuration

        protected string Endpoint = string.Empty;
        protected string Key = string.Empty;
        protected string DatabaseId = string.Empty;
        protected string CollectionId = string.Empty;
        protected Database _database;
        protected DocumentClient _client;
        protected DocumentCollection _collection;

        #endregion

        protected Database Database
        {
            get
            {
                if (_database == null)
                {
                    _database = ReadOrCreateDatabase();
                }

                return _database;
            }
        }

        public DocumentDBRepositoryBase()
        {

        }

        public async Task<T> GetItemAsync<T>(string id) where T : class
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<T> GetItemAsync<T>(string id, string partitionKey) where T : class
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Document> GetDocumentAsync(string id, string partitionKey)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                return document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>() where T : class
        {
            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public IEnumerable<T> CreateDocumentQuery<T>(string query, FeedOptions options) where T : class
        {
            return _client.CreateDocumentQuery<T>(_collection.DocumentsLink, query, options).AsEnumerable();
        }

        public async Task<Document> CreateItemAsync<T>(T item) where T : class
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public async Task<Document> CreateItemAsync<T>(T item, RequestOptions options) where T : class
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item, options);
        }

        public async Task<Document> UpdateItemAsync<T>(string id, T item) where T : class
        {
            return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public async Task<ResourceResponse<Attachment>> CreateAttachmentAsync(string attachmentsLink, object attachment, RequestOptions options)
        {
            return await _client.CreateAttachmentAsync(attachmentsLink, attachment, options);
        }

        public async Task<ResourceResponse<Attachment>> ReadAttachmentAsync(string attachmentLink, string partitionkey)
        {
            return await _client.ReadAttachmentAsync(attachmentLink, new RequestOptions() { PartitionKey = new PartitionKey(partitionkey) });
        }

        public async Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(Attachment attachment, RequestOptions options)
        {
            return await _client.ReplaceAttachmentAsync(attachment, options);
        }

        public async Task DeleteItemAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
        }

        public async Task<StoredProcedureResponse<dynamic>> ExecuteStoredProcedureAsync(string procedureName, string query, string partitionKey)
        {
            StoredProcedure storedProcedure = _client.CreateStoredProcedureQuery(_collection.StoredProceduresLink)
                                    .Where(sp => sp.Id == procedureName)
                                    .AsEnumerable()
                                    .FirstOrDefault();

            return await _client.ExecuteStoredProcedureAsync<dynamic>(storedProcedure.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(partitionKey) }, query);

        }

        public abstract Task InitAsync(string collectionId);

        #region Private methods
        private Database ReadOrCreateDatabase()
        {
            var db = _client.CreateDatabaseQuery().Where(x => x.Id == DatabaseId).AsEnumerable().FirstOrDefault();

            if (db == null)
            {
                db = _client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }
        #endregion
    }
}
