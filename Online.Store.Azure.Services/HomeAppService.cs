using Online.Store.Core.DTOs;
using Online.Store.DocumentDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class HomeAppService
    {
        private readonly DocumentRepository _repository;

        public HomeAppService(string databaseId, string documentDBEndpoint, string documentDBKey)
        {
            _repository = new DocumentRepository(databaseId, documentDBEndpoint, documentDBKey);
        }

        /// <summary>
        /// Gets the top community post.
        /// </summary>
        /// <returns></returns>
        public async Task<List<CommunityDTO>> GetTopCommunityPost()
        {
            var data = new List<CommunityDTO>();
            //Get all community details.
            var communities = await _repository.GetCommunity();
            data = communities.OrderByDescending(x => x.CreatedDate).ToList();
            if (data != null && data.Count > 5)
            {
                return data.Take(5).ToList();
            }
            return data ?? new List<CommunityDTO>();
        }
    }
}
