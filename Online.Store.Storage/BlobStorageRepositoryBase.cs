using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Storage
{
    public abstract class BlobStorageRepositoryBase : IBlobStorageRepository
    {
        protected string StorageAccountName = string.Empty;
        protected string StorageAccountKey = string.Empty;
        protected string ConnectionString;
        protected CloudStorageAccount cloudStorageAccount;
        protected CloudBlobClient cloudBlobClient;
        protected CloudBlobContainer cloudBlobContainer { get; set; }
        protected CloudBlockBlob cloudBlockBlob { get; set; }

        public async Task<bool> CreateContainerIfNotExistsAsync(string container)
        {
            cloudBlobContainer = cloudBlobClient.GetContainerReference(container);
            return await cloudBlobContainer.CreateIfNotExistsAsync();
        }

        public async Task SetContainerPermissionsAsync(BlobContainerPermissions permissions)
        {
            await cloudBlobContainer.SetPermissionsAsync(permissions);
        }

        public async Task UploadFromFileAsync(string blobRef, string filePath)
        {
            cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobRef);
            await cloudBlockBlob.UploadFromFileAsync(filePath);
        }

        public async Task<BlobResultSegment> ListBlobsSegmentedAsync()
        {
            return await cloudBlobContainer.ListBlobsSegmentedAsync(null, null);
        }

        public abstract void Init(string container);
    }
}
