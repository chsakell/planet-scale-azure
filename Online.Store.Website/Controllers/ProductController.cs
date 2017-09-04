using Online.Store.Azure.Services;
using Online.Store.Core.DTOs;
using Online.Store.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Website.Controllers
{
    public class ProductController : Controller
    {
        /// <summary>
        /// The _service
        /// </summary>
        private ProductsAppService _service;
        private IStoreService _storeService;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        public ProductController(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IStoreService storeService)
        {
            _service = new ProductsAppService(
                configuration["DocumentDB:DatabaseId"], 
                configuration["DocumentDB:Endpoint"], 
                configuration["DocumentDB:Key"], 
                configuration["SQL:ElasticScaleDb"],
                configuration["SQL:ServerName"],
                configuration["SQL:DatabaseEdition"],
                configuration["SQL:ElasticDbUsername"],
                configuration["SQL:ElasticDbPassword"],
                configuration["SQL:IntegratedSecurity"]);

            _storeService = storeService;
            _userManager = userManager;
        }

        // GET: Product
        /// <summary>
        /// Indexes the specified user search input.
        /// </summary>
        /// <param name="userSearchInput">The user search input.</param>
        /// <returns></returns>
        public async Task<ActionResult> Index(SearchViewModel userSearchInput)
        {
            ViewBag.SelectedMenu = "Product";
            // get products from store
            //var products = await _service.GetProducts(userSearchInput.SearchString);
            var products = await _storeService.GetProducts(userSearchInput.SearchString);

            var productsViewData = new List<ProductsViewModel>();
            foreach (var product in products)
            {
                var productModel = this.ProductDTOToProductViewModel(product);
                productsViewData.Add(productModel);
            }
            return View(productsViewData);
        }

        /// <summary>
        /// Get product details
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> Details(string id)
        {
            var data = await _storeService.GetProductDetails(id);
            var model = new ProductsViewModel();
            if (data != null)
            {
                model.ProductHeading = data.Title;
                model.ProductPrice = data.Price;
                model.ProductId = data.Id;
                model.Description = data.Description;
                model.ProductComponentsList = new List<ProductComponentViewModel>();
                if (data.Components != null)
                {
                    var componentsList = new List<ProductComponentViewModel>();
                    foreach (var component in data.Components)
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

                    model.ProductComponentsList = componentsList;
                }
            }
            return View(model);
        }

        /// <summary>
        /// Shows the media.
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <returns></returns>
        public ActionResult ShowMedia(ShowMediaViewModel imagePath)
        {
            ViewBag.imageName = imagePath.ImageName;
            return View();
        }

        /// <summary>
        /// Adds to cart.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("/Product/AddToCart/{productId}")]
        public async Task<ActionResult> AddToCart(string productId)
        {
            var user = await _userManager.GetUserAsync(User);

            var userId = user.Id;

            var cart = await _storeService.GetCart(userId);

            if (cart !=null && cart.CartItems != null)
            {
                CartItemsDTO existingcartitem = null;

                cart.CartItems.ForEach((c) =>
                {
                    if (c.Products.Id == productId)
                    {
                        c.Qty = c.Qty + 1;
                        existingcartitem = c;
                    }
                });

                if (existingcartitem == null)
                {
                    cart.CartItems.Add(new CartItemsDTO
                    {
                        Products = await _storeService.GetProductDetails(productId),
                        Qty = 1,
                        Id = Guid.NewGuid().ToString()
                    });
                }
                await _storeService.UpdateToCart(cart);
            }
            else
            {
                cart = new CartDTO();
                cart.UserId = userId;
                cart.CreatedDate = DateTime.Now;
                cart.CartItems.Add(new CartItemsDTO
                {
                    Products = await _storeService.GetProductDetails(productId),
                    Qty = 1,
                    Id = userId
                });
                await _storeService.AddToCart(cart);
            }

            return Json(true);
        }

        /// <summary>
        /// Views the cart.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ViewCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var cart = new CartViewModel();
            if (!string.IsNullOrEmpty(userId))
            {
                var items = await _storeService.GetCart(userId);
                if(items!=null)
                {
                    cart.UserId = items.UserId;
                    cart.CreatedDate = items.CreatedDate;
                    cart.UpdateDate = items.UpdateDate;
                    if (items.CartItems!=null && items.CartItems.Any())
                    {
                        items.CartItems.ForEach((ci) =>
                        {
                            cart.CartItems.Add(new CartItemViewModel
                            {
                                Qty = ci.Qty,
                                Id = ci.Id,
                                Products = this.ProductDTOToProductViewModel(ci.Products),
                            });
                        });
                    }
                }
            }
            return View(cart);
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Route("/Product/RemoveItem/{id}/{productId}")]
        public async Task<ActionResult> RemoveItem(string id, string productId)
        {
            var cart = await _storeService.GetCart(id);
            if (cart == null)
                return Json(false);
            else
            {
                if (cart.CartItems != null)
                {
                    CartItemsDTO existingItem = null;
                    cart.CartItems.ForEach((c) =>
                    {
                        if (c.Products.Id == productId)
                        {
                            existingItem = c;
                        }
                    });
                    cart.CartItems.Remove(existingItem);
                }
                await _storeService.UpdateToCart(cart);
            }
            return Json(true);
        }

        /// <summary>
        /// Updates the qty.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="qty">The qty.</param>
        /// <returns></returns>
        [Route("/Product/UpdateQty/{id}/{productId}/{qty}")]
        public async Task<ActionResult> UpdateQty(string id, string productId, int qty)
        {
            var cart = await _storeService.GetCart(id);
            if (cart == null)
                return Json(false);
            else
            {
                if (cart.CartItems != null && cart.CartItems.Any() && qty >= 1)
                {
                    cart.CartItems.ForEach((c) =>
                    {
                        if (c.Products.Id == productId)
                        {
                            c.Qty = qty;
                        }
                    });
                    await _storeService.UpdateToCart(cart);
                }
            }
            return Json(true);
        }

        /// <summary>
        /// Checks the out.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> CheckOut()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var products = await _service.GetCart(userId);
            if (products == null)
                ViewBag.Error = true;
            else
            {
                try
                {
                    products.UserName = user !=null ? user.FullName : string.Empty;
                    _service.CheckOut(products);
                    await _service.RemoveFromCart(products);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = true;
                }
            }
            return View(products);
        }

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
                ProductURL=product.Url,
                ProductPrice = product.Price
            };
        }


    }
}