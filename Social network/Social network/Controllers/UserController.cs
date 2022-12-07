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

namespace Social_network.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<AppUser> _userManager;


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
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            return RedirectToAction("UserPage", "User", new { id = id });
        }

        //Get:User/UserPage/id
        [HttpGet]
        public IActionResult UserPage(int id)
        {
            var User = _userService.GetUserById(id);
            if (User == null)
            {
                return NotFound();
            }
            UserViewModel model = new UserViewModel();

            model.Id = id;
            model.Name = User.Name;
            model.Notes = User.Notes;
            model.BirthDate = User.BirthDate;
            model.PhoneNumber = User.PhoneNumber;
            model.Surname = User.Surname;
            model.AvatarURL = User.AvatarURL;
            model.Email = User.Email;
            model.Posts = new List<PostViewModel>();


            var NotSortedAllPosts = _postService.GetAllPosts();
            var SortedAllPosts = from s in NotSortedAllPosts
                                 orderby s.PostTime descending
                                 select s;

            foreach (var post in SortedAllPosts)
            {
                if (post.UserId == id)
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
        public IActionResult Send (UserViewModel sendpost)
        {
            int Userid = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            if (sendpost.NewPost.Text == null)
            {
                return RedirectToAction("UserPage", new { id = sendpost.NewPost.UserId });
            }
            else
            {
                sendpost.NewPost.PostTime = DateTime.Now;
                sendpost.NewPost.UserId = Userid;
                _postService.CreatePost(ToModel(sendpost.NewPost));
                return RedirectToAction("UserPage", new { id = sendpost.NewPost.UserId });
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditMyInformation()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUserInformation = _userService.GetUserById(id);
            var result = await _authorizationService.AuthorizeAsync(User, MyUserInformation, "OwnerPagePolicy");
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
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _userService.GetUserById(UserId);
            var PostForRemoving = _postService.GetPostsById(PostId);

            if (PostForRemoving.UserId == UserId)
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
