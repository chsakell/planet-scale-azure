using Online.Store.Azure.Services;
using Online.Store.Core.DTOs;
using Online.Store.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Online.Store.DocumentDB;

namespace Online.Store.Website.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// The _service
        /// </summary>
        private IStoreService _service;
        private SearchAppServices _searchService;
        private IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController(IConfiguration configuration, IStoreService storeService)
        {
            _service = storeService;
            _searchService = new SearchAppServices(configuration["SearchService:Name"], configuration["SearchService:ApiKey"]);
            _configuration = configuration;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            MainViewModel homeViewModelData = new MainViewModel();
            homeViewModelData.News = this.FillNewsViewModel();
            homeViewModelData.Community = this.FillCommunityViewModel(await _service.GetTopCommunityPost());
            homeViewModelData.CarousalItems = this.FillCarousalViewModel();
            return View(homeViewModelData);
        }

        /// <summary>
        /// Fills the carousal view model with static data.
        /// </summary>
        /// <returns></returns>
        private List<CarouselItemViewModel> FillCarousalViewModel()
        {
            var carouselItemViewData = new List<CarouselItemViewModel>();
            CarouselItemViewModel carouselItem1 = new CarouselItemViewModel();

            carouselItem1.ImageUrl = "sb_4.jpg";
            carouselItem1.AlternateImageUrl = "banner-3.jpg";
            carouselItem1.ItemHeading = "Pro Skate Board Motion 9000";
            carouselItem1.RedirectUrl = "/product";
            carouselItem1.ItemDescription = " Do all your cool stuff on Contoso board Motion 9000.";
            carouselItem1.DivisionClassName = "item active";
            carouselItemViewData.Add(carouselItem1);

            CarouselItemViewModel carouselItem2 = new CarouselItemViewModel();

            carouselItem2.ImageUrl = "sb_2.jpg";
            carouselItem2.AlternateImageUrl = "banner-2.jpg";
            carouselItem2.ItemHeading = "Carve";
            carouselItem2.RedirectUrl = "/product";
            carouselItem2.ItemDescription = "Carve your new style on Motion 9000.";
            carouselItem2.DivisionClassName = "item";
            carouselItemViewData.Add(carouselItem2);

            CarouselItemViewModel carouselItem3 = new CarouselItemViewModel();

            carouselItem3.ImageUrl = "sb_1.jpg";
            carouselItem3.AlternateImageUrl = "banner-1.jpg";
            carouselItem3.ItemHeading = "Varial Kick Flip ";
            carouselItem3.RedirectUrl = "/product";
            carouselItem3.ItemDescription = " Do a Kick Flip or some serious Technical Skating on Motion Board 9000";
            carouselItem3.DivisionClassName = "item";
            carouselItemViewData.Add(carouselItem3);
            return carouselItemViewData;

        }

        /// <summary>
        /// Fills the news view model with static data.
        /// </summary>
        /// <returns></returns>
        private List<NewsViewModel> FillNewsViewModel()
        {
            var newsViewData = new List<NewsViewModel>();
            NewsViewModel newsItem = new NewsViewModel();
            newsItem.NewsDate = "13th May 2014";
            newsItem.NewsLink = "http://skateboarding.transworld.net/1000197850/videos/aidan-campbell-knows/";
            newsItem.NewsText = "Aidan Campbell comes through with the newest clip for the Th...";
            newsViewData.Add(newsItem);
            NewsViewModel newsItem1 = new NewsViewModel();
            newsItem1.NewsDate = "5th July 2012";
            newsItem1.NewsLink = "/news/feed7";
            newsItem1.NewsText = "Some New Yorkers skateboard to work as a stay-fit strategy f...";
            newsViewData.Add(newsItem1);
            return newsViewData;
        }

        /// <summary>
        /// Fills the community view model.
        /// </summary>
        /// <param name="communityData">The community data.</param>
        /// <returns></returns>
        private List<CommunityViewModel> FillCommunityViewModel(List<CommunityDTO> communityData)
        {
            var communityViewData = new List<CommunityViewModel>();
            if (communityData == null)
                return communityViewData;
            foreach (var item in communityData)
            {
                var model = new CommunityViewModel()
                {
                    id = item.PostId,
                    UserImageName = "",
                    No_of_Posts = 1,
                    PostContent = item.Content,
                    PostDate = item.CreatedDate,
                    PostTitle = item.Title,
                    MediaDescription = item.MediaDescription,
                    ResponseCount = item.Responses != null ? item.Responses.Count : 0
                };
                communityViewData.Add(model);
            }
            return communityViewData;
        }

        public ActionResult SearchResult(string searchString)
        {
            ViewBag.SearchText = searchString;
            var resultProduct = _searchService.SearchProduct(searchString, null);
            var resultPost = _searchService.SearchPost(searchString, null);
            var model = new SearchResultDTO();
            if (resultProduct != null && resultProduct.value != null)
            {
                var products = new List<ProductDTO>();
                foreach (var item in resultProduct.value)
                {
                    products.Add(new ProductDTO()
                    {
                        Id = item.id,
                        Title = item.title,
                        Description = item.description,
                        Url = item.url
                    });
                }
                model.Product = products;
            }
            if (resultPost != null && resultPost.value != null)
            {
                var posts = new List<CommunityDTO>();
                foreach (var item in resultPost.value)
                {
                    posts.Add(new CommunityDTO()
                    {
                        PostId = item.postId,
                        Title = item.title,
                        Content = item.content,
                    });
                }
                model.Posts = posts;
            }
            return View(model);
        }

        /// <summary>
        /// Suggests the specified term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suggest(string term)
        {
            var options = new List<string>();
            if (term.Length >= 3)
            {
                var result = _searchService.Suggest(term);

                foreach (var option in result.value)
                {
                    options.Add((string)option["@search.text"]);
                }
            }

            return Json(options);
        }

    }
}