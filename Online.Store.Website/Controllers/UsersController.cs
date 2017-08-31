using Online.Store.Azure.Services;
using Online.Store.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Online.Store.Website.Controllers
{
    public class UsersController : Controller
    {

        #region Private Region

        /// <summary>
        /// Gets or sets the _user manager.
        /// </summary>
        /// <value>
        /// The _user manager.
        /// </value>
        private UserManager<ApplicationUser> _userManager { get; set; }

        private readonly IConfiguration _configuration;

        /// <summary>
        /// The _storage account
        /// </summary>
        private CloudStorageAccount _storageAccount = null;
        
        #endregion Private Region

        #region public properties

        /// <summary>
        /// Gets the user manager.
        /// </summary>
        /// <value>
        /// The user manager.
        /// </value>
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        public UsersController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            UserManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Users the profile.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var userDetails = new UserAccountDetailsViewModel()
            {    
                FullName = user.FullName,
                City = user.City,
                Email = user.Email,
                State = user.State,
                ImageUrl = user.ImageUrl,
                Notifications = user.Notifications,
                PostNotification = user.PostNotification,
                TweetNotification = user.TweetNotification,
                ProductNotification = user.ProductNotification,
                TwitterHandle = user.TwitterHandle
            };
           
            return View(userDetails);
        }

        /// <summary>
        /// Edits this instance.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var userDetails = new UserAccountDetailsViewModel()
            {
                FullName = user.FullName,
                City = user.City,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                State = user.State,
                TwitterHandle = user.TwitterHandle,
                Notifications = user.Notifications,
                PostNotification = user.PostNotification,
                TweetNotification = user.TweetNotification,
                ProductNotification = user.ProductNotification
            };
            return View(userDetails);
        }

        /// <summary>
        /// Updates the specified user details update.
        /// </summary>
        /// <param name="userDetailsUpdate">The user details update.</param>
        /// <param name="avatar">The avatar.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Update(UserAccountDetailsViewModel userDetailsUpdate, IFormFile avatar)
        {
            if (avatar != null)
            {
                string accountName = _configuration["Storage:AccountName"];
                string accountKey = _configuration["Storage:AccountKey"];

                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                _storageAccount = new CloudStorageAccount(creds, useHttps: true);

                // Create the blob client.
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container;

                string uniqueBlobName = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(avatar.FileName), Guid.NewGuid().ToString(), Path.GetExtension(avatar.FileName));

                container = blobClient.GetContainerReference("images");

                container.CreateIfNotExists();
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = avatar.ContentType;

                using (var fileStream = avatar.OpenReadStream())
                {
                    blob.UploadFromStream(fileStream);
                }

                userDetailsUpdate.ImageUrl = blob.StorageUri.PrimaryUri.AbsoluteUri;
            }
            var user = await _userManager.GetUserAsync(User);

            user.FullName = userDetailsUpdate.FullName;
            user.City = userDetailsUpdate.City;
            user.State = userDetailsUpdate.State;
            user.Email = userDetailsUpdate.Email;
            user.ImageUrl = userDetailsUpdate.ImageUrl;
            user.Notifications = userDetailsUpdate.Notifications;
            user.PostNotification = userDetailsUpdate.PostNotification;
            user.ProductNotification = userDetailsUpdate.ProductNotification;
            user.TweetNotification = userDetailsUpdate.TweetNotification;
            user.TwitterHandle = userDetailsUpdate.TwitterHandle;

            var result = await UserManager.UpdateAsync(user);

            return RedirectToAction("UserProfile", "Users");
        }


    }
}