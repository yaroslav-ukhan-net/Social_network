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
using Microsoft.AspNetCore.Http; //testComit try start in main Branch

namespace Social_network.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<AppUser> _userManager;

        const string OwnerPagePolicy = "OwnerPagePolicy";


        public UserController(
            UserService userService, 
            PostService postService, 
            IAuthorizationService authorizationService, 
            UserManager<AppUser> userManager
            )
        {
            _userService = userService;
            _postService = postService;
            _userManager = userManager;
            _authorizationService = authorizationService;
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
            var User = _userService.GetUserById(id);
            if (User == null) return NotFound();

            UserViewModel model = new()
            {
                Id = id,
                Name = User.Name,
                Notes = User.Notes,
                BirthDate = User.BirthDate,
                PhoneNumber = User.PhoneNumber,
                Surname = User.Surname,
                AvatarURL = User.AvatarURL,
                Email = User.Email,
                Posts = new List<PostViewModel>()
            };

            var sortedPosts = _postService.GetAllPotsQuerible(post=> post.UserId == id).OrderByDescending(post=>post.PostTime);

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

            return View(model);
        }

        //Post post
        [HttpPost]
        public IActionResult Send (UserViewModel sendPost)
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
            return RedirectToAction("MyPage","User");
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
                 PostTime=postView.PostTime,
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
    }
}
