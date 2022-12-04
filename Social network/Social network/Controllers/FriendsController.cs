using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Models;
using Services;
using Social_network.Data;
using Social_network.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly UserService _userService;
        private readonly FriendService _friendService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<AppUser> _userManager;

        public FriendsController(UserService userService, FriendService friendService, IAuthorizationService authorizationService, UserManager<AppUser> userManager)
        {
            _userService = userService;
            _friendService = friendService;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        //Get:Friends/СonfirmedFriends/
        [HttpGet]
        public IActionResult DoingsWithFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["PageName"] = "Мои друзья";
            ViewData["Doing"] = "DoingsWithFriends";
            var Main_user = _userService.GetUserById(id);
            if (Main_user == null)
            {
                return NotFound();
            }
            var AllFriends = _friendService.GetAllFriends();
            FriendsViewModel model = new();
            model.Id = id;
            model.Name = Main_user.Name;
            model.Surname = Main_user.Surname;
            model.AvatarURL = Main_user.AvatarURL;
            model.FriendsListForModel = new();
            foreach (var u in AllFriends)
            {
                if (u.Status == 2)
                {
                    if (u.Friend_oneId == id)
                    {
                        model.FriendsListForModel.Add(new FriendsListViewModel()
                        {
                            Id = u.Friend_twoId,
                            Name = u.Friend_two.Name,
                            AvatarURL = u.Friend_two.AvatarURL,
                            Surname = u.Friend_two.Surname
                        });
                    }
                    if (u.Friend_twoId == id)
                    {
                        model.FriendsListForModel.Add(new FriendsListViewModel()
                        {
                            Id = u.Friend_one.Id,
                            Name = u.Friend_one.Name,
                            AvatarURL = u.Friend_one.AvatarURL,
                            Surname = u.Friend_one.Surname
                        });
                    }
                }
            }
            return View(model);
        }

        //Get:Friends/SendFriends/id
        [HttpGet]
        public IActionResult SendFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["PageName"] = "Отправленные заявки";
            ViewData["Doing"] = "SendFriends";
            var Main_user = _userService.GetUserById(id);
            if (Main_user == null)
            {
                return NotFound();
            }
            var AllFriends = _friendService.GetAllFriends();
            FriendsViewModel model = new();
            model.Id = id;
            model.Name = Main_user.Name;
            model.Surname = Main_user.Surname;
            model.AvatarURL = Main_user.AvatarURL;
            model.FriendsListForModel = new();
            foreach (var u in AllFriends)
            {
                if (u.Status == 1)
                {
                    if (u.Friend_oneId == id)
                    {
                        model.FriendsListForModel.Add(new FriendsListViewModel()
                        {
                            Id = u.Friend_twoId,
                            Name = u.Friend_two.Name,
                            AvatarURL = u.Friend_two.AvatarURL,
                            Surname = u.Friend_two.Surname
                        });
                    }
                }
            }
            return View("DoingsWithFriends", model);
        }
        //Get:Friends/RequestFriends/id
        [HttpGet]
        public IActionResult RequestFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["PageName"] = "Полученные заявки";
            ViewData["Doing"] = "RequestFriends";
            var Main_user = _userService.GetUserById(id);
            if (Main_user == null)
            {
                return NotFound();
            }
            var AllFriends = _friendService.GetAllFriends();
            FriendsViewModel model = new();
            model.Id = id;
            model.Name = Main_user.Name;
            model.Surname = Main_user.Surname;
            model.AvatarURL = Main_user.AvatarURL;
            model.FriendsListForModel = new();
            foreach (var u in AllFriends)
            {
                if (u.Status == 1)
                {
                    if (u.Friend_twoId == id)
                    {
                        model.FriendsListForModel.Add(new FriendsListViewModel()
                        {
                            Id = u.Friend_one.Id,
                            Name = u.Friend_one.Name,
                            AvatarURL = u.Friend_one.AvatarURL,
                            Surname = u.Friend_one.Surname
                        });
                    }
                }
            }
            return View("DoingsWithFriends", model);
            //return View("СonfirmedFriends", model);
        }

        //Get:Friends/AddFreeFriends/id
        [HttpGet]
        public IActionResult AddFreeFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MainUser = _userService.GetUserById(id);
            var AllUsers = _userService.GetAllUsers();
            var AllFriends = _friendService.GetAllFriends();

            if (MainUser == null)
            {
                return BadRequest();
            }

            FriendsViewModel model = new();
            model.Id = id;
            model.Name = MainUser.Name;
            model.Surname = MainUser.Surname;
            model.AvatarURL = MainUser.AvatarURL;
            model.FriendsListForModel = new();

            List<FriendsListViewModel> userwithfriends = new();

            foreach (var u in AllFriends)
            {
                if (u.Status == 1 || u.Status == 2)
                {
                    if (u.Friend_oneId == id)
                    {
                        userwithfriends.Add(new FriendsListViewModel()
                        {
                             Id = u.Friend_twoId,
                            Name = u.Friend_two.Name,
                            AvatarURL = u.Friend_two.AvatarURL,
                            Surname = u.Friend_two.Surname
                        });
                    }
                    if (u.Friend_twoId == id)
                    {
                        userwithfriends.Add(new FriendsListViewModel()
                        {
                             Id = u.Friend_one.Id,
                            Name = u.Friend_one.Name,
                            AvatarURL = u.Friend_one.AvatarURL,
                            Surname = u.Friend_one.Surname
                        });
                    }
                }
            }
            foreach (var withfr in AllUsers)
            {
                if (!(userwithfriends.Any(u => u.Id == withfr.Id) || withfr.Id == id))
                {
                    model.FriendsListForModel.Add(new FriendsListViewModel
                    {
                         Id = withfr.Id,
                        AvatarURL = withfr.AvatarURL,
                        Name = withfr.Name,
                        Surname = withfr.Surname
                    });
                }
            }
            return View(model);
        }

        //Get:Friends/AddFriend/id
        [HttpGet]
        public IActionResult AddFriend(int UserRequest)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            _friendService.CreateFriend(new Friend() { Friend_oneId = UserSender, Friend_twoId = UserRequest, Status = 1 });
            return RedirectToAction("AddFreeFriends");
        }
        //Get:Friends/DeleteFriend/id
        [HttpGet]
        public IActionResult DeleteFriend(int UserRequest)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var existFrienship = _friendService.GetFriendsByTwoId(UserSender, UserRequest);
            if (existFrienship == null)
            {
                _friendService.UpdateFriend(new() { Friend_oneId = UserRequest, Friend_twoId = UserSender, Status = 1 });
            }
            else
            {
                _friendService.DeleteFriendentity(existFrienship);
                _friendService.CreateFriend(new()
                {
                    Friend_oneId = UserRequest,
                    Friend_twoId = UserSender,
                    Status = 1
                });
            }
            return RedirectToAction("DoingsWithFriends", "Friends");
        }
        //ViewData["Action"] = "CancelFriendship";
        [HttpGet]
        public IActionResult CancelRequestFriendship( int UserRequest)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            Friend CancelFriendship = new();
            CancelFriendship.Friend_oneId = UserRequest;
            CancelFriendship.Friend_twoId = UserSender;
            _friendService.DeleteFriendentity(CancelFriendship);
            return RedirectToAction("RequestFriends", "Friends");
        }
        [HttpGet]
        public IActionResult CancelSendFriendship(int UserRequest)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            Friend CancelFriendship = new();
            CancelFriendship.Friend_oneId = UserSender;
            CancelFriendship.Friend_twoId = UserRequest;
            _friendService.DeleteFriendentity(CancelFriendship);
            return RedirectToAction("SendFriends", "Friends");
        }
        [HttpGet]
        public IActionResult AcceptNewFriend(int UserRequest)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            _friendService.UpdateFriend(new Friend() { Friend_oneId = UserRequest, Friend_twoId = UserSender, Status = 2 });
            return RedirectToAction("RequestFriends", "Friends");
        }
    }
}
