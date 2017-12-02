using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace Online.Store.Storage
{
    public interface IStorageRepository
    {
        void Connect(string accountName, string accountKey);
        CloudBlobContainer GetBlobContainer(string containerName);
        Task CreateBlobContainerAsync(string containerName);
        Task UploadToContainerAsync(string containerName, string filePath, string blobName);
        Task<string> UploadToContainerAsync(string containerName, Stream fileStream, string blobName, string contentType);
    }
}
