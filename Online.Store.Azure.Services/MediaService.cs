using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Online.Store.Azure.Services
{
    public class MediaService
    {
        #region Private Members

        /// <summary>
        /// The Account key for media services
        /// </summary>
        private string _accountKey;
        /// <summary>
        /// The Account Name for media services
        /// </summary>
        private string _accountName;
        /// <summary>
        /// The storage account name
        /// </summary>
        private string _storageAccountName;
        /// <summary>
        /// The storage account key
        /// </summary>
        private string _storageAccountKey;

        // Field for service context.
        /// <summary>
        /// The _context
        /// </summary>
        private static CloudMediaContext _context = null;

        /// <summary>
        /// The _cached media credentials
        /// </summary>
        private static MediaServicesCredentials _cachedMediaCredentials = null;

        /// <summary>
        /// The _source storage account
        /// </summary>
        //private static CloudStorageAccount _sourceStorageAccount = null;

        /// <summary>
        /// The _destination storage account
        /// </summary>
        private static CloudStorageAccount _destinationStorageAccount = null;

        /// <summary>
        /// The _storage credentials
        /// </summary>
        private static StorageCredentials _storageCredentials = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaService"/> class.
        /// </summary>
        public MediaService(string mediaAccountKey, string mediaAccountName, string blobAccountName, string blobAccountKey)
        {
            _accountKey = mediaAccountKey;
            _accountName = mediaAccountName;
            _storageAccountName = blobAccountName;
            _storageAccountKey = blobAccountKey;

            _cachedMediaCredentials = _cachedMediaCredentials ?? new MediaServicesCredentials(_accountName, _accountKey);
            _context = _context ?? new CloudMediaContext(_cachedMediaCredentials);
            _storageCredentials = _storageCredentials ?? new StorageCredentials(_storageAccountName, _storageAccountKey);
            _destinationStorageAccount = _destinationStorageAccount ?? new CloudStorageAccount(_storageCredentials, false);
        }

        /// <summary>
        /// copy the blob content to media asset
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <returns></returns>
        public IAsset CopyBlobToMediaAsset(ICloudBlob blob)
        {
            // Create a new asset and copy the smooth streaming files into 
            // the container that is associated with the asset.
            IAsset asset = CreateAssetFromExistingBlobs(blob);

            return asset;
        }

        /// <summary>
        /// Creates the asset from existing blobs.
        /// </summary>
        /// <param name="sourceBlob">The source BLOB.</param>
        /// <returns></returns>
        private IAsset CreateAssetFromExistingBlobs(ICloudBlob sourceBlob)
        {
            // Create a new asset. 
            IAsset asset = _context.Assets.Create("NewAsset_" + Guid.NewGuid(), AssetCreationOptions.None);

            IAccessPolicy writePolicy = _context.AccessPolicies.Create("writePolicy",
                TimeSpan.FromHours(24), AccessPermissions.Write);

            ILocator destinationLocator = _context.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);

            CloudBlobClient destBlobStorage = _destinationStorageAccount.CreateCloudBlobClient();

            // Get the asset container URI and Blob copy from mediaContainer to assetContainer. 
            string destinationContainerName = (new Uri(destinationLocator.Path)).Segments[1];

            CloudBlobContainer assetContainer = destBlobStorage.GetContainerReference(destinationContainerName);

            if (assetContainer.CreateIfNotExists())
            {
                assetContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }

            var assetFile = asset.AssetFiles.Create((sourceBlob as ICloudBlob).Name);
            this.CopyBlob(sourceBlob as ICloudBlob, assetContainer);

            destinationLocator.Delete();
            writePolicy.Delete();

            // Since we copied a set of Smooth Streaming files, 
            // set the .ism file to be the primary file. 
            //SetISMFileAsPrimary(asset);

            return asset;

        }

        /// <summary>
        /// Copies the BLOB.
        /// </summary>
        /// <param name="sourceBlob">The source BLOB.</param>
        /// <param name="destinationContainer">The destination container.</param>
        private void CopyBlob(ICloudBlob sourceBlob, CloudBlobContainer destinationContainer)
        {
            var signature = sourceBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
            });

            CloudBlob destinationBlob = destinationContainer.GetBlockBlobReference(sourceBlob.Name);
            if (destinationBlob.Exists())
            {
            }
            else
            {
                try
                {
                    destinationBlob.StartCopy(new Uri(sourceBlob.Uri.AbsoluteUri + signature));

                    while (true)
                    {
                        // The StartCopyFromBlob is an async operation, 
                        // so we want to check if the copy operation is completed before proceeding. 
                        // To do that, we call FetchAttributes on the blob and check the CopyStatus. 
                        destinationBlob.FetchAttributes();
                        if (destinationBlob.CopyState.Status != CopyStatus.Pending)
                        {
                            break;
                        }
                        //It's still not completed. So wait for some time.
                        System.Threading.Thread.Sleep(1000);
                    }


                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Creates the streaming locator.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        private string CreateStreamingLocator(IAsset asset)
        {
            var ismAssetFile = asset.AssetFiles.ToList().
                Where(f => f.Name.EndsWith(".ism", StringComparison.OrdinalIgnoreCase)).First();

            // Create a 30-day readonly access policy. 
            IAccessPolicy policy = _context.AccessPolicies.Create("Streaming policy",
                TimeSpan.FromDays(30),
                AccessPermissions.Read);

            // Create a locator to the streaming content on an origin. 
            ILocator originLocator = _context.Locators.CreateLocator(LocatorType.OnDemandOrigin, asset,
                policy,
                DateTime.UtcNow.AddMinutes(-5));

            return originLocator.Path + ismAssetFile.Name + "/manifest";
        }

        #region "Encoding"

        /// <summary>
        /// Creates the encoding job.
        /// </summary>
        /// <param name="uploadAsset">The upload asset.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not find assetId:  + encodeAssetId</exception>
        public string CreateEncodingJob(IAsset uploadAsset)
        {
            var encodeAssetId = uploadAsset.Id; // "YOUR ASSET ID";
            // Preset reference documentation: http://msdn.microsoft.com/en-us/library/windowsazure/jj129582.aspx
            var encodingPreset = "Adaptive Streaming";// "H264 Smooth Streaming 720p";
            var assetToEncode = _context.Assets.Where(a => a.Id == encodeAssetId).FirstOrDefault();
            if (assetToEncode == null)
            {
                throw new ArgumentException("Could not find assetId: " + encodeAssetId);
            }

            IJob job = _context.Jobs.Create("Encoding " + assetToEncode.Name + " to " + encodingPreset);

            IMediaProcessor latestWameMediaProcessor = (from p in _context.MediaProcessors where p.Name == "Media Encoder Standard" select p).ToList().OrderBy(wame => new Version(wame.Version)).LastOrDefault();
            ITask encodeTask = job.Tasks.AddNew("Encoding", latestWameMediaProcessor, encodingPreset, TaskOptions.None);
            encodeTask.InputAssets.Add(assetToEncode);
            encodeTask.OutputAssets.AddNew(assetToEncode.Name + " as " + encodingPreset, AssetCreationOptions.None);

            job.StateChanged += new EventHandler<JobStateChangedEventArgs>((sender, jsc) => Console.WriteLine(string.Format("{0}\n  State: {1}\n  Time: {2}\n\n", ((IJob)sender).Name, jsc.CurrentState, DateTime.UtcNow.ToString(@"yyyy_M_d_hhmmss"))));
            job.Submit();
            job.GetExecutionProgressTask(CancellationToken.None).Wait();

            var preparedAsset = job.OutputMediaAssets.FirstOrDefault();

            string surl = this.GetStreamingUrl(preparedAsset);

            return surl;
        }

        /// <summary>
        /// Gets the streaming URL.
        /// </summary>
        /// <param name="preparedAsset">The prepared asset.</param>
        /// <returns></returns>
        public string GetStreamingUrl(IAsset preparedAsset)
        {
            var streamingAssetId = preparedAsset.Id; // "YOUR ASSET ID";
            var daysForWhichStreamingUrlIsActive = 365;
            var streamingAsset = _context.Assets.Where(a => a.Id == streamingAssetId).FirstOrDefault();
            var accessPolicy = _context.AccessPolicies.Create(streamingAsset.Name, TimeSpan.FromDays(daysForWhichStreamingUrlIsActive),
                                                     AccessPermissions.Read);
            string streamingUrl = string.Empty;
            var assetFiles = streamingAsset.AssetFiles.ToList();
            var streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith("m3u8-aapl.ism")).FirstOrDefault();
            if (streamingAssetFile != null)
            {
                var locator = _context.Locators.CreateLocator(LocatorType.OnDemandOrigin, streamingAsset, accessPolicy);
                Uri hlsUri = new Uri(locator.Path + streamingAssetFile.Name + "/manifest(format=m3u8-aapl)");
                streamingUrl = hlsUri.ToString();
            }
            streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".ism")).FirstOrDefault();
            if (string.IsNullOrEmpty(streamingUrl) && streamingAssetFile != null)
            {
                var locator = _context.Locators.CreateLocator(LocatorType.OnDemandOrigin, streamingAsset, accessPolicy);
                Uri smoothUri = new Uri(locator.Path + streamingAssetFile.Name + "/manifest(format=mpd-time-csf)"); //mpeg-dash
                streamingUrl = smoothUri.ToString();
            }
            streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".mp4")).FirstOrDefault();
            if (string.IsNullOrEmpty(streamingUrl) && streamingAssetFile != null)
            {
                var locator = _context.Locators.CreateLocator(LocatorType.Sas, streamingAsset, accessPolicy);
                var mp4Uri = new UriBuilder(locator.Path);
                mp4Uri.Path += "/" + streamingAssetFile.Name;
                streamingUrl = mp4Uri.ToString();
            }

            return streamingUrl;
        }

        /// <summary>
        /// Gets the name of the latest media processor by.
        /// </summary>
        /// <param name="mediaProcessorName">Name of the media processor.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        private IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            var processor = _context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
                ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor", mediaProcessorName));

            return processor;
        }

        /// <summary>
        /// Gets the asset sas URL list.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="locator">The locator.</param>
        /// <returns></returns>
        private List<String> GetAssetSasUrlList(IAsset asset, ILocator locator)
        {
            // Declare a list to contain all the SAS URLs.
            List<String> fileSasUrlList = new List<String>();

            // If the asset has files, build a list of URLs to 
            // each file in the asset and return. 
            foreach (IAssetFile file in asset.AssetFiles)
            {
                string sasUrl = BuildFileSasUrl(file, locator);
                fileSasUrlList.Add(sasUrl);
            }

            // Return the list of SAS URLs.
            return fileSasUrlList;
        }

        // Create and return a SAS URL to a single file in an asset. 
        private string BuildFileSasUrl(IAssetFile file, ILocator locator)
        {
            // Take the locator path, add the file name, and build 
            // a full SAS URL to access this file. This is the only 
            // code required to build the full URL.
            var uriBuilder = new UriBuilder(locator.Path);
            uriBuilder.Path += "/" + file.Name;

            //Return the SAS URL.
            return uriBuilder.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Gets the job.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns></returns>
        private IJob GetJob(string jobId)
        {
            // Use a Linq select query to get an updated 
            // reference by Id. 
            var jobInstance =
                from j in _context.Jobs
                where j.Id == jobId
                select j;
            // Return the job reference as an Ijob. 
            IJob job = jobInstance.FirstOrDefault();

            return job;
        }

        #endregion
    }
}
