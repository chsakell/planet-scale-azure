using Online.Store.Core.DTOs;
using Online.Store.DocumentDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class StoreService : IStoreService
    {
        #region "Private Constant Members"

        private const string _PRODUCT_COLLECTION_ID = "Products";
        private const string _CART_COLLECTION_ID = "Cart";
        private const string _COMMUNITY_COLLECTION_ID = "Community";

        #endregion

        /// <summary>
        /// The DocumentDB _repository
        /// </summary>
        private IDocumentDBRepository<DocumentDBStoreRepository> _repository;

        public StoreService(IDocumentDBRepository<DocumentDBStoreRepository> _repository)
        {
            this._repository = _repository;
        }

        public async Task<List<CommunityDTO>> GetTopCommunityPost()
        {
            var data = new List<CommunityDTO>();
            //Get all community details.
            //var communities = await _repository.GetCommunity();
            await _repository.InitAsync(_COMMUNITY_COLLECTION_ID);

            var communities = await this._repository.GetItemsAsync<CommunityDTO>();

            data = communities.OrderByDescending(x => x.CreatedDate).ToList();
            if (data != null && data.Count > 5)
            {
                return data.Take(5).ToList();
            }
            return data ?? new List<CommunityDTO>();
        }
    }

    public interface IStoreService
    {
        Task<List<CommunityDTO>> GetTopCommunityPost();
    }
}
