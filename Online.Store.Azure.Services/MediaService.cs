using Microsoft.Extensions.Configuration;
using Online.Store.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Azure.Services
{
    public class MediaService : IMediaService
    {
        private IStorageRepository _storageRepository;

        public MediaService(IConfiguration configuration, IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;

            _storageRepository.Connect(configuration["Storage:AccountName"], 
                                       configuration["Storage:AccountKey"]);
        }

        public async Task<string> UploadMediaAsync(Stream stream, string filename, string contentType)
        {
            string container = contentType.ToLower().Contains("image") ? "forum-images" : "forum-videos";

            return await _storageRepository.UploadToContainerAsync(container, stream, filename, contentType);
        }
    }

    public interface IMediaService
    {
        Task<string> UploadMediaAsync(Stream stream, string filename, string contentType);
    }
}
