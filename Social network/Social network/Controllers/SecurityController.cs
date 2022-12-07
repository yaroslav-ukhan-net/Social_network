using Data_SocialNetwork.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Models.Models;
using NETCore.MailKit.Core;
using Services;
using Social_network.Data;
using Social_network.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Social_network.Controllers
{
    public class SecurityController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserService _userService;

        public SecurityController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var User = _userManager.Users.SingleOrDefault(p => p.Email == model.Email);
                    if (User != null)
                    {
                        return RedirectToAction("UserPage", "User", new { id = User.AppUserId });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login or password");
                    return View(model);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(new AppUser(model.Email) { Email = model.Email }, model.Password);
                if (result.Succeeded)
                {
                    var User = _userManager.Users.SingleOrDefault(p => p.Email == model.Email);
                    _userService.CreateUser(new User()
                    {
                        AvatarURL = "https://i.imgur.com/7wwkYMN.png",
                        BirthDate = model.BirthDate,
                        Name = model.Name,
                        PhoneNumber = model.PhoneNumber,
                        Surname = model.Surname,
                        Email = model.Email,
                        Id = User.AppUserId
                    });

                    return RedirectToAction("UserPage", "User", new { id = User.AppUserId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid create User");
                    return View(model);
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Security");
        }
        [HttpPost]
        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                 new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return LocalRedirect(returnUrl);
        }
    }
}
