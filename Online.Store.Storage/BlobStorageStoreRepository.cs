using Microsoft.WindowsAzure.Storage;

using System;
using System.Collections.Generic;
using System.Text;

namespace Online.Store.Storage
{
    public class BlobStorageStoreRepository : BlobStorageRepositoryBase
    {
        public BlobStorageStoreRepository(string storageAccountName, string storageAccountKey)
        {
            //StorageAccountName = configuration["StorageAccountName"];
            //StorageAccountKey = configuration["StorageAccountKey"];
            StorageAccountName = storageAccountName;
            StorageAccountKey = storageAccountKey;

            ConnectionString =
              string.Format(@"DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
              StorageAccountName, StorageAccountKey);

            if (cloudStorageAccount == null)
                cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);

            if (cloudBlobClient == null)
                cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        public override void Init(string container)
        {
            cloudBlobContainer = cloudBlobClient.GetContainerReference(container);
        }
    }
}
