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
        public void Post([FromBody]string value)
        {
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
