using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Core.DTOs;
using Online.Store.Azure.Services;
using Online.Store.ViewModels;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]
        [Route("topics/", Name = "GetTopics")]
        public async Task<IEnumerable<Topic>> GetTopics()
        {
            var topics = await _storeService.GetTopics(2, string.Empty);
            
            return topics.OrderByDescending(t => t.CreatedDate);
        }

        [HttpGet("topics/{id}", Name = "GetTopic")]
        public async Task<Topic> GetTopic(string id)
        {
            var topic = await _storeService.GetTopic(id);

            return topic;
        }

        [HttpPost("topics/create", Name = "Create")]
        [Authorize]
        public async Task<ActionResult> Create(IFormFile file, ReplyViewModel reply)
        {
            Topic topic = new Topic()
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
                        topic.MediaUrl = await _mediaService.UploadMediaAsync(filestream, file.FileName, file.ContentType);
                        topic.MediaType = file.ContentType;
                    }
                }

                await _storeService.AddTopicAsync(topic);

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

        [HttpPost("topics/{id}/addreply", Name = "AddReply")]
        [Authorize]
        public async Task<ActionResult> AddReply(string id, IFormFile file, ReplyViewModel reply)
        {
            Post post = new Post()
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
