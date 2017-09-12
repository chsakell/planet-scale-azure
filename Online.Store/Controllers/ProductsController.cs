using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;
using Online.Store.ViewModels;
using Online.Store.Core.DTOs;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private IStoreService _storeService;

        public ProductsController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IEnumerable<ProductsViewModel>> Get(string SearchString)
        {
            var products = await _storeService.GetProducts(SearchString);

            var productsViewData = new List<ProductsViewModel>();
            foreach (var product in products)
            {
                var productModel = this.ProductDTOToProductViewModel(product);
                productsViewData.Add(productModel);
            }

            return productsViewData;
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Products
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #region Private methods 
        /// <summary>
        /// Convert ProductDTO to ProductViewModel.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns></returns>
        private ProductsViewModel ProductDTOToProductViewModel(ProductDTO product)
        {
            var componentsList = new List<ProductComponentViewModel>();
            if (product.Components != null)
            {
                foreach (var component in product.Components)
                {
                    var mediaList = new List<ProductMediaViewModel>();
                    foreach (var media in component.Medias)
                    {
                        mediaList.Add(new ProductMediaViewModel
                        {
                            ImagePath = media.Url,
                            ImageEnlargeHeight = media.Height,
                            ImageEnlargeWidth = media.Width
                        });
                    }

                    componentsList.Add(new ProductComponentViewModel
                    {
                        ProductMediaList = mediaList,
                        ProductComponentDescription = component.ComponentDetail,
                        ProductComponentHeading = component.ComponentType
                    });
                }
            }

            return new ProductsViewModel
            {
                ProductId = product.Id,
                ProductHeading = product.Title,
                ProductComponentsList = componentsList,
                Description = product.Description,
                ProductURL = product.Url,
                ProductPrice = product.Price
            };
        }
        #endregion
    }
}
