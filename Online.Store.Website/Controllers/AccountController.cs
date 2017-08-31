using Online.Store.Azure.Services;
using Online.Store.Core.DTOs;
using Online.Store.Website.Models;
using Online.Store.Website.Models.AccountViewModels;
using Online.Store.Website.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.Website.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly string _externalCookieScheme;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _configuration = configuration;
        }

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET: /Account/Login
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> Login(string returnUrl = null)
        //{
        //    // Clear the existing external cookie to ensure a clean login process
        //    await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(1, "User logged in.");
                        return Json(new MessageDTO { Status = true, Message = "Successful login." });
                    }
                    // TEMPLATED CODE : REMOVED DUE TO CURRENT UNUSE
                    //if (result.RequiresTwoFactor)
                    //{
                    //    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    //}
                    //if (result.IsLockedOut)
                    //{
                    //    _logger.LogWarning(2, "User account locked out.");
                    //    return View("Lockout");
                    //}
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Json(new MessageDTO { Status = false, Message = "The email or password you entered is incorrect." });
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, IFormFile mediaFile, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                if (mediaFile != null)
                {
                    var azureService = new AzureServices(
                        _configuration["MediaServices:AccountKey"], 
                        _configuration["MediaServices:AccountName"], 
                        _configuration["Storage:AccountName"], 
                        _configuration["Storage:AccountKey"]);

                    using (var stream = mediaFile.OpenReadStream())
                    {
                        var mediaresult = azureService.UploadMedia(stream, mediaFile.FileName, mediaFile.ContentType);
                        model.ImageUrl = mediaresult.MediaUrl;
                    }
                }

                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                    State = model.State,
                    ImageUrl = model.ImageUrl,
                    TwitterHandle = model.TwitterHandle,
                    Notifications = model.Notifications,
                    ProductNotification = model.ProductNotification,
                    PostNotification = model.ProductNotification,
                    TweetNotification = model.TweetNotification,
                };

                IdentityResult result;

                result = await _userManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                }
                
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                    return !string.IsNullOrEmpty(returnUrl) ? RedirectToLocal(returnUrl) : RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterExternal(RegisterExternalViewModel model, IFormFile mediaFile, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if(info == null)
                {
                    ViewBag.BadMessage = "Invalid Registration";
                    return RedirectToAction("Index", "Home");
                }

                var user = new ApplicationUser
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                    State = model.State,
                    ImageUrl = model.ImageUrl,
                    TwitterHandle = model.TwitterHandle,
                    Notifications = model.Notifications,
                    ProductNotification = model.ProductNotification,
                    PostNotification = model.ProductNotification,
                    TweetNotification = model.TweetNotification,
                };

                IdentityResult result;

                result = await _userManager.CreateAsync(user); // no password for external login
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                    }
                }

                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                    return !string.IsNullOrEmpty(returnUrl) ? RedirectToLocal(returnUrl) : RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Logout
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ViewBag.BadMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction("Index", "Home");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ViewBag.BadMessage = $"{info.LoginProvider} could not log you in";
                return RedirectToAction("Index", "Home");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                // Save access_token and access_token_secret so that we can call the API later
                var exResult = await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            // TEMPLATED CODE : REMOVED DUE TO CURRENT UNUSE
            //if (result.RequiresTwoFactor)
            //{
            //    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            //}
            //if (result.IsLockedOut)
            //{
            //    return View("Lockout");
            //}
            else
            {
                // Build a RegisterExternalViewModel so the user can create a local profile
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var data = new RegisterExternalViewModel()
                {
                    Email = email
                };

                if (info.LoginProvider == "Twitter")
                {
                    var oauth_consumer_key = _configuration["Authentication:Twitter:ConsumerKey"];
                    var oauth_consumer_secret = _configuration["Authentication:Twitter:ConsumerSecret"];
                    var oauth_access_token = info.AuthenticationTokens.FirstOrDefault(t => t.Name == "access_token").Value;
                    var oauth_access_token_secret = info.AuthenticationTokens.FirstOrDefault(t => t.Name == "access_token_secret").Value;
                    var screen_name = info.Principal.FindFirstValue("urn:twitter:screenname");
                    data.TwitterHandle = screen_name;

                    var rawTwitterProfile = await GetTwitterUserProfileAsync(screen_name, oauth_consumer_key, oauth_consumer_secret, oauth_access_token, oauth_access_token_secret);
                    if (rawTwitterProfile != null)
                    {
                        JObject twitterProfile = JsonConvert.DeserializeObject(rawTwitterProfile) as JObject;
                        data.FullName = twitterProfile.GetValue("name")?.Value<string>();
                        data.ImageUrl = twitterProfile.GetValue("profile_image_url")?.Value<string>();
                        data.ImageUrl = data.ImageUrl.Replace("_normal.jpg", "_400x400.jpg");
                        data.City = twitterProfile.GetValue("location")?.Value<string>();

                        return View("RegisterExternal", data);
                    }
                }

                return View("RegisterExternal", data);
            }
        }

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await _signInManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await _userManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await _userManager.AddLoginAsync(user, info);
        //            if (result.Succeeded)
        //            {
        //                await _signInManager.SignInAsync(user, isPersistent: false);
        //                _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View(model);
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        //// GET: /Account/ConfirmEmail
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await _userManager.ConfirmEmailAsync(user, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET: /Account/ForgotPassword
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
        //        // Send an email with this link
        //        //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        //var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
        //        //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
        //        //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
        //        //return View("ForgotPasswordConfirmation");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET: /Account/ResetPassword
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ResetPassword(string code = null)
        //{
        //    return code == null ? View("Error") : View();
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
        //    }
        //    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET: /Account/SendCode
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        //{
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }

        //    // Generate the token and send it
        //    var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
        //    if (string.IsNullOrWhiteSpace(code))
        //    {
        //        return View("Error");
        //    }

        //    var message = "Your security code is: " + code;
        //    if (model.SelectedProvider == "Email")
        //    {
        //        await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
        //    }
        //    else if (model.SelectedProvider == "Phone")
        //    {
        //        await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
        //    }

        //    return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET: /Account/VerifyCode
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes.
        //    // If a user enters incorrect codes for a specified amount of time then the user account
        //    // will be locked out for a specified amount of time.
        //    var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToLocal(model.ReturnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        _logger.LogWarning(7, "User account locked out.");
        //        return View("Lockout");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid code.");
        //        return View(model);
        //    }
        //}

        // TEMPLATED METHOD : REMOVED DUE TO CURRENT UNUSE
        ////
        //// GET /Account/AccessDenied
        //[HttpGet]
        //public IActionResult AccessDenied()
        //{
        //    return View();
        //}

        #region Helpers

        private async Task<string> GetTwitterUserProfileForSignedInUserAsync(string consumerKey, string consumerSecret)
        {
            // load user details - oauth token, token secret, screen name
            var user = await _userManager.GetUserAsync(User);
            var oauthToken = await _userManager.GetAuthenticationTokenAsync(user, "Twitter", "access_token");
            var oauthTokenSecret = await _userManager.GetAuthenticationTokenAsync(user, "Twitter", "access_token_secret");
            if (oauthToken == null || oauthToken == null)
            {
                return null;
            }

            return await GetTwitterUserProfileAsync(user.TwitterHandle, consumerKey, consumerSecret, oauthToken, oauthTokenSecret);
        }

        private async Task<string> GetTwitterUserProfileAsync(string screen_name, string consumerKey, string consumerSecret, string access_token, string access_token_secret)
        {
            var url = "https://api.twitter.com/1.1/users/show.json";

            var authorizationString = BuildTwitterAuthorizationHeader(screen_name, consumerKey, consumerSecret, access_token, access_token_secret);

            // GET request to url with authorization header
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authorizationString);
            var result = await httpClient.GetAsync($"{url}?screen_name={screen_name}");

            // view result
            var responseBody = await result.Content.ReadAsStringAsync();
            return responseBody;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private string BuildTwitterAuthorizationHeader(string screen_name, string consumerKey, string consumerSecret, string oauthToken, string oauthTokenSecret)
        {
            var url = "https://api.twitter.com/1.1/users/show.json";
            var method = "GET";

            // 7 standard parameters
            string oauth_consumer_key = consumerKey;
            string oauth_nonce = "";
            string oauth_signature = "";
            string oauth_signature_method = "HMAC-SHA1";
            string oauth_timestamp = "";
            string oauth_token = oauthToken;
            string oauth_version = "1.0";

            // generate the nonce value
            char[] nonceChars = new char[32];
            int charCount = 0;
            var rand = new Random();
            for (; charCount < 32;)
            {
                var c = Convert.ToChar(rand.Next(48, 122));
                if (char.IsLetter(c))
                {
                    nonceChars[charCount++] = c;
                }
            }
            oauth_nonce = new string(nonceChars);

            oauth_timestamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();

            // generate the signature
            StringBuilder sigBuilder = new StringBuilder();
            var signatureDictionary = new Dictionary<string, string>
            {
                { nameof(oauth_consumer_key), oauth_consumer_key },
                { nameof(oauth_nonce), oauth_nonce },
                { nameof(oauth_signature_method), oauth_signature_method },
                { nameof(oauth_timestamp), oauth_timestamp },
                { nameof(oauth_token), oauth_token },
                { nameof(oauth_version), oauth_version },
                { nameof(screen_name), screen_name }
            };
            string lastKey = nameof(screen_name);
            foreach(var kvp in signatureDictionary.OrderBy(k => k.Key))
            {
                sigBuilder.Append($"{RFC3986Encode(kvp.Key)}={RFC3986Encode(kvp.Value)}");
                if (!string.Equals(kvp.Key, lastKey)) sigBuilder.Append("&");
            }
            var signatureBaseString = $"{method}&{RFC3986Encode(url)}&{RFC3986Encode(sigBuilder.ToString())}";
            var signingKey = $"{RFC3986Encode(consumerSecret)}&{RFC3986Encode(oauthTokenSecret)}";
            HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
            oauth_signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString)));

            // generate the final result
            var finalParamsDictionary = new Dictionary<string, string>
            {
                { nameof(oauth_consumer_key), oauth_consumer_key },
                { nameof(oauth_nonce), oauth_nonce },
                { nameof(oauth_signature), oauth_signature },
                { nameof(oauth_signature_method), oauth_signature_method },
                { nameof(oauth_timestamp), oauth_timestamp },
                { nameof(oauth_token), oauth_token },
                { nameof(oauth_version), oauth_version }
            };

            var result = new StringBuilder();
            lastKey = nameof(oauth_version);
            foreach (var kvp in finalParamsDictionary.OrderBy(k => k.Key))
            {
                result.Append($"{RFC3986Encode(kvp.Key)}=\"{RFC3986Encode(kvp.Value)}\"");
                if (!string.Equals(kvp.Key, lastKey)) result.Append(", ");
            }

            // done
            return result.ToString();
        }

        // From https://shlomio.wordpress.com/2013/03/10/c-snippet-convertencode-string-to-escaped-representation-url-encoding/
        private string RFC3986Encode(string value)
        {
            string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };
            StringBuilder escaped = new StringBuilder(System.Uri.EscapeDataString(value));
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], System.Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }
            return escaped.ToString();
        }

        #endregion
    }
}
