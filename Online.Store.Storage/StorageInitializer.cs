using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Storage
{
    public class StorageInitializer
    {
        private static string accountName;
        private static string accountKey;
        public static StorageRepository _repository;

        public static void Initialize(IConfiguration configuration)
        {
            accountName = configuration["Storage:AccountName"];
            accountKey = configuration["Storage:AccountKey"];

            _repository = new StorageRepository();
            _repository.Connect(accountName, accountKey);

            InitContainerAsync("product-images").Wait();
        }

        public static async Task InitContainerAsync(string container)
        {
            await _repository.CreateBlobContainerAsync(container);
        }

        private static async Task UploadProductImagesAsync()
        {
            string filePath = @"C:\Users\developer\Desktop\cameras\Sony - DSC-W830 20.1-Megapixel Digital Camera - Silver\1.jpg";
            await _repository.UploadToContainerAsync("product-images", filePath, "myfile.jpg");
        }
    }
}
