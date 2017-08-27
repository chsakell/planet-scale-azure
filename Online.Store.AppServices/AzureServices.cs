using Online.Store.Core.DTOs;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace Online.Store.AppServices
{
    public class AzureServices
    {
        /// <summary>
        /// The _storage account
        /// </summary>
        private CloudStorageAccount _storageAccount;

        /// <summary>
        /// The _storage credentials
        /// </summary>
        private StorageCredentials _storageCredentials;

        /// <summary>
        /// The _media service
        /// </summary>
        private AzureMediaServices _mediaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServices"/> class.
        /// </summary>
        public AzureServices(
            string mediaAccountKey, 
            string mediaAccountName, 
            string blobAccountName, 
            string blobAccountKey)
        {
            _storageCredentials = _storageCredentials ?? new StorageCredentials(blobAccountName, blobAccountKey);
            _storageAccount = _storageAccount ?? new CloudStorageAccount(_storageCredentials, useHttps: true);
            _mediaService = new AzureMediaServices(mediaAccountKey, mediaAccountName, blobAccountName, blobAccountKey);
        }

        /// <summary>
        /// Uploads the media.
        /// </summary>
        /// <param name="filesStream">The files stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public MediaDetailsDTO UploadMedia(Stream filesStream, string filename, string contentType)
        {
            var result = new MediaDetailsDTO();

            try
            {
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container;

                if (contentType.Contains("image"))
                {
                    container = blobClient.GetContainerReference("images");
                }
                else if (contentType.Contains("video"))
                {
                    container = blobClient.GetContainerReference("videos");
                }
                else
                {
                    container = blobClient.GetContainerReference("others");
                }

                string uniqueBlobName = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(filename), Guid.NewGuid().ToString(), Path.GetExtension(filename));

                if(container.CreateIfNotExists())
                {
                    container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }
                
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);

                blob.Properties.ContentType = contentType;

                using (var stream = filesStream)
                {
                    blob.UploadFromStream(stream);
                }

                result.MediaType = contentType;
                result.MediaUrl = blob.StorageUri.PrimaryUri.AbsoluteUri;
                if (contentType.Contains("video"))
                {
                    IAsset asset = _mediaService.CopyBlobToMediaAsset(blob);
                    var url = _mediaService.CreateEncodingJob(asset);
                    result.MediaUrl = url;
                }
            }
            catch (Exception Ex)
            {
                result.Status = false;
                result.Message = Ex.Message;
                return result;
            }
            result.Status = true;
            return result;
        }

    }
}
