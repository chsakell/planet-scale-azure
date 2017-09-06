using Microsoft.Azure.Documents;
using Online.Store.Core.DTOs;
using Online.Store.DocumentDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class StoreService : IStoreService
    {
        #region "Private Constant Members"

        private const string _PRODUCT_COLLECTION_ID = "Products";
        private const string _CART_COLLECTION_ID = "Cart";
        private const string _COMMUNITY_COLLECTION_ID = "Community";
        private int PAGE_SIZE = 5;
        #endregion

        /// <summary>
        /// The DocumentDB _repository
        /// </summary>
        private IDocumentDBRepository<DocumentDBStoreRepository> _repository;

        public StoreService(IDocumentDBRepository<DocumentDBStoreRepository> _repository)
        {
            this._repository = _repository;
        }

        public async Task<List<CommunityDTO>> GetTopCommunityPost()
        {
            var data = new List<CommunityDTO>();
            await _repository.InitAsync(_COMMUNITY_COLLECTION_ID);

            var communities = await this._repository.GetItemsAsync<CommunityDTO>();

            data = communities.OrderByDescending(x => x.CreatedDate).ToList();
            if (data != null && data.Count > 5)
            {
                return data.Take(5).ToList();
            }
            return data ?? new List<CommunityDTO>();
        }

        public async Task<List<ProductDTO>> GetProducts(string filter)
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
        /// Get cart item by cartid.
        /// </summary>
        /// <returns></returns>
        public async Task<CartDTO> GetCart(string cartId)
        {
            CartDTO cartItem = new CartDTO();
            await _repository.InitAsync(_CART_COLLECTION_ID);

            cartItem = await _repository.GetItemAsync<CartDTO>(cartId);
            return cartItem;
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

            await _repository.UpdateItemAsync<CartDTO>(item.UserId, item);
        }

        /// <summary>
        /// Removes from cart.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveFromCart(CartDTO item)
        {
            await _repository.InitAsync(_CART_COLLECTION_ID);

            await _repository.DeleteItemAsync(item.UserId);
        }

        public async Task<PagedCommunityDto> GetAllCommunity(string id, int? pageId)
        {
            await _repository.InitAsync(_COMMUNITY_COLLECTION_ID);

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
            await _repository.InitAsync(_COMMUNITY_COLLECTION_ID);
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
            await _repository.InitAsync(_COMMUNITY_COLLECTION_ID);

            var datatoAdd = new CommunityDTO()
            {
                PostId = Guid.NewGuid().ToString(),
                Content = post.Content,
                ContentType = post.ContentType,
                CreatedDate = DateTime.Now,
                MediaDescription = post.MediaDescription,
                Title = post.Title,
                UserId = post.UserId,
                ContentUrl = post.ContentUrl
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
            await _repository.InitAsync(_COMMUNITY_COLLECTION_ID);

            var datatoAdd = new CommunityDTO()
            {
                PostId = Guid.NewGuid().ToString(),
                Content = post.Content,
                ContentType = post.ContentType,
                CreatedDate = DateTime.Now,
                MediaDescription = post.MediaDescription,
                Title = post.Title,
                UserId = post.UserId,
                ContentUrl = post.ContentUrl
            };

            if (!string.IsNullOrEmpty(post.PostId))
            {
                var entity = await _repository.GetItemAsync<CommunityDTO>(post.PostId);
                if (entity != null)
                {
                    if (entity.Responses == null)
                        entity.Responses = new List<CommunityDTO>();

                    entity.Responses.Add(datatoAdd);

                    await _repository.UpdateItemAsync<CommunityDTO>(post.PostId, entity);
                }
            }

            return datatoAdd;
        }

        #region Private methods
        private async Task<List<ProductDTO>> GetAllProducts()
        {
            List<ProductDTO> products = new List<ProductDTO>();
            await _repository.InitAsync(_PRODUCT_COLLECTION_ID);

            dynamic allProducts = _repository.CreateDocumentQuery<Document>().AsEnumerable();
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
        #endregion
    }

    public interface IStoreService
    {
        Task<List<CommunityDTO>> GetTopCommunityPost();
        Task<List<ProductDTO>> GetProducts(string filter);
        Task<ProductDTO> GetProductDetails(string productId);
        Task<CartDTO> GetCart(string cartId);
        Task AddToCart(CartDTO Item);
        Task UpdateToCart(CartDTO item);
        Task RemoveFromCart(CartDTO Item);
        Task<PagedCommunityDto> GetAllCommunity(string id, int? pageId);
        Task<CommunityResponseDto> GetCommunityDetails(string id, string filterId, int? pageId);
        Task<CommunityDTO> AddPost(PostDTO post);
        Task<CommunityDTO> AddPostResponse(PostDTO post);
    }
}
