using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Core.DTOs;
using Online.Store.Azure.Services;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Forum")]
    public class ForumController : Controller
    {
        private IStoreService _storeService;

        public ForumController(IStoreService storeService)
        {
            this._storeService = storeService;
        }

        // GET: api/Forum
        [HttpGet]
        [Route("topics/", Name = "GetTopics")]
        public async  Task<IEnumerable<TopicDTO>> GetTopics()
        {
            var topics = await _storeService.GetTopics();

            return topics;
        }

        // GET: api/Forum/5
        [HttpGet("topics/{id}", Name = "GetTopic")]
        public async Task<TopicDTO> GetTopic(string id)
        {
            var topic = await _storeService.GetTopic(id);

            return topic;
        }

        // POST: api/Forum
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> AddPost(PostDTO post, IFormFile mediaFile)
        {
            //var user = await _userManager.GetUserAsync(User);

            //if (mediaFile != null)
            //{
            //    var azureServices = new AzureServices(
            //            _configuration["MediaServices:AccountKey"],
            //            _configuration["MediaServices:AccountName"],
            //            _configuration["Storage:AccountName"],
            //            _configuration["Storage:AccountKey"]);
            //    var mediaResult = new MediaDetailsDTO();
            //    using (var filestream = mediaFile.OpenReadStream())
            //    {
            //        mediaResult = azureServices.UploadMedia(filestream, mediaFile.FileName, mediaFile.ContentType);
            //    }
            //    if (!mediaResult.Status)
            //    {
            //        return Json(new MessageDTO { Status = false, Message = "Error. Please try later." });
            //    }
            //    post.MediaType = mediaResult.MediaType;
            //    post.MediaUrl = mediaResult.MediaUrl;
            //}

            post.UserId = "chsakell";
            var data = await _storeService.AddPostResponse(post);
            //var result = new CommunityResponseViewModel();
            //var model = new CommunityViewModel()
            //{
            //    id = data.PostId,
            //    UserImageName = "https://avatars2.githubusercontent.com/u/7770797?s=460&v=4",
            //    No_of_Posts = data.Responses != null ? data.Responses.Count : 0,
            //    PostContent = data.Content,
            //    PostDate = data.CreatedDate,
            //    PostTitle = data.Title,
            //    UserName = "christos_sakellarios",
            //    ContentUrl = data.ContentUrl,
            //    ContentType = data.ContentType
            //};
            //result.Community = model;
            //return PartialView("_ResponseView", result);
            return null;
        }

        // PUT: api/Forum/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
