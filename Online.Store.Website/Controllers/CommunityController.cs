using Online.Store.AppServices;
using Online.Store.Core.DTOs;
using Online.Store.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Website.Controllers
{
    public class CommunityController : Controller
    {
        /// <summary>
        /// The _service
        /// </summary>
        private CommunityAppService _service = null;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityController"/> class.
        /// </summary>
        public CommunityController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _service = new CommunityAppService(configuration["DocumentDB:DatabaseId"], configuration["DocumentDB:Endpoint"], configuration["DocumentDB:Key"], configuration["App:PageSize"]);
            _configuration = configuration;
            _userManager = userManager;
        }

        // GET: Community
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index(string id, int? pageId)
        {
            ViewBag.SelectedMenu = "Community";
            ViewBag.Selected = id;
            var communityResult = new PagedCommunityViewModel();
            communityResult = await this.GetCommunityDetails(id, pageId);
            return View(communityResult);
        }

        /// <summary>
        /// Gets the community.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetCommunity(string id, int? pageId)
        {
            var communityResult = new PagedCommunityViewModel();
            communityResult = await this.GetCommunityDetails(id, pageId);
            return PartialView("_CommunityPostView", communityResult);
        }

        /// <summary>
        /// Posts .
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult NewDiscussion()
        {
            ViewBag.SelectedMenu = "Community";
            return View();
        }

        /// <summary>
        /// Add new Post.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="mediaFile">The media file.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> NewDiscussion(PostDTO post, IFormFile mediaFile)
        {
            try
            {
                if (mediaFile != null)
                {
                    var azureServices = new AzureServices(
                        _configuration["MediaServices:AccountKey"],
                        _configuration["MediaServices:AccountName"],
                        _configuration["Storage:AccountName"],
                        _configuration["Storage:AccountKey"]);
                    var mediaResult = new MediaDetailsDTO();
                    using (var filestream = mediaFile.OpenReadStream())
                    {
                        mediaResult = azureServices.UploadMedia(filestream, mediaFile.FileName, mediaFile.ContentType);
                    }
                    if (!mediaResult.Status)
                    {
                        return Json(new MessageDTO { Status = false, Message = "Error. Please try later." });
                    }
                    post.ContentType = mediaResult.MediaType;
                    post.ContentUrl = mediaResult.MediaUrl;
                }

                var user = await _userManager.GetUserAsync(User);

                post.UserId = user.Id;

                var data = await _service.Post(post);

                var model = new CommunityViewModel()
                {
                    id = data.PostId,
                    UserImageName = user != null ? user.ImageUrl : string.Empty,
                    No_of_Posts = data.Responses != null ? data.Responses.Count : 0,
                    PostContent = data.Content,
                    PostDate = data.CreatedDate,
                    PostTitle = data.Title,
                    UserName = user != null ? user.FullName : string.Empty,
                    ContentUrl = data.ContentUrl,
                    ContentType = data.ContentType
                };
                return Json(new MessageDTO { Status = true, Message = "Data updated successfully." });
            }
            catch (Exception)
            {
                return Json(new MessageDTO { Status = false, Message = "Error. Please try again.." });
            }
        }

        /// <summary>
        /// Reponses for specified post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <param name="mediaFile">The media file.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Reponse(PostDTO post, IFormFile mediaFile)
        {
            var user = await _userManager.GetUserAsync(User);

            if (mediaFile != null)
            {
                var azureServices = new AzureServices(
                        _configuration["MediaServices:AccountKey"],
                        _configuration["MediaServices:AccountName"],
                        _configuration["Storage:AccountName"],
                        _configuration["Storage:AccountKey"]);
                var mediaResult = new MediaDetailsDTO();
                using (var filestream = mediaFile.OpenReadStream())
                {
                    mediaResult = azureServices.UploadMedia(filestream, mediaFile.FileName, mediaFile.ContentType);
                }
                if (!mediaResult.Status)
                {
                    return Json(new MessageDTO { Status = false, Message = "Error. Please try later." });
                }
                post.ContentType = mediaResult.MediaType;
                post.ContentUrl = mediaResult.MediaUrl;
            }

            post.UserId = user.Id;
            var data = await _service.PostReplay(post);
            var result = new CommunityResponseViewModel();
            var model = new CommunityViewModel()
            {
                id = data.PostId,
                UserImageName = user != null ? user.ImageUrl : string.Empty,
                No_of_Posts = data.Responses != null ? data.Responses.Count : 0,
                PostContent = data.Content,
                PostDate = data.CreatedDate,
                PostTitle = data.Title,
                UserName = user != null ? user.FullName : string.Empty,
                ContentUrl = data.ContentUrl,
                ContentType = data.ContentType
            };
            result.Community = model;
            return PartialView("_ResponseView", result);
        }

        /// <summary>
        /// Views the post.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="filterId">The filter identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> View(string id, string filterId, int? pageId)
        {
            ///TODO : Remove
            ViewBag.SelectedMenu = "Community";
            ViewBag.Selected = filterId;

            var communityResponse = new CommunityResponseViewModel();
            communityResponse = await this.GetComunityResponseDetails(id, filterId, pageId);
            return View(communityResponse);
        }

        /// <summary>
        /// Views the comunity response.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="filterId">The filter identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> ViewComunityResponse(string id, string filterId, int? pageId)
        {
            var communityResponse = new CommunityResponseViewModel();
            communityResponse = await this.GetComunityResponseDetails(id, filterId, pageId);
            return PartialView("_ResponseView", communityResponse);
        }

        /// <summary>
        /// Shows the community media.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> ShowCommunityMedia(string id)
        {
            var item = await _service.GetCommunityDetails(id);
            var user = await _userManager.FindByIdAsync(item.UserId);

            var model = new CommunityViewModel()
            {
                id = item.PostId,
                UserImageName = user != null ? user.ImageUrl : string.Empty,
                No_of_Posts = item.Responses != null ? item.Responses.Count : 0,
                PostContent = item.Content,
                PostDate = item.CreatedDate,
                PostTitle = item.Title,
                UserName = user != null ? user.FullName : string.Empty,
                ContentUrl = item.ContentUrl,
                ContentType = item.ContentType,
                MediaDescription = item.MediaDescription
            };

            return View(model);
        }

        /// <summary>
        /// Shows the community response.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <returns></returns>
        public async Task<ActionResult> ShowCommunityResponse(string id, string responseId)
        {
            var item = await _service.GetCommunityDetails(id);
            var model = new CommunityViewModel();
            if (item.Responses != null && item.Responses.Any())
            {
                var result = item.Responses.FirstOrDefault(x => x.PostId == responseId);
                if (result != null)
                {
                    model.id = result.PostId;
                    model.ContentUrl = result.ContentUrl;
                    model.ContentType = result.ContentType;
                    model.MediaDescription = result.MediaDescription;
                }
            }
            return View("ShowCommunityMedia", model);
        }

        #region Private Methods

        /// <summary>
        /// Gets the community details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        private async Task<PagedCommunityViewModel> GetCommunityDetails(string id, int? pageId)
        {
            var communityResult = new PagedCommunityViewModel();
            var community = await _service.GetAllCommunity(id, pageId);

            if (community != null)
            {
                communityResult.TotalPages = community.TotalPages;
                communityResult.SelectedPage = community.SelectedPage;

                var communitydata = community.Community;
                var communityModel = new List<CommunityViewModel>();
                if (communitydata != null && communitydata.Any())
                {
                    foreach (var item in communitydata)
                    {
                        var user = await _userManager.FindByIdAsync(item.UserId);
                        var model = new CommunityViewModel()
                        {
                            id = item.PostId,
                            UserImageName = "",
                            No_of_Posts = 1,
                            PostContent = item.Content,
                            PostDate = item.CreatedDate,
                            PostTitle = item.Title,
                            UserName = user != null ? user.FullName : string.Empty,
                            MediaDescription = item.MediaDescription
                        };
                        communityModel.Add(model);
                    }
                }
                communityResult.Community = communityModel;
            }
            return communityResult;
        }

        /// <summary>
        /// Gets the comunity response details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="filterId">The filter identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        private async Task<CommunityResponseViewModel> GetComunityResponseDetails(string id, string filterId, int? pageId)
        {
            var item = await _service.GetDetails(id, filterId, pageId);
            var user = await _userManager.FindByIdAsync(item.Community.UserId);

            var communityResponse = new CommunityResponseViewModel();
            if (item != null)
            {
                communityResponse.TotalPages = item.TotalPages;
                communityResponse.SelectedPage = item.SelectedPage;

                var model = new CommunityViewModel()
                {
                    id = item.Community.PostId,
                    UserImageName = user != null ? user.ImageUrl : string.Empty,
                    No_of_Posts = item.Community.Responses != null ? item.Community.Responses.Count : 0,
                    PostContent = item.Community.Content,
                    PostDate = item.Community.CreatedDate,
                    PostTitle = item.Community.Title,
                    UserName = user != null ? user.FullName : string.Empty,
                    ContentUrl = item.Community.ContentUrl,
                    ContentType = item.Community.ContentType,
                    MediaDescription = item.Community.MediaDescription
                };

                if (item.Community.Responses != null && item.Community.Responses.Any())
                {
                    foreach (var response in item.Community.Responses)
                    {
                        user = await _userManager.FindByIdAsync(response.UserId);
                        var replay = new CommunityViewModel()
                        {
                            id = response.PostId,
                            UserImageName = user != null ? user.ImageUrl : string.Empty,
                            No_of_Posts = response.Responses != null ? item.Community.Responses.Count : 0,
                            PostContent = response.Content,
                            PostDate = response.CreatedDate,
                            PostTitle = response.Title,
                            UserName = user != null ? user.FullName : string.Empty,
                            ContentUrl = response.ContentUrl,
                            ContentType = response.ContentType,
                            MediaDescription = response.MediaDescription,
                            ParentId = item.Community.PostId
                        };
                        model.Response.Add(replay);
                    }
                }
                communityResponse.Community = model;
            }
            return communityResponse;
        }

        #endregion Private Methode
    }
}