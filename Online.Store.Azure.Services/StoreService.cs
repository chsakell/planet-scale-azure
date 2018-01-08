using Microsoft.Azure.Documents;
using Online.Store.Core.DTOs;
using Online.Store.DocumentDB;
using Online.Store.RedisCache;
using Online.Store.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class StoreService : IStoreService
    {
        #region "Private Constant Members"

        private const string _PRODUCT_COLLECTION_ID = "Items";
        private const string _FORUM_COLLECTION_ID = "Forum";
        #endregion

        /// <summary>
        /// The DocumentDB _repository
        /// </summary>
        private IDocumentDBRepository _docDbRepository;

        private IRedisCacheRepository _cacheRepository;

        public StoreService(IDocumentDBRepository _repository,
                            IRedisCacheRepository cacheRepository)
        {
            this._docDbRepository = _repository;
            this._cacheRepository = cacheRepository;
        }

        #region DocumentDB

        public async Task<List<Topic>> GetTopics()
        {
            await _docDbRepository.InitAsync(_FORUM_COLLECTION_ID);

            string query = "select c.id, c.title, c.content, c.mediaDescription, c.mediaUrl, c.mediaType, c.userId, c.createdDate from c";

            var justTopics = await _docDbRepository.CreateDocumentQueryAsync<Topic>(query, new Microsoft.Azure.Documents.Client.FeedOptions() { EnableCrossPartitionQuery = true });

            return justTopics.ToList();
        }

        public async Task<PagedTopics> GetTopics(int size, string continuationToken)
        {
            await _docDbRepository.InitAsync(_FORUM_COLLECTION_ID);

            var dic = await _docDbRepository.CreateDocumentQueryAsync<Topic>(2, continuationToken);

            return new PagedTopics
            {
                Topics = dic.Keys.First().OrderByDescending(t => t.CreatedDate).ToList(),
                ContinuationToken = dic.Values.First()
            };
        }

        public async Task<Topic> GetTopic(string id)
        {
            await _docDbRepository.InitAsync(_FORUM_COLLECTION_ID);

            var topic = await _docDbRepository.GetItemAsync<Topic>(id);

            return topic;
        }

        public async Task AddTopicAsync(Topic topic)
        {
            await _docDbRepository.InitAsync(_FORUM_COLLECTION_ID);

            await _docDbRepository.CreateItemAsync<Topic>(topic);
        }

        public async Task<Topic> AddTopicReply(string id, Post reply)
        {
            var topic = await GetTopic(id);

            if (topic != null)
            {
                topic.Posts.Add(reply);
                await _docDbRepository.UpdateItemAsync<Topic>(id, topic);
            }

            return topic;
        }

        public async Task<IEnumerable<Product>> GetProducts(string filter)
        {
            return await GetAllProducts();
        }

        public async Task<Product> GetProductDetails(string productId)
        {
            await _docDbRepository.InitAsync(_PRODUCT_COLLECTION_ID);
            var product = await _docDbRepository.GetItemAsync<Product>(productId);
            return product;
        }


        #region Private methods
        private async Task<IEnumerable<Product>> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            await _docDbRepository.InitAsync(_PRODUCT_COLLECTION_ID);

            return await _docDbRepository.GetItemsAsync<Product>();
        }
        #endregion

        #endregion

        #region RedisCache
        /// <summary>
        /// Get cart item by cartid.
        /// </summary>
        /// <returns></returns>
        public async Task<CartDTO> GetCart(string cartId)
        {
            return await _cacheRepository.GetItemAsync<CartDTO>("cart-" + cartId);
        }

        public async Task RemoveCart(string key)
        {
            await _cacheRepository.RemoveAsync(key);
        }

        public async Task<CartDTO> AddProductToCart(string cardId, string productId)
        {
            CartDTO cart = null;
            var product = await GetProductDetails(productId);

            if (string.IsNullOrEmpty(cardId))
            {
                cardId = Guid.NewGuid().ToString();

                cart = new CartDTO()
                {
                    Id = cardId,
                    CreatedDate = DateTime.Now
                };

                cart.Items.Add(new CartItemsDTO()
                {
                    Id = productId,
                    Image = product.Image,
                    Title = product.Title,
                    Model = product.Model,
                    Description = product.Description,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            else
            {
                cart = await _cacheRepository.GetItemAsync<CartDTO>("cart-" + cardId);

                if (cart == null)
                {
                    cart = new CartDTO()
                    {
                        Id = cardId,
                        CreatedDate = DateTime.Now
                    };
                }
                else
                {
                    cart.UpdateDate = DateTime.Now;
                }

                var productCartItem = cart.Items.Where(i => i.Id == productId).FirstOrDefault();

                if (productCartItem != null)
                {
                    productCartItem.Quantity++;
                }
                else
                {
                    cart.Items.Add(new CartItemsDTO()
                    {
                        Id = productId,
                        Image = product.Image,
                        Title = product.Title,
                        Model = product.Model,
                        Description = product.Description,
                        Price = product.Price,
                        Quantity = 1
                    });
                }
            }

            await _cacheRepository.SetItemAsync("cart-" + cardId, cart);

            return cart;
        }

        public async Task<CartDTO> RemoveProductFromCart(string cardId, string productId)
        {
            CartDTO cart = null;

            cart = await _cacheRepository.GetItemAsync<CartDTO>("cart-" + cardId);

            if (cart == null)
            {
                cart = new CartDTO()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.Now
                };
            }
            else
            {
                cart.Items.RemoveAll(item => item.Id == productId);
                await _cacheRepository.SetItemAsync("cart-" + cardId, cart);

                cart.UpdateDate = DateTime.Now;
            }

            return cart;
        }

        #endregion

        #region Sharding
        /*
        public int? AddOrder(Order order)
        {
            int shardingKey = _dataRepository.TotalOrders() + 1;
            int? orderId = _dataRepository.AddOrder(order, shardingKey);

            return orderId;
        }
        */
        #endregion
    }

    // Service Interface
    public interface IStoreService
    {
        Task<List<Topic>> GetTopics();
        Task<PagedTopics> GetTopics(int size, string continuationToken);
        Task<Topic> GetTopic(string id);
        Task<Topic> AddTopicReply(string id, Post reply);
        Task AddTopicAsync(Topic topic);
        Task<IEnumerable<Product>> GetProducts(string filter);
        Task<Product> GetProductDetails(string productId);
        Task<CartDTO> GetCart(string cartId);
        Task<CartDTO> AddProductToCart(string cardId, string productId);
        Task<CartDTO> RemoveProductFromCart(string cardId, string productId);
        Task RemoveCart(string key);
        //int? AddOrder(Order order);
    }
}
