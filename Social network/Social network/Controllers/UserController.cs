using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using Models;
//using Models.Models;
using Services;
using Social_network.Models;
using Social_network.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Models.Models;
using Microsoft.AspNetCore.Identity;
using Social_network.Data;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using SocialNetwork.CheckingAccess;
using SocialNetwork.ViewModels;

namespace Social_network.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly FriendService _friendService;
        private readonly GroupService _groupService;

        const string OwnerPagePolicy = "OwnerPagePolicy";


        public UserController(
            UserService userService,
            PostService postService,
            IAuthorizationService authorizationService,
            UserManager<AppUser> userManager,
            FriendService friendService,
            GroupService groupService
            )
        {
            _userService = userService;
            _postService = postService;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _friendService = friendService;
            _groupService = groupService;
        }

        [HttpGet]
        public IActionResult MyPage()
        {

            


            int userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            return RedirectToAction("UserPage", "User", new { id = userId });
        }

        //Get:User/UserPage/id
        [HttpGet]
        public IActionResult UserPage(int id)
        {
            bool myFriend = false;
            int myUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                myUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var userPage = _userService.GetUserById(id);
            if (userPage == null) return NotFound();



            UserViewModel model = new()
            {
                Id = id,
                Name = userPage.Name,
                Notes = userPage.Notes,
                BirthDate = userPage.BirthDate,
                PhoneNumber = userPage.PhoneNumber,
                Surname = userPage.Surname,
                AvatarURL = userPage.AvatarURL,
                Email = userPage.Email,
                Posts = new List<PostViewModel>(),
                pageAccess = new()
            };


            model.friendshipStatus = CheckingAccess.CheckFriendship(myUserId, id, _friendService);
            if (model.friendshipStatus == (int)friendshipStatusEnum.areFriends)
            {
                myFriend = true;
            }
            model.pageAccess = CheckingAccess.CheckAccess(myUserId, id, userPage, myFriend);

            if (model.pageAccess.AccessSeeMyPosts) //access to see posts
            {
                var sortedPosts = _postService.GetAllPotsQuerible(post => post.UserId == id).OrderByDescending(post => post.PostTime);
                foreach (var post in sortedPosts.ToList())
                {
                    model.Posts.Add(new PostViewModel()
                    {
                        Id = post.Id,
                        UserId = post.UserId,
                        Text = post.Text,
                        PostTime = post.PostTime
                    });
                }
            }

            return View(model);
        }

        //Post post
        [HttpPost]
        public IActionResult Send(UserViewModel sendPost)
        {
            int userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            if (sendPost.NewPost == null) throw new NullReferenceException();
            if (sendPost.NewPost.Text == null)
            {
                return RedirectToAction("UserPage", new { id = sendPost.NewPost.UserId });
            }
            else
            {
                sendPost.NewPost.PostTime = DateTime.Now;
                sendPost.NewPost.UserId = userId;
                _postService.CreatePost(ToModel(sendPost.NewPost));
                return RedirectToAction("UserPage", new { id = sendPost.NewPost.UserId });
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditMyInformation()
        {
            int userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var MyUserInformation = _userService.GetUserById(userId);
            var result = await _authorizationService.AuthorizeAsync(User, MyUserInformation, OwnerPagePolicy);
            if (result.Succeeded)
            {
                return View(ToViewModel(MyUserInformation));
            }
            return Forbid();
        }

        [HttpPost]
        public IActionResult EditMyInformation(UserViewModel userView)
        {
            _userService.UpdateUser(ToModel(userView));
            return RedirectToAction("MyPage", "User");
        }

        [HttpGet]
        public IActionResult DeletePost(int PostId) //remove post
        {
            int userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                userId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var MyUser = _userService.GetUserById(userId);
            var PostForRemoving = _postService.GetPostsById(PostId);

            if (PostForRemoving.UserId == userId)
            {
                _postService.DeletePost(PostId);
                return RedirectToAction("MyPage");
            }
            return Forbid();
        }

        private UserViewModel ToViewModel(User user)
        {
            return new UserViewModel()
            {
                Id = user.Id,
                AvatarURL = user.AvatarURL,
                BirthDate = user.BirthDate,
                Name = user.Name,
                Notes = user.Notes,
                PhoneNumber = user.PhoneNumber,
                Surname = user.Surname,
                Email = user.Email
            };
        }
        private Post ToModel(Post postView)
        {
            return new Post()
            {
                PostTime = postView.PostTime,
                Text = postView.Text,
                UserId = postView.UserId
            };
        }
        private User ToModel(UserViewModel userView)
        {
            return new User()
            {
                Id = userView.Id,
                AvatarURL = userView.AvatarURL,
                BirthDate = userView.BirthDate,
                Name = userView.Name,
                PhoneNumber = userView.PhoneNumber,
                Surname = userView.Surname,
                Notes = userView.Notes,
                Email = userView.Email
            };
        }
        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            culture = "uk-UA";
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            //return LocalRedirect(returnUrl);
            return RedirectToAction("MyPage");
        }
        [HttpGet]
        public IActionResult UserFriends(int userPageId)
        {
            int myId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                myId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var pageUser = _userService.GetUserById(userPageId);
            if (pageUser == null) return NotFound();


            FriendsViewModel model = new();
            model.Id = userPageId;
            model.Name = pageUser.Name;
            model.Surname = pageUser.Surname;
            model.AvatarURL = pageUser.AvatarURL;
            model.FriendsListForModel = new();


            int friendshipStatus = CheckingAccess.CheckFriendship(myId, userPageId, _friendService);
            bool myFriend = false;
            if (friendshipStatus == (int)friendshipStatusEnum.areFriends)
            {
                myFriend = true;
            }
            PageAccess pageAccess = CheckingAccess.CheckAccess(myId, userPageId, pageUser, myFriend);
            if (!pageAccess.AccessSeeMyFriends)
            {
                return View(model);
            }


            var userFirstInFrienship = _friendService.GetAllFriendsQuerible(c =>
               ((c.Status == (int)StatusFriendship.areFriends) &&
               (c.FirstFriendId == userPageId))).ToList();
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
               (c.SecondFriendId == userPageId))).ToList();
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
        [HttpGet]
        public IActionResult UserGroups(int userPageId)
        {
            int myId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId != 0 ?
                myId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId : throw new NullReferenceException();
            var pageUser = _userService.GetUserById(userPageId);
            if (pageUser == null) return NotFound();


            GroupViewModel model = new();
            model.AvatarURL = pageUser.AvatarURL;
            model.Surname = pageUser.Surname;
            model.Name = pageUser.Name;
            model.Id = userPageId;
            model.Groups = new();


            int friendshipStatus = CheckingAccess.CheckFriendship(myId, userPageId, _friendService);
            bool myFriend = false;
            if (friendshipStatus == (int)friendshipStatusEnum.areFriends)
            {
                myFriend = true;
            }
            PageAccess pageAccess = CheckingAccess.CheckAccess(myId, userPageId, pageUser, myFriend);
            if (!pageAccess.AccessSeeMyGroups)
            {
                return View(model);
            }

            var pageGroups = _groupService.GetAllGroupsQuerible(g => g.UserGroup.Any(us => us.User == pageUser && us.ConsistInGroup)).ToList();

            foreach (var group in pageGroups)
            {
                model.Groups.Add(new GroupsViewModel()
                {
                    GroupAvatarURL = group.AvatarURL,
                    GroupCountFollowers = group.CountFollowers,
                    GroupId = group.Id,
                    GroupName = group.Name
                });
            }


            return View(model);
        }

    }
}
