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
    }
}
