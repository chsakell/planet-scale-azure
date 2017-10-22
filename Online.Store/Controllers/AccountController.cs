using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Online.Store.Models.AccountViewModels;
using Online.Store.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Online.Store.Core;

namespace Online.Store.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var callbackUrl = Url.Action(nameof(HomeController.Index), "Home", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpPost(Name = "RegisterAD")]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Icr78VL86Up+ZxLdv+OR5aQVKov2rsg9wzVfXKZpbAg=");

                /* OAuth2 is required to access this API. For more information visit:
                   https://msdn.microsoft.com/en-us/office/office365/howto/common-app-authentication-tasks */

                // Specify values for path parameters (shown as {...})
                var uri = "https://graph.windows.net/chsakellhotmail.onmicrosoft.com/users?api-version=1.6";

                var jsonObject = new JObject
                        {
                            {"accountEnabled", true},
                            {"creationType", "LocalAccount"},
                            {"displayName", model.Username.Trim()},
                            {"passwordPolicies", "DisablePasswordExpiration,DisableStrongPassword"},
                            {"passwordProfile", new JObject
                            {
                                {"password", model.Password},
                                {"forceChangePasswordNextLogin", false}
                            } },
                            {"signInNames", new JArray
                                {
                                    new JObject
                                    {
                                        {"value", model.Email.Trim()},
                                        {"type", "emailAddress"}
                                    }
                                }
                            }
                        };

                try
                {
                    string ClientId = "3437ef01-f3e1-49f9-8114-afcc5d884cfb";
                    string ClientSecret = "+wPsOTMiT+/fPKL6X1H++hZWTDV5S1jbdGC4r2R3otk=";
                    string TenantId = "PlanetScaleStoreTenant.onmicrosoft.com";

                    var b2cClient = new B2CGraphClient(ClientId, ClientSecret, TenantId);
                    var response = await b2cClient.CreateUser(jsonObject.ToString());
                    var adUser = JsonConvert.DeserializeObject<ActiveDirectoryUser>(response);

                    if (adUser != null)
                    {
                        return Ok(new ResultViewModel()
                        {
                            Result = Result.SUCCESS,
                            Data = new { username = adUser.SignInNames[0].Value, id = adUser.ObjectId }
                        });
                    }
                    else
                    {
                        return Ok(new ResultViewModel()
                        {
                            Result = Result.ERROR,
                            Message = "Something went wrong"
                        });
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return BadRequest(new ResultViewModel()
            {
                Result = Result.ERROR,
                Message = "Bad request"
            });
        }

        //[HttpGet]
        //public IActionResult SignedOut()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // Redirect to home page if the user is authenticated.
        //        return RedirectToAction(nameof(HomeController.Index), "Home");
        //    }

        //    return View();
        //}

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
