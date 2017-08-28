using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Online.Store.Core;
using Online.Store.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.DocumentDB
{
    public class DocumentDBInitializer
    {
        private static string Endpoint = string.Empty;
        private static string Key = string.Empty;
        private static DocumentClient client;

        public static void Initialize(string endpoint, string key)
        {
            //Endpoint = configuration["DocumentDBEndpoint"];
            //Key = configuration["DocumentDBKey"];

            Endpoint = endpoint;
            Key = key;

            client = new DocumentClient(new Uri(Endpoint), Key);
            CreateDatabaseIfNotExistsAsync("Store").Wait();
            // Products Collection
            CreateCollectionIfNotExistsAsync("Store", "Products", "category").Wait();
            CreateCollectionIfNotExistsAsync("Store", "Categories").Wait();
            CreateCollectionIfNotExistsAsync("Store", "Suppliers").Wait();

            InitGalleryAsync(Endpoint, Key).Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync(string DatabaseId)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync(string DatabaseId, string CollectionId, string partitionkey = null)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    if (string.IsNullOrEmpty(partitionkey))
                    {
                        await client.CreateDocumentCollectionAsync(
                            UriFactory.CreateDatabaseUri(DatabaseId),
                            new DocumentCollection { Id = CollectionId },
                            new RequestOptions { OfferThroughput = 1000 });
                    }
                    else
                    {
                        await client.CreateDocumentCollectionAsync(
                            UriFactory.CreateDatabaseUri(DatabaseId),
                            new DocumentCollection
                            {
                                Id = CollectionId,
                                PartitionKey = new PartitionKeyDefinition
                                {
                                    Paths = new Collection<string> { "/" + partitionkey }
                                }
                            },
                            new RequestOptions { OfferThroughput = 1000 });
                    }

                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateTriggerIfNotExistsAsync(string databaseId, string collectionId, string triggerName, string triggerPath)
        {
            DocumentCollection collection = await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));

            string triggersLink = collection.TriggersLink;
            string TriggerName = triggerName;

            Trigger trigger = client.CreateTriggerQuery(triggersLink)
                                    .Where(sp => sp.Id == TriggerName)
                                    .AsEnumerable()
                                    .FirstOrDefault();

            if (trigger == null)
            {
                // Register a pre-trigger
                trigger = new Trigger
                {
                    Id = TriggerName,
                    Body = File.ReadAllText(Path.Combine(Config.ContentRootPath, triggerPath)),
                    TriggerOperation = TriggerOperation.Create,
                    TriggerType = TriggerType.Pre
                };

                await client.CreateTriggerAsync(triggersLink, trigger);
            }
        }

        private static async Task CreateStoredProcedureIfNotExistsAsync(string databaseId, string collectionId, string procedureName, string procedurePath)
        {
            DocumentCollection collection = await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));

            string storedProceduresLink = collection.StoredProceduresLink;
            string StoredProcedureName = procedureName;

            StoredProcedure storedProcedure = client.CreateStoredProcedureQuery(storedProceduresLink)
                                    .Where(sp => sp.Id == StoredProcedureName)
                                    .AsEnumerable()
                                    .FirstOrDefault();

            if (storedProcedure == null)
            {
                // Register a stored procedure
                storedProcedure = new StoredProcedure
                {
                    Id = StoredProcedureName,
                    Body = File.ReadAllText(Path.Combine(Config.ContentRootPath, procedurePath))
                };
                storedProcedure = await client.CreateStoredProcedureAsync(storedProceduresLink,
            storedProcedure);
            }
        }

        private static async Task CreateUserDefinedFunctionIfNotExistsAsync(string databaseId, string collectionId, string udfName, string udfPath)
        {
            DocumentCollection collection = await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));

            UserDefinedFunction userDefinedFunction =
                        client.CreateUserDefinedFunctionQuery(collection.UserDefinedFunctionsLink)
                            .Where(udf => udf.Id == udfName)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (userDefinedFunction == null)
            {
                // Register User Defined Function
                userDefinedFunction = new UserDefinedFunction
                {
                    Id = udfName,
                    Body = System.IO.File.ReadAllText(Path.Combine(Config.ContentRootPath, udfPath))
                };

                await client.CreateUserDefinedFunctionAsync(collection.UserDefinedFunctionsLink, userDefinedFunction);
            }
        }

        private static async Task InitGalleryAsync(string endpoint, string key)
        {
            // Init Products
            DocumentDBStoreRepository storeRepository = new DocumentDBStoreRepository(endpoint, key);

            await storeRepository.InitAsync("Products");

            var productsDB = await storeRepository.GetItemsAsync<ProductDTO>();
            if (productsDB.Count() == 0)
            {
                List<ProductDTO> products = null;

                using (StreamReader r = new StreamReader(Path.Combine(Config.ContentRootPath, @"App_Data\products.json")))
                {
                    string json = r.ReadToEnd();
                    products = JsonConvert.DeserializeObject<List<ProductDTO>>(json);

                    foreach (var product in products)
                    {
                        Document document = await storeRepository.CreateItemAsync(product);
                        string contentType = string.Empty;

                        /*
                        new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider().TryGetContentType(product.ProductPicUrl, out contentType);
                        var attachment = new Attachment { ContentType = contentType, Id = "picture", MediaLink = product.ProductPicUrl };
                        ResourceResponse<Attachment> createdAttachment = await storeRepository.CreateAttachmentAsync(document.AttachmentsLink, attachment, new RequestOptions() { PartitionKey = new PartitionKey(product.Category) });
                        */
                    }
                }
            }

            await storeRepository.InitAsync("Categories");

            var categoriesDB = await storeRepository.GetItemsAsync<CustomItem>();
            if (categoriesDB.Count() == 0)
            {
                List<CustomItem> categories = null;

                using (StreamReader r = new StreamReader(Path.Combine(Config.ContentRootPath, @"App_Data\categories.json")))
                {
                    string json = r.ReadToEnd();
                    categories = JsonConvert.DeserializeObject<List<CustomItem>>(json);

                    foreach (var category in categories)
                    {
                        Document document = await storeRepository.CreateItemAsync(category);
                    }
                }
            }

            await storeRepository.InitAsync("Suppliers");

            var suppliersDB = await storeRepository.GetItemsAsync<CustomItem>();
            if (suppliersDB.Count() == 0)
            {
                List<CustomItem> suppliers = null;

                using (StreamReader r = new StreamReader(Path.Combine(Config.ContentRootPath, @"App_Data\suppliers.json")))
                {
                    string json = r.ReadToEnd();
                    suppliers = JsonConvert.DeserializeObject<List<CustomItem>>(json);

                    foreach (var supplier in suppliers)
                    {
                        Document document = await storeRepository.CreateItemAsync(supplier);
                    }
                }
            }
        }
    }

    class CustomItem
    {
        public string Name { get; set; }
    }
}
