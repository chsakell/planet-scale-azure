using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Online.Store.Models;

namespace Online.Store.Controllers
{
    public class HomeController : Controller
    {
        IConfiguration _configuration;
        //private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            //_signInManager = signInManager;
        }
        public IActionResult Index()
        {
            ViewData.Add("Region", _configuration["Region"]);
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> Signout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}
    }
}
