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
        private const string _CART_COLLECTION_ID = "Cart";
        private const string _FORUM_COLLECTION_ID = "Forum";
        private int PAGE_SIZE = 5;
        #endregion

        /// <summary>
        /// The DocumentDB _repository
        /// </summary>
        private IDocumentDBRepository<DocumentDBStoreRepository> _repository;

        private IRedisCacheRepository _cacheRepository;

        IShardingRepository _dataRepository;

        public StoreService(IDocumentDBRepository<DocumentDBStoreRepository> _repository,
                            IRedisCacheRepository cacheRepository,
                            IShardingRepository dataRepository)
        {
            this._repository = _repository;
            this._cacheRepository = cacheRepository;
            this._dataRepository = dataRepository;
        }

        #region DocumentDB
        public async Task<List<CommunityDTO>> GetTopCommunityPost()
        {
            var data = new List<CommunityDTO>();
            await _repository.InitAsync(_FORUM_COLLECTION_ID);

            var communities = await this._repository.GetItemsAsync<CommunityDTO>();

            data = communities.OrderByDescending(x => x.CreatedDate).ToList();
            if (data != null && data.Count > 5)
            {
                return data.Take(5).ToList();
            }
            return data ?? new List<CommunityDTO>();
        }

        public async Task<List<TopicDTO>> GetTopics()
        {
            await _repository.InitAsync(_FORUM_COLLECTION_ID);

            string query = "select c.id, c.title, c.content, c.mediaDescription, c.mediaUrl, c.mediaType, c.userId, c.createdDate from c";

            var justTopics = await _repository.CreateDocumentQueryAsync<TopicDTO>(query, new Microsoft.Azure.Documents.Client.FeedOptions() { EnableCrossPartitionQuery = true });

            // var topics = await _repository.GetItemsAsync<TopicDTO>();

            return justTopics.ToList(); // topics.ToList();
        }

        public async Task<TopicDTO> GetTopic(string id)
        {
            await _repository.InitAsync(_FORUM_COLLECTION_ID);

            var topic = await _repository.GetItemAsync<TopicDTO>(id);

            return topic;
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts(string filter)
        {
            return await GetAllProducts();
        }

        public async Task<ProductDTO> GetProductDetails(string productId)
        {
            await _repository.InitAsync(_PRODUCT_COLLECTION_ID);
            var product = await _repository.GetItemAsync<ProductDTO>(productId);
            return product;
        }

        /// <summary>
        /// Add to Cart.
        /// </summary>
        /// <returns></returns>
        public async Task AddToCart(CartDTO item)
        {
            await _repository.InitAsync(_CART_COLLECTION_ID);
            await _repository.CreateItemAsync<CartDTO>(item);
        }

        /// <summary>
        /// Update cart items.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateToCart(CartDTO item)
        {
            await _repository.InitAsync(_CART_COLLECTION_ID);

            await _repository.UpdateItemAsync<CartDTO>(item.Id, item);
        }

        /// <summary>
        /// Removes from cart.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveFromCart(CartDTO item)
        {
            await _repository.InitAsync(_CART_COLLECTION_ID);

            await _repository.DeleteItemAsync(item.Id);
        }

        public async Task<PagedCommunityDto> GetAllCommunity(string id, int? pageId)
        {
            await _repository.InitAsync(_FORUM_COLLECTION_ID);

            int _pageSize = PAGE_SIZE; // todo : configuration
            int pageSize = _pageSize;
            var community = new PagedCommunityDto();
            var data = new List<CommunityDTO>();

            var communities = await this._repository.GetItemsAsync<CommunityDTO>();
            data = communities.OrderByDescending(x => x.CreatedDate).ToList();

            //Filter basesed on request.
            if (id != null && id != "all")
            {
                var result = data.FindAll(x => !string.IsNullOrEmpty(x.ContentType) && x.ContentType.Contains(id));
                data = result.ToList();
            }

            //Get total count and total page count.
            var totalCount = data.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            //Implementing pagination.
            var pages = data.Skip((pageId ?? 0) * pageSize).Take(pageSize).ToList();
            community.Community = pages.ToList();
            community.TotalPages = totalPages;
            community.SelectedPage = pageId != null ? pageId.Value : 0;

            return community;
        }

        /// <summary>
        /// Gets the details of selected post.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="filterId">The filter identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        public async Task<CommunityResponseDto> GetCommunityDetails(string id, string filterId, int? pageId)
        {
            await _repository.InitAsync(_FORUM_COLLECTION_ID);
            int pageSize = PAGE_SIZE;
            var communityResult = new CommunityResponseDto();

            var entity = await _repository.GetItemAsync<CommunityDTO>(id);

            if (entity != null)
            {
                var communityResponses = new List<CommunityDTO>();
                if (entity.Responses != null && entity.Responses.Any())
                {
                    communityResponses = entity.Responses.OrderByDescending(x => x.CreatedDate).ToList();
                    //Filter basesed on request.
                    if (filterId != null && filterId != "all")
                    {
                        var result = communityResponses.FindAll(x => !string.IsNullOrEmpty(x.ContentType) && x.ContentType.Contains(filterId));
                        communityResponses = result.ToList();
                    }

                    //Get total count and total page count.
                    var totalCount = communityResponses.Count();
                    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                    //Implementing pagination.
                    var pages = communityResponses.Skip((pageId ?? 0) * pageSize).Take(pageSize).ToList();
                    entity.Responses = pages.ToList();

                    communityResult.Community = entity;
                    communityResult.TotalPages = totalPages;
                    communityResult.SelectedPage = pageId != null ? pageId.Value : 0;
                }
                communityResult.Community = entity;
            }
            return communityResult;
        }

        /// <summary>
        /// Add New Post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns></returns>
        public async Task<CommunityDTO> AddPost(PostDTO post)
        {
            await _repository.InitAsync(_FORUM_COLLECTION_ID);

            var datatoAdd = new CommunityDTO()
            {
                PostId = Guid.NewGuid().ToString(),
                Content = post.Content,
                ContentType = post.MediaType,
                CreatedDate = DateTime.Now,
                MediaDescription = post.MediaDescription,
                Title = post.Title,
                UserId = post.UserId,
                ContentUrl = post.MediaUrl
            };

            Document created = await _repository.CreateItemAsync<CommunityDTO>(datatoAdd);

            return datatoAdd;
        }

        /// <summary>
        /// Posts the replay.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns></returns>
        public async Task<CommunityDTO> AddPostResponse(PostDTO post)
        {
            await _repository.InitAsync(_FORUM_COLLECTION_ID);

            var datatoAdd = new CommunityDTO()
            {
                PostId = Guid.NewGuid().ToString(),
                Content = post.Content,
                ContentType = post.MediaType,
                CreatedDate = DateTime.Now,
                MediaDescription = post.MediaDescription,
                Title = post.Title,
                UserId = post.UserId,
                ContentUrl = post.MediaUrl
            };

            if (!string.IsNullOrEmpty(post.Id))
            {
                var entity = await _repository.GetItemAsync<CommunityDTO>(post.Id);
                if (entity != null)
                {
                    if (entity.Responses == null)
                        entity.Responses = new List<CommunityDTO>();

                    entity.Responses.Add(datatoAdd);

                    await _repository.UpdateItemAsync<CommunityDTO>(post.Id, entity);
                }
            }

            return datatoAdd;
        }

        #region Private methods
        private async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            List<ProductDTO> products = new List<ProductDTO>();
            await _repository.InitAsync(_PRODUCT_COLLECTION_ID);

            return await _repository.GetItemsAsync<ProductDTO>();

            #region Obsolete
            /*

            dynamic allProducts = _repository.CreateDocumentQuery<Document>().AsEnumerable();
            foreach (var product in allProducts)
            {
                var components = new List<ProductComponentDTO>();
                foreach (var component in product.components)
                {
                    var componentMedias = new List<ProductMediaDTO>();
                    if (component.medias != null)
                    {
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
                    }

                    components.Add(new ProductComponentDTO
                    {
                        Id = component.id,
                        ComponentType = component.componenttype,
                        ComponentTitle = component.title,
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

            */
            #endregion

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
            return await _cacheRepository.GetItemAsync<CartDTO>(cartId);
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
                cart = await _cacheRepository.GetItemAsync<CartDTO>(cardId);

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

            await _cacheRepository.SetItemAsync(cardId, cart);

            return cart;
        }

        public async Task<CartDTO> RemoveProductFromCart(string cardId, string productId)
        {
            CartDTO cart = null;

            cart = await _cacheRepository.GetItemAsync<CartDTO>(cardId);

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
                await _cacheRepository.SetItemAsync(cardId, cart);

                cart.UpdateDate = DateTime.Now;
            }

            return cart;
        }

        #endregion

        #region Sharding

        public int? AddOrder(Order order)
        {
            int shardingKey = _dataRepository.TotalOrders() + 1;
            int? orderId = _dataRepository.AddOrder(order, shardingKey);

            return orderId;
        }

        #endregion
    }

    public interface IStoreService
    {
        Task<List<CommunityDTO>> GetTopCommunityPost();
        Task<List<TopicDTO>> GetTopics();
        Task<TopicDTO> GetTopic(string id);
        Task<IEnumerable<ProductDTO>> GetProducts(string filter);
        Task<ProductDTO> GetProductDetails(string productId);
        Task<CartDTO> GetCart(string cartId);
        Task AddToCart(CartDTO Item);
        Task<CartDTO> AddProductToCart(string cardId, string productId);
        Task<CartDTO> RemoveProductFromCart(string cardId, string productId);
        Task UpdateToCart(CartDTO item);
        Task RemoveFromCart(CartDTO Item);
        Task RemoveCart(string key);
        Task<PagedCommunityDto> GetAllCommunity(string id, int? pageId);
        Task<CommunityResponseDto> GetCommunityDetails(string id, string filterId, int? pageId);
        Task<CommunityDTO> AddPost(PostDTO post);
        Task<CommunityDTO> AddPostResponse(PostDTO post);
        int? AddOrder(Order order);
    }
}
