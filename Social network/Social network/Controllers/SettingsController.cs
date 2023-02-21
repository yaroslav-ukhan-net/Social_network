using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Social_network.Data;
using System.Linq;
using System;
using SocialNetwork.ViewModels;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SettingsService _settingsService ;
        private readonly UserService _userService;

        public SettingsController(SettingsService settingsService, UserManager<AppUser> userManager, UserService userService)
        {
            _userManager = userManager;
            _settingsService = settingsService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            int myUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                myUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var mySettings = _userService.GetUserById(5).Settings;

            SettingsVievModel model = new()
            {
                Id = mySettings.Id,
                ChatInvites = mySettings.ChatInvites,
                LeavePosts = mySettings.LeavePosts,
                SeeMyFriends = mySettings.SeeMyFriends,
                SeeMyGroups = mySettings.SeeMyGroups,
                SeeMyPosts = mySettings.SeeMyPosts,
                UserId = mySettings.UserId,
                WriteToMe = mySettings.WriteToMe,
                SeeMyPhone = mySettings.SeeMyPhone
                
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Privacy(SettingsVievModel model)
        {
            int myUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                myUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var myUser = _userService.GetUserById(myUserId);
            
            myUser.Settings.SeeMyGroups = model.SeeMyGroups;
            myUser.Settings.SeeMyPosts = model.SeeMyPosts;
            myUser.Settings.SeeMyFriends = model.SeeMyFriends;
            myUser.Settings.ChatInvites = model.ChatInvites;
            myUser.Settings.LeavePosts = model.LeavePosts;
            myUser.Settings.WriteToMe = model.WriteToMe;
            myUser.Settings.SeeMyPhone = model.SeeMyPhone;

            _userService.UpdateUser(myUser);
            return RedirectToAction("Privacy");
        }
    }
}
