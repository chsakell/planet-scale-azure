using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Azure.Services;
using Online.Store.ViewModels;
using Online.Store.Core.DTOs;
using AutoMapper;

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
        public async Task<IEnumerable<ProductViewModel>> Get()
        {
            var products = await _storeService.GetProducts(null);

            var productsViewData = new List<ProductViewModel>();

            productsViewData = Mapper.Map<List<ProductViewModel>>(products);

            return productsViewData;
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ProductViewModel> Get(string id)
        {
            var productDTO = await _storeService.GetProductDetails(id);

            var product = Mapper.Map<ProductViewModel>(productDTO);

            return product;
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
        private ProductViewModel ProductDTOToProductViewModel(ProductDTO product)
        {
            var componentsList = new List<ProductComponentViewModel>();
            if (product.Components != null)
            {
                foreach (var component in product.Components)
                {
                    var mediaList = new List<ProductMediaViewModel>();
                    if (component.Medias != null)
                    {
                        foreach (var media in component.Medias)
                        {
                            mediaList.Add(new ProductMediaViewModel
                            {
                                Url = media.Url,
                                Type = media.Type
                            });
                        }
                    }

                    componentsList.Add(new ProductComponentViewModel
                    {
                        Medias = mediaList,
                        ComponentDetail = component.ComponentDetail,
                        ComponentType = component.ComponentType,
                        ComponentTitle = component.ComponentTitle
                    });
                }
            }

            return new ProductViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Components = componentsList,
                Description = product.Description,
                Image = product.Image,
                Price = product.Price,
                Model = product.Model,
                SKU = product.SKU
            };
        }
        #endregion
    }
}
