using Online.Store.ElasticScale;
using Online.Store.Core.DTOs;
using Online.Store.DocumentDB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class ProductsAppService
    {
        private DocumentRepository _repository;
        private readonly string _elasticScaleDb;
        private readonly string _serverName;
        private readonly string _databaseEdition;
        private readonly string _elasticDbUsername;
        private readonly string _elasticDbPassword;
        private readonly string _integratedSecurity;

        public ProductsAppService(
            string databaseId, 
            string documentDBEndpoint, 
            string documentDBKey, 
            string elasticScaleDb,
            string serverName,
            string databaseEdition,
            string elasticDbUsername,
            string elasticDbPassword,
            string integratedSecurity)
        {
            _elasticScaleDb = elasticScaleDb;
            _serverName = serverName;
            _databaseEdition = databaseEdition;
            _elasticDbUsername = elasticDbUsername;
            _elasticDbPassword = elasticDbPassword;
            _integratedSecurity = integratedSecurity;

            _repository = new DocumentRepository(databaseId, documentDBEndpoint, documentDBKey);
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public async Task<List<ProductDTO>> GetProducts(string filter)
        {
            return await _repository.GetProducts();
        }

        /// <summary>
        /// Get Product details by product id.
        /// </summary>
        /// <returns></returns>
        public async Task<ProductDTO> GetProductDetails(string productId)
        {
            return await _repository.GetProductDetails(productId);
        }

        /// <summary>
        /// Get cart item by cartid.
        /// </summary>
        /// <returns></returns>
        public async Task<CartDTO> GetCart(string cartId)
        {
            return await _repository.GetCartItems(cartId);
        }

        /// <summary>
        /// Add to Cart.
        /// </summary>
        /// <returns></returns>
        public async Task AddToCart(CartDTO Item)
        {
            await _repository.AddToCart(Item);
        }

        /// <summary>
        /// Update cart items.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateToCart(CartDTO Item)
        {
            await _repository.UpdateToCart(Item);
        }

        /// <summary>
        /// Removes from cart.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveFromCart(CartDTO Item)
        {
            await _repository.RemoveFromCart(Item);
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<ProductDTO> GetDetails(string id)
        {
            return await _repository.GetProductDetails(id);
        }

        public void CheckOut(CartDTO cart)
        {
            var azureElasticService = new AzureElasticScaleServices(
                _elasticScaleDb,
                _serverName,
                _databaseEdition,
                _elasticDbUsername,
                _elasticDbPassword,
                _integratedSecurity);
            azureElasticService.InsertData(cart);
        }
    }
}
