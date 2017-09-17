using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Storage
{
    public interface IStorageRepository
    {
        void Connect(string accountName, string accountKey);
        CloudBlobContainer GetBlobContainer(string containerName);
        Task CreateBlobContainerAsync(string containerName);
        Task UploadToContainerAsync(string containerName, string filePath, string blobName);
    }
}
