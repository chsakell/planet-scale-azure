using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Storage
{
    public class StorageRepository : IStorageRepository
    {
        protected CloudStorageAccount cloudStorageAccount { get; set; }
        protected CloudBlobClient cloudBlobClient { get; set; }

        public void Connect(string accountName, string accountKey)
        {
            string connection =
              string.Format(@"DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}"
              ,accountName, accountKey);

            cloudStorageAccount = accountName == "devstoreaccount1" ? 
                CloudStorageAccount.DevelopmentStorageAccount : 
                CloudStorageAccount.Parse(connection);
            cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        public async Task CreateBlobContainerAsync(string containerName)
        {
            CloudBlobContainer cloudBlobContainer = GetBlobContainer(containerName);

            bool containerExists = await cloudBlobContainer.ExistsAsync();
            {
                if (!containerExists)
                {
                    await cloudBlobContainer.CreateAsync();
                    BlobContainerPermissions permissions = new BlobContainerPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
                }
            }
        }

        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            return cloudBlobContainer;
        }

        public async Task UploadToContainerAsync(string containerName, string filePath, string blobName)
        {
            CloudBlobContainer cloudBlobContainer = GetBlobContainer(containerName);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            await cloudBlockBlob.UploadFromFileAsync(filePath);
        }

        public async Task<string> UploadToContainerAsync(string containerName, Stream fileStream, string blobName, string contentType)
        {
            CloudBlobContainer cloudBlobContainer = GetBlobContainer(containerName);
            CloudBlockBlob cloudBlob = cloudBlobContainer.GetBlockBlobReference(blobName);

            cloudBlob.Properties.ContentType = contentType;

            using (var stream = fileStream)
            {
                await cloudBlob.UploadFromStreamAsync(stream);
            }

            return cloudBlob.StorageUri.PrimaryUri.AbsoluteUri;
        }
    }
}
