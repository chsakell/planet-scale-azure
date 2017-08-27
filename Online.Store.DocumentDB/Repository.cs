using Online.Store.Core.DTOs;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.DocumentDB
{
    public class DocumentRepository
    {
        public DocumentRepository(string databaseId, string documentDBEndpoint, string documentDBKey)
        {
            _databaseId = databaseId;
            _documentDBEndpoint = documentDBEndpoint;
            _documentDBKey = documentDBKey;
        }

        #region "Private Members"

        private string _databaseId;
        private string _documentDBEndpoint;
        private string _documentDBKey;
        private Database _database;
        private DocumentClient _client;

        #endregion

        #region "Private Constant Members"

        private const string _PRODUCT_COLLECTION_ID = "Products";
        private const string _CART_COLLECTION_ID = "Cart";
        private const string _COMMUNITY_COLLECTION_ID = "Community";

        #endregion

        #region "Private Properties"

        private String DatabaseId
        {
            get
            {
                return _databaseId;
            }
        }

        private Database Database
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

        private DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    Uri endpointUri = new Uri(_documentDBEndpoint);
                    _client = new DocumentClient(endpointUri, _documentDBKey);
                }

                return _client;
            }
        }

        #endregion

        #region "Public Methods"

        //Products.
        public async Task<List<ProductDTO>> GetProducts()
        {
            var products = await GetAllProducts();
            return products.ToList();
        }

        public async Task CreateProduct(ProductDTO product)
        {
            var productCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _PRODUCT_COLLECTION_ID);
            Document created = await Client.CreateDocumentAsync(productCollection.SelfLink, product);
            var createdData = Guid.Empty;
        }

        //
        public async Task<ProductDTO> GetProductDetails(string productId)
        {
            var product = await GetProductDetail(productId);
            return product;
        }

        //Carts.
        public async Task<CartDTO> GetCartItems(string cartId)
        {
            var cartItems = await GetAllCartItem(cartId);
            return cartItems;
        }

        public async Task AddToCart(CartDTO Item)
        {
            var productCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _CART_COLLECTION_ID);
            Document created = await Client.CreateDocumentAsync(productCollection.SelfLink, Item);
        }

        public async Task UpdateToCart(CartDTO Item)
        {
            var productCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _CART_COLLECTION_ID);
            Document updated = Client.CreateDocumentQuery(productCollection.DocumentsLink).Where(x => x.Id == Item.UserId).AsEnumerable().FirstOrDefault();
            await Client.ReplaceDocumentAsync(updated.SelfLink, Item);
        }

        public async Task RemoveFromCart(CartDTO Item)
        {
            var productCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _CART_COLLECTION_ID);
            Document updated = Client.CreateDocumentQuery(productCollection.DocumentsLink).Where(x => x.Id == Item.UserId).AsEnumerable().FirstOrDefault();
            var options = new RequestOptions();
            if(productCollection.PartitionKey.Paths.Count() > 0)
            {
                options.PartitionKey = new PartitionKey(updated.Id);
            }
            await Client.DeleteDocumentAsync(updated.SelfLink, options);
        }

        //Community
        public async Task<List<CommunityDTO>> GetCommunity()
        {
            var comments = await GetAllCommunity();
            return comments.ToList();
        }

        public async Task AddPost(CommunityDTO Item)
        {
            var communityCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _COMMUNITY_COLLECTION_ID);
            Document created = await Client.CreateDocumentAsync(communityCollection.SelfLink, Item);
        }

        public async Task<CommunityDTO> GetCommunityById(string id)
        {
            var community = await GetCommunityItem(id);
            return community;
        }

        public async Task AddPostResponse(CommunityDTO Item)
        {
            var communityCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _COMMUNITY_COLLECTION_ID);
            Document updated = Client.CreateDocumentQuery(communityCollection.DocumentsLink).Where(x => x.Id == Item.PostId).AsEnumerable().FirstOrDefault();
            await Client.ReplaceDocumentAsync(updated.SelfLink, Item);
        }

        /*public async Task<UserPostsDTO> GetUserPostsCountById(string userId)
        {
            var userPostCount = await GetUserPostsById(userId);
            return userPostCount;
        }

        public async Task AddUserPost(UserPostsDTO Item)
        {
            var collection = await GetOrCreateCollectionAsync(Database.SelfLink, _UEERPOSTS_COLLECTION_ID);
            Document created = await Client.CreateDocumentAsync(collection.SelfLink, Item);
        }

        public async Task UpdateUserPost(UserPostsDTO Item)
        {
            var collection = await GetOrCreateCollectionAsync(Database.SelfLink, _UEERPOSTS_COLLECTION_ID);
            Document updated = Client.CreateDocumentQuery(collection.DocumentsLink).Where(x => x.Id == Item.UserPostId).AsEnumerable().FirstOrDefault();
            await Client.ReplaceDocumentAsync(updated.SelfLink, Item);
        }*/

        #endregion

        #region "Private Methods"

        private DocumentCollection ReadOrCreateCollection(string databaseLink, string collectionId)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == collectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                col = Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = collectionId }).Result;
            }

            return col;
        }

        private Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery().Where(x => x.Id == DatabaseId).AsEnumerable().FirstOrDefault();
            //.Where(d => d.Id == DatabaseId)
            //.AsEnumerable()
            //.FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }

        #endregion

        private async Task<DocumentCollection> GetOrCreateCollectionAsync(string dbLink, string id)
        {
            DocumentCollection collection = _client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null) { collection = await _client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id }); }
            return collection;
        }

        private async Task<List<ProductDTO>> GetAllProducts()
        {
            List<ProductDTO> products = new List<ProductDTO>();
            var productCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _PRODUCT_COLLECTION_ID);

            //fetch all products
            dynamic allProducts = _client.CreateDocumentQuery<Document>(productCollection.SelfLink).AsEnumerable();
            foreach (var product in allProducts)
            {
                var components = new List<ProductComponentDTO>();
                foreach (var component in product.components)
                {
                    var componentMedias = new List<ProductMediaDTO>();
                    foreach (var media in component.medias)
                    {
                        componentMedias.Add(new ProductMediaDTO
                        {
                            Id = media.id,
                            Name = media.name,
                            Type = media.type,
                            UpdatedDate = media.updated,
                            Url = media.url,
                            CreatedDate = media.created,
                            Height = media.height,
                            Width = media.width
                        });
                    }

                    components.Add(new ProductComponentDTO
                    {
                        Id = component.id,
                        ComponentType = component.componenttype,
                        UpdatedDate = component.updated,
                        CreatedDate = component.created,
                        ComponentDetail = component.detail,
                        Medias = componentMedias.ToArray()
                    });
                }

                products.Add(new ProductDTO
                {
                    Id = product.id,
                    Title = product.title,
                    Price = Convert.ToDouble(product.price),
                    UpdatedDate = product.updated,
                    CreatedDate = product.created,
                    Url = product.url,
                    Description = product.description,
                    Components = components.ToArray()
                });
            }

            return products;

        }

        private async Task<CartDTO> GetAllCartItem(string cartId)
        {
            CartDTO cartItems = new CartDTO();
            var cartCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _CART_COLLECTION_ID);

            //fetch all Cart items
            dynamic allCart = _client.CreateDocumentQuery<Document>(cartCollection.SelfLink).Where(x => x.Id == cartId).AsEnumerable().FirstOrDefault();
            cartItems = allCart;
            return cartItems;
        }

        private async Task<ProductDTO> GetProductDetail(string productId)
        {
            var productCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _PRODUCT_COLLECTION_ID);
            dynamic dynamicProduct = _client.CreateDocumentQuery<Document>(productCollection.SelfLink).Where(x => x.Id == productId).AsEnumerable().FirstOrDefault();
            ProductDTO product = dynamicProduct;
            return product;
        }

        private async Task<List<CommunityDTO>> GetAllCommunity()
        {
            List<CommunityDTO> communities = new List<CommunityDTO>();
            var communityCollection = await GetOrCreateCollectionAsync(Database.SelfLink, _COMMUNITY_COLLECTION_ID);

            //fetch all products
            dynamic allCommunity = _client.CreateDocumentQuery<Document>(communityCollection.SelfLink).AsEnumerable();
            foreach (dynamic community in allCommunity)
            {
                var item = new CommunityDTO();
                item = community;
                communities.Add(item);
            }

            return communities;
        }

        private async Task<CommunityDTO> GetCommunityItem(string id)
        {
            CommunityDTO communityItem = new CommunityDTO();
            var collection = await GetOrCreateCollectionAsync(Database.SelfLink, _COMMUNITY_COLLECTION_ID);
            //fetch item
            dynamic item = _client.CreateDocumentQuery<Document>(collection.SelfLink).Where(x => x.Id == id).AsEnumerable().FirstOrDefault();
            communityItem = item;
            return communityItem;
        }

        //public async Task<UserPostsDTO> GetUserPostsById(string userId)
        //{
        //    UserPostsDTO result = new UserPostsDTO();
        //    var collection = await GetOrCreateCollectionAsync(Database.SelfLink, _UEERPOSTS_COLLECTION_ID);
        //    dynamic item = _client.CreateDocumentQuery<Document>(collection.SelfLink).Where(x => x.Id == userId).AsEnumerable().FirstOrDefault();
        //    result = item;
        //    return result;
        //}
    }
}
