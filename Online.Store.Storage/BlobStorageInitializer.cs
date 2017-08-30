using Microsoft.WindowsAzure.Storage.Blob;
using Online.Store.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Storage
{
    public class BlobStorageInitializer
    {
        static BlobStorageStoreRepository repository;
        public static void Initialize(string storageAccountName, string storageAccountKey)
        {
            repository = new BlobStorageStoreRepository(storageAccountName, storageAccountKey);
            repository.CreateContainerIfNotExistsAsync("products").Wait();

            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            repository.SetContainerPermissionsAsync(permissions).Wait();

            UploadImagesAsync().Wait();
        }

        private static async Task UploadImagesAsync()
        {
            repository.Init("products");

            var blobs = await repository.ListBlobsSegmentedAsync();

            if ((blobs.Results as ICollection<IListBlobItem>).Count == 0)
            {
                foreach (var filePath in Directory.GetFiles(Path.Combine(Config.ContentRootPath, @"App_Data\img")))
                {
                    string prefix = "images";
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    string fileName = Path.GetFileName(filePath);
                    await repository.UploadFromFileAsync(prefix + "/" + fileNameWithoutExtension + "/" + fileName, filePath);
                }
            }
        }
    }
}
