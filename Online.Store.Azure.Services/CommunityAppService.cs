using Online.Store.Core.DTOs;
using Online.Store.DocumentDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class CommunityAppService
    {
        private readonly DocumentRepository _repository;
        private readonly int _pageSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityAppService"/> class.
        /// </summary>
        public CommunityAppService(string databaseId, string documentDBEndpoint, string documentDBKey, string pageSize)
        {
            _repository = new DocumentRepository(databaseId, documentDBEndpoint, documentDBKey);
            _pageSize = int.Parse(pageSize);
        }

        /// <summary>
        /// Add New Post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns></returns>
        public async Task<CommunityDTO> Post(PostDTO post)
        {
            var datatoAdd = new CommunityDTO()
            {
                PostId = Guid.NewGuid().ToString(),
                Content = post.Content,
                ContentType = post.ContentType,
                CreatedDate = DateTime.Now,
                MediaDescription = post.MediaDescription,
                Title = post.Title,
                UserId = post.UserId,
                ContentUrl = post.ContentUrl
            };

            await _repository.AddPost(datatoAdd);
            //await this.UpdateUserPostCount(post.UserId);

            return datatoAdd;
        }

        /// <summary>
        /// Posts the replay.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns></returns>
        public async Task<CommunityDTO> PostReplay(PostDTO post)
        {
            var datatoAdd = new CommunityDTO()
            {
                PostId = Guid.NewGuid().ToString(),
                Content = post.Content,
                ContentType = post.ContentType,
                CreatedDate = DateTime.Now,
                MediaDescription = post.MediaDescription,
                Title = post.Title,
                UserId = post.UserId,
                ContentUrl = post.ContentUrl
            };

            if (!string.IsNullOrEmpty(post.PostId))
            {
                var entity = await _repository.GetCommunityById(post.PostId);
                if (entity != null)
                {
                    if (entity.Responses == null)
                        entity.Responses = new List<CommunityDTO>();

                    entity.Responses.Add(datatoAdd);

                    await _repository.AddPostResponse(entity);
                }
            }

            return datatoAdd;
        }

        /// <summary>
        /// Gets the details of selected post.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="filterId">The filter identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        public async Task<CommunityResponseDto> GetDetails(string id, string filterId, int? pageId)
        {
            int pageSize = _pageSize;
            var communityResult = new CommunityResponseDto();

            var entity = await _repository.GetCommunityById(id);

            if (entity != null)
            {
                var communityResponses = new List<CommunityDTO>();
                if (entity.Responses != null && entity.Responses.Any())
                {
                    communityResponses = entity.Responses.OrderByDescending(x => x.CreatedDate).ToList();
                    //Filter basesed on request.
                    if (filterId != null && filterId != "all")
                    {
                        var result = communityResponses.FindAll(x => !string.IsNullOrEmpty(x.ContentType) && x.ContentType.Contains(filterId));
                        communityResponses = result.ToList();
                    }

                    //Get total count and total page count.
                    var totalCount = communityResponses.Count();
                    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                    //Implementing pagination.
                    var pages = communityResponses.Skip((pageId ?? 0) * pageSize).Take(pageSize).ToList();
                    entity.Responses = pages.ToList();

                    communityResult.Community = entity;
                    communityResult.TotalPages = totalPages;
                    communityResult.SelectedPage = pageId != null ? pageId.Value : 0;
                }
                communityResult.Community = entity;
            }
            return communityResult;
        }

        /// <summary>
        /// Gets all community.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        public async Task<PagedCommunityDto> GetAllCommunity(string id, int? pageId)
        {
            int pageSize = _pageSize;
            var community = new PagedCommunityDto();
            var data = new List<CommunityDTO>();

            //Get all community details.
            var communities = await _repository.GetCommunity();
            data = communities.OrderByDescending(x => x.CreatedDate).ToList();

            //Filter basesed on request.
            if (id != null && id != "all")
            {
                var result = data.FindAll(x => !string.IsNullOrEmpty(x.ContentType) && x.ContentType.Contains(id));
                data = result.ToList();
            }

            //Get total count and total page count.
            var totalCount = data.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            //Implementing pagination.
            var pages = data.Skip((pageId ?? 0) * pageSize).Take(pageSize).ToList();
            community.Community = pages.ToList();
            community.TotalPages = totalPages;
            community.SelectedPage = pageId != null ? pageId.Value : 0;

            return community;
        }

        /// <summary>
        /// Gets the community details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<CommunityDTO> GetCommunityDetails(string id)
        {
            var entity = await _repository.GetCommunityById(id);

            if (entity != null)
            {
                return entity;
            }
            return new CommunityDTO();
        }

        /*public async Task<UserPostsDTO> UpdateUserPostCount(string userId)
        {
            var result = new UserPostsDTO();
            var userPostExists = await _repository.GetUserPostsCountById(userId);
            if(userPostExists!=null)
            {
                result = userPostExists;
                result.PostCount = result.PostCount + 1;
                await _repository.AddUserPost(result);
            }
            else
            {
                result.UserPostId = userId;
                result.PostCount = 1;
                await _repository.UpdateUserPost(result);
            }

            return result;
        }*/

    }
}
