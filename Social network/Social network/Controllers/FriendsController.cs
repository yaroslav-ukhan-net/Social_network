using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services;
using Social_network.Data;
using Social_network.ViewModels;
using System;
using System.Linq;


namespace Social_network.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly UserService _userService;
        private readonly FriendService _friendService;
        private readonly UserManager<AppUser> _userManager;

        public FriendsController(UserService userService, FriendService friendService, UserManager<AppUser> userManager)
        {
            _userService = userService;
            _friendService = friendService;
            _userManager = userManager;
        }

        //Get:Friends/СonfirmedFriends/
        [HttpGet]
        public IActionResult DoingsWithFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ? 
                id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            ViewData["Doing"] = "DoingsWithFriends";
            var userMain = _userService.GetUserById(id);
            if (userMain == null) return NotFound();

            FriendsViewModel model = new();
            model.Id = id;
            model.Name = userMain.Name;
            model.Surname = userMain.Surname;
            model.AvatarURL = userMain.AvatarURL;
            model.FriendsListForModel = new();

            var userFirstInFrienship = _friendService.GetAllFriendsQuerible(c =>
               ((c.Status == (int)StatusFriendship.areFriends) &&
               (c.FirstFriendId == id))).ToList();
            foreach (var u in userFirstInFrienship)
            {
                model.FriendsListForModel.Add(new FriendsListViewModel()
                {
                    Id = u.SecondFriendId,
                    Name = u.SecondFriend.Name,
                    AvatarURL = u.SecondFriend.AvatarURL,
                    Surname = u.SecondFriend.Surname
                });
            }

            var userSecondInFrienship = _friendService.GetAllFriendsQuerible(c =>
               ((c.Status == (int)StatusFriendship.areFriends) &&
               (c.SecondFriendId == id))).ToList();
            foreach (var u in userSecondInFrienship)
            {
                model.FriendsListForModel.Add(new FriendsListViewModel()
                {
                    Id = u.FirstFriend.Id,
                    Name = u.FirstFriend.Name,
                    AvatarURL = u.FirstFriend.AvatarURL,
                    Surname = u.FirstFriend.Surname
                });
            }

            return View(model);
        }

        //Get:Friends/SendFriends/id
        [HttpGet]
        public IActionResult SendFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            ViewData["Doing"] = "SendFriends";
            var mainUser = _userService.GetUserById(id);
            if (mainUser == null)
            {
                return NotFound();
            }
            var userRequestsQuery = _friendService.GetAllFriendsQuerible(f=>
                        f.Status == (int)StatusFriendship.requestToFriendship && 
                        f.FirstFriendId==id);

            var userRequestsList = userRequestsQuery.ToList();
            FriendsViewModel model = new();
            model.Id = id;
            model.Name = mainUser.Name;
            model.Surname = mainUser.Surname;
            model.AvatarURL = mainUser.AvatarURL;
            model.FriendsListForModel = new();
            foreach (var u in userRequestsList)
            {
                model.FriendsListForModel.Add(new FriendsListViewModel()
                {
                    Id = u.SecondFriendId,
                    Name = u.SecondFriend.Name,
                    AvatarURL = u.SecondFriend.AvatarURL,
                    Surname = u.SecondFriend.Surname
                });
            }
            return View("DoingsWithFriends", model);
        }
        //Get:Friends/RequestFriends/id
        [HttpGet]
        public IActionResult RequestFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            ViewData["Doing"] = "RequestFriends";
            var mainUser = _userService.GetUserById(id);
            if (mainUser == null)
            {
                return NotFound();
            }
            var requestsToUserQuery = _friendService.GetAllFriendsQuerible(f =>
            f.Status == (int)StatusFriendship.requestToFriendship &&
            f.SecondFriendId == id);

            var requestsToUserList = requestsToUserQuery.ToList();
            FriendsViewModel model = new();
            model.Id = id;
            model.Name = mainUser.Name;
            model.Surname = mainUser.Surname;
            model.AvatarURL = mainUser.AvatarURL;
            model.FriendsListForModel = new();
            foreach (var u in requestsToUserList)
            {
                model.FriendsListForModel.Add(new FriendsListViewModel()
                {
                    Id = u.FirstFriend.Id,
                    Name = u.FirstFriend.Name,
                    AvatarURL = u.FirstFriend.AvatarURL,
                    Surname = u.FirstFriend.Surname
                });
            }
            return View("DoingsWithFriends", model);
        }

        //Get:Friends/AddFreeFriends/id
        [HttpGet]
        public IActionResult AddFreeFriends()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var MainUser = _userService.GetUserById(id);
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

            var usersWithFriendshipList = _friendService.GetAllFriendsQuerible(f =>
                            (f.Status == (int)StatusFriendship.requestToFriendship || f.Status == (int)StatusFriendship.areFriends) &&
                            ((f.FirstFriendId == id) || (f.SecondFriendId == id))).AsEnumerable();

            var FreeFriends = _userService.GetAllUsersQuerible(us => 
                        !(usersWithFriendshipList.Any(fr => fr.FirstFriend == us || fr.SecondFriend == us) || us.Id == id)).ToList();

            foreach (var user in FreeFriends)
            {
                model.FriendsListForModel.Add(new FriendsListViewModel
                {
                    Id = user.Id,
                    AvatarURL = user.AvatarURL,
                    Name = user.Name,
                    Surname = user.Surname
                });
            }
            return View(model);
        }

        //Get:Friends/AddFriend/id
        [HttpGet]
        public IActionResult AddFriend(int userToAddId, string returnUrl)
        {
            if (_userService.GetUserById(userToAddId) == null) return BadRequest();

            int myUser = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                myUser = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();

            _friendService.CreateFriend(new Friend() { FirstFriendId = myUser, SecondFriendId = userToAddId, Status = 1 });

            return LocalRedirect(returnUrl);
        }
        //Get:Friends/DeleteFriend/id
        [HttpGet]
        public IActionResult DeleteFriend(int userToDeleteId, string returnUrl)
        {
            int myUser = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                myUser = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var existFrienship = _friendService.GetFriendsByTwoId(myUser, userToDeleteId);
            if (existFrienship == null)
            {
                _friendService.UpdateFriend(new() { FirstFriendId = userToDeleteId, SecondFriendId = myUser, Status = 1 });
            }
            else
            {
                _friendService.DeleteFriendentity(existFrienship);
                _friendService.CreateFriend(new()
                {
                    FirstFriendId = userToDeleteId,
                    SecondFriendId = myUser,
                    Status = 1
                });
            }
            return LocalRedirect(returnUrl);
        }
        //ViewData["Action"] = "CancelFriendship";
        [HttpGet]
        public IActionResult CancelRequestFriendship( int UserRequest, string returnUrl)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            Friend CancelFriendship = new();
            CancelFriendship.FirstFriendId = UserRequest;
            CancelFriendship.SecondFriendId = UserSender;
            _friendService.DeleteFriendentity(CancelFriendship);
            return LocalRedirect(returnUrl);
        }
        [HttpGet]
        public IActionResult CancelSendFriendship(int UserRequest, string returnUrl)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            Friend CancelFriendship = new();
            CancelFriendship.FirstFriendId = UserSender;
            CancelFriendship.SecondFriendId = UserRequest;
            _friendService.DeleteFriendentity(CancelFriendship);
            return LocalRedirect(returnUrl);
        }
        [HttpGet]
        public IActionResult AcceptNewFriend(int UserRequest, string returnUrl)
        {
            int UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                UserSender = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            _friendService.UpdateFriend(new Friend() { FirstFriendId = UserRequest, SecondFriendId = UserSender, Status = 2 });
            return LocalRedirect(returnUrl);
        }
    }
}
