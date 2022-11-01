using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.Models;
using Services;
using Social_network.Models;
using Social_network.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;

        public UserController(UserService userService, PostService postService)
        {
            _userService = userService;
            _postService = postService;
        }

        //Get:User/UserPage/id
        [HttpGet]
        public IActionResult UserPage(int id)
        {
            var User = _userService.GetUserById(id);
            if(User == null)
            {
                return NotFound();
            }
            UserViewModel model = new UserViewModel();

            model.Id = id;
            model.Name = User.Name;
            model.Notes = User.Notes;
            model.PhoneNumber = User.PhoneNumber;
            model.Surname = User.Surname;
            model.AvatarURL = User.AvatarURL;
            model.Posts = new List<PostViewModel>();

            var NotSortedAllPosts = _postService.GetAllPosts();
            var AllPosts = from s in NotSortedAllPosts
                          orderby s.PostTime descending
                           select s;

            foreach(var post in AllPosts)
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
            if (sendpost.NewPost.Text == null)
            {
                return RedirectToAction("UserPage", new { id = sendpost.NewPost.UserId });
            }
            else
            {
                sendpost.NewPost.PostTime = DateTime.Now;
                sendpost.NewPost.UserId = sendpost.Id;
                _postService.CreatePost(ToModel(sendpost.NewPost));
                return RedirectToAction("UserPage", new { id = sendpost.NewPost.UserId });
            }
        }

        //private UserViewModel ToViewModel(User user)
        //{
        //    return new UserViewModel()
        //    {
        //        Id = user.Id,
        //        AvatarURL = user.AvatarURL,
        //        BirthDate = user.BirthDate,
        //        Name = user.Name,
        //        Notes = user.Notes,
        //        PhoneNumber = user.PhoneNumber,
        //        Surname = user.Surname
        //    };
        //}
        private Post ToModel(Post postView)
        {
            return new Post()
            {
                 PostTime=postView.PostTime,
                  Text = postView.Text,
                   UserId = postView.UserId
            };
        }
    }
}
