using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace Online.Store.Storage
{
    public interface IBlobStorageRepository
    {
        Task<bool> CreateContainerIfNotExistsAsync(string container);
        Task SetContainerPermissionsAsync(BlobContainerPermissions permissions);
        Task UploadFromFileAsync(string blobRef, string filePath);
        Task<BlobResultSegment> ListBlobsSegmentedAsync();
        void Init(string container);
    }
}
