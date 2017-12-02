using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Core.DTOs;
using Online.Store.Azure.Services;
using Online.Store.ViewModels;

namespace Online.Store.Controllers
{
    [Produces("application/json")]
    [Route("api/Forum")]
    public class ForumController : Controller
    {
        private IStoreService _storeService;
        private IMediaService _mediaService;

        public ForumController(IStoreService storeService, IMediaService mediaService)
        {
            _storeService = storeService;
            _mediaService = mediaService;
        }

        // GET: api/Forum
        [HttpGet]
        [Route("topics/", Name = "GetTopics")]
        public async Task<IEnumerable<TopicDTO>> GetTopics()
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
        [HttpPost("topics/{id}/addreply", Name = "AddReply")]
        //[Authorize]
        public async Task<ActionResult> Post(string id, IFormFile file, ReplyViewModel reply)
        {
            PostDTO post = new PostDTO()
            {
                Title = reply.Title,
                Content = reply.Content,
                MediaDescription = reply.MediaDescription,
                UserId = reply.UserId,
                UserName = reply.UserName,
                CreatedDate = DateTime.Now
            };

            try
            {
                if (file != null)
                {
                    using (var filestream = file.OpenReadStream())
                    {
                        post.MediaUrl = await _mediaService.UploadMediaAsync(filestream, file.FileName, file.ContentType);
                        post.MediaType = file.ContentType;
                    }
                }

                var topic = await _storeService.AddTopicReply(id, post);

                return Ok(new ResultViewModel()
                {
                    Result = Result.SUCCESS,
                    Data = topic
                });
            }
            catch (Exception ex)
            {
                return Ok(new ResultViewModel()
                {
                    Result = Result.ERROR,
                    Message = ex.Message
                });
            }
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
