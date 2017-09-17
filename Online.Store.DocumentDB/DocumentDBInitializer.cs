using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Online.Store.Core;
using Online.Store.Core.DTOs;
using Online.Store.Storage;
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
        private static string DatabaseId = string.Empty;
        private static DocumentClient client;
        private static string storageAccount = string.Empty;

        public static void Initialize(IConfiguration configuration)
        {
            Endpoint = configuration["DocumentDB:Endpoint"];
            Key = configuration["DocumentDB:Key"];
            DatabaseId = configuration["DocumentDB:DatabaseId"];
            storageAccount = configuration["Storage:AccountName"];

            client = new DocumentClient(new Uri(Endpoint), Key);
            CreateDatabaseIfNotExistsAsync(DatabaseId).Wait();
            // Products Collection
            CreateCollectionIfNotExistsAsync(DatabaseId, "Items").Wait();

            StorageInitializer.Initialize(configuration);
            StorageInitializer.InitContainerAsync("product-images").Wait();
            InitStoreAsync(configuration).Wait();
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

        private static async Task InitStoreAsync(IConfiguration configuration)
        {
            // Init Products
            DocumentDBStoreRepository storeRepository = new DocumentDBStoreRepository(configuration);

            await storeRepository.InitAsync("Items");

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
                        await UploadProductImagesAsync(product);

                        foreach (var component in product.Components)
                        {
                            component.Id = Guid.NewGuid().ToString();
                            foreach(var media in component.Medias)
                            {
                                media.Id = Guid.NewGuid().ToString();
                            }
                        }
                        Document document = await storeRepository.CreateItemAsync(product);
                        string contentType = string.Empty;
                    }
                }
            }
        }

        private static async Task UploadProductImagesAsync(ProductDTO product)
        {
            string[] images = Directory.GetFiles(Path.Combine(Config.ContentRootPath, @"App_Data\images\" + product.Model));

            await StorageInitializer._repository.UploadToContainerAsync("product-images", images[0], product.Model + "/" + Path.GetFileName(images[0]));
            product.Image = string.Format("https://{0}.blob.core.windows.net/{1}/{2}/{3}", storageAccount, "product-images", product.Model, Path.GetFileName(images[0]));

            ProductComponentDTO mediaComponent = product.Components.Where(c => c.ComponentType == "Media").First();

            for(int i=0; i<mediaComponent.Medias.Count; i++)
            {
                await StorageInitializer._repository.UploadToContainerAsync("product-images", images[i+1], product.Model + "/" + Path.GetFileName(images[i+1]));
                ProductMediaDTO media = mediaComponent.Medias[i];
                media.Name = Path.GetFileNameWithoutExtension(images[i + 1]);
                media.Url = string.Format("https://{0}.blob.core.windows.net/{1}/{2}/{3}", storageAccount, "product-images", product.Model, Path.GetFileName(images[i + 1]));
            }
        }
    }
}
