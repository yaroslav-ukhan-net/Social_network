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
    public class GroupController : Controller
    {
        private readonly UserService _UserService;
        private readonly GroupService _GroupService;
        private readonly PostService _PostService;
        private readonly UserManager<AppUser> _userManager;

        public GroupController(UserService userService, GroupService groupService, PostService postService, UserManager<AppUser> userManager)
        {
            _UserService = userService;
            _GroupService = groupService;
            _PostService = postService;
            _userManager = userManager;
        }
        //Get:Group/MyGroups/id
        [Authorize]
        [HttpGet]
        public IActionResult MyGroups()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["PageName"] = "MyGroups";
            var AllGroups = _GroupService.GetAllGroups();
            var MyUser = _UserService.GetUserById(id);
            var model = new UserGroupsViewModel();

            if (MyUser == null)
            {
                return BadRequest();
            }

            model.Id = MyUser.Id;
            model.Name = MyUser.Name;
            model.Surname = MyUser.Surname;
            model.AvatarURL = MyUser.AvatarURL;
            model.Groups = new List<GroupsViewModel>();

            foreach (var group in AllGroups)
            {
                if (MyUser.UserGroup.Any(g => g.GroupId == group.Id))
                {
                    model.Groups.Add(new GroupsViewModel()
                    {
                        GroupAdminId = group.AdminId,
                        GroupAvatarURL = group.AvatarURL,
                        GroupCountFollowers = group.CountFollowers,
                        GroupCreateDate = group.CreateDate,
                        GroupId = group.Id,
                        GroupName = group.Name,
                        GroupNotes = group.Notes
                    });
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateGroup()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["Action"] = "CreateGroup";
            var model = new GroupsViewModel();
            model.GroupAdminId = id;

            return View("EditGroup",model);
        }
        [HttpPost]
        public IActionResult CreateGroup(GroupsViewModel groupsView)
        {
            Group newgroup = new Group()
            {
                AdminId = groupsView.GroupAdminId,
                AvatarURL = groupsView.GroupAvatarURL,
                CountFollowers = 1,
                Name = groupsView.GroupName,
                Notes = groupsView.GroupNotes,
                CreateDate = DateTime.Now,
                IsClose = groupsView.IsClose
            };
            _GroupService.CreateGroup(newgroup);

            var myuser = _UserService.GetUserById(groupsView.GroupAdminId);
            var asd = new UserGroup() { Group = newgroup, User = myuser, IsSubscribed = true };
            myuser.UserGroup.Add(asd);

            _UserService.UpdateUser(myuser);

            return RedirectToAction("MyGroups", new { Id = groupsView.GroupAdminId });
        }

        [HttpGet]
        public IActionResult NewGroups()
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["PageName"] = "NewGroups";
            var AllGroups = _GroupService.GetAllGroups();
            var MyUser = _UserService.GetUserById(id);
            var model = new UserGroupsViewModel();

            if (MyUser == null)
            {
                return BadRequest();
            }

            model.Id = MyUser.Id;
            model.Name = MyUser.Name;
            model.Surname = MyUser.Surname;
            model.AvatarURL = MyUser.AvatarURL;
            model.Groups = new List<GroupsViewModel>();

            foreach (var group in AllGroups)
            {
                if (!(MyUser.UserGroup.Any(g => g.GroupId == group.Id)))
                {
                    model.Groups.Add(new GroupsViewModel()
                    {
                        GroupAdminId = group.AdminId,
                        GroupAvatarURL = group.AvatarURL,
                        GroupCountFollowers = group.CountFollowers,
                        GroupCreateDate = group.CreateDate,
                        GroupId = group.Id,
                        GroupName = group.Name,
                        GroupNotes = group.Notes,
                        IsClose = group.IsClose
                    });
                }
            }
            return View("MyGroups", model);
        }
        [HttpGet]
        public IActionResult AddGroup(int AddGroupId)
        {
            int MyUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            //if(admin)
            //_UserService.UserUnsubscribe(MyUserId, AddGroupId);
            return RedirectToAction("MyGroups", MyUserId);
        }
        [HttpGet]
        public IActionResult GroupPage(int GroupPageId)
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var ChekError = _UserService.GetUserById(UserId);
            if (ChekError == null)
            {
                return BadRequest();
            }
            var mygroup = _GroupService.GetGroupById(GroupPageId);
            if (mygroup == null)
            {
                return BadRequest();
            }
            var AllPosts = _PostService.GetAllPosts();
            GroupsViewModel model = new();
            model.Id = UserId;
            model.GroupAdminId = mygroup.AdminId;
            model.GroupAvatarURL = mygroup.AvatarURL;
            model.GroupCountFollowers = mygroup.CountFollowers;
            model.GroupCreateDate = mygroup.CreateDate;
            model.GroupId = GroupPageId;
            model.GroupName = mygroup.Name;
            model.GroupNotes = mygroup.Notes;
            model.IsClose = mygroup.IsClose;
            model.Posts = new List<PostViewModel>();
            model.Users = new List<User>();
            foreach (var user in mygroup.UserGroup)
            {
                var GroupUsers = mygroup.UserGroup.Any(u => u.GroupId == GroupPageId);
                if (GroupUsers)
                {
                    model.Users.Add(new User()
                    {
                        Name = user.User.Name,
                        Surname = user.User.Surname,
                        AvatarURL = user.User.AvatarURL,
                        Id = user.User.Id,
                    });
                }
            }
            foreach (var post in AllPosts)
            {
                if (post.GroupId == GroupPageId)
                {
                    var UserPost = _UserService.GetUserById(post.UserId);
                    model.Posts.Add(new PostViewModel()
                    {
                        Id = post.Id,
                        UserId = post.UserId,
                        Text = post.Text,
                        PostTime = post.PostTime,
                        PostSenderName = UserPost.Name + " " + UserPost.Surname,
                        PostSenderAvatar = UserPost.AvatarURL
                    });
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult EditGroup(int Groupid)
        {
            int MyUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["Action"] = "EditGroup";
            var MyGroupInformation = _GroupService.GetGroupById(Groupid);
            if (MyGroupInformation == null)
            {
                return BadRequest();
            }
            GroupsViewModel model = new();
            model.GroupNotes = MyGroupInformation.Notes;
            model.GroupAvatarURL = MyGroupInformation.AvatarURL;
            model.GroupName = MyGroupInformation.Name;
            model.IsClose = MyGroupInformation.IsClose;
            
            return View(model);
        }

        [HttpPost]
        public IActionResult EditGroup(GroupsViewModel group)
        {
            var groupfromBD = _GroupService.GetGroupById(group.Id);
            if(groupfromBD == null)
            {
                return BadRequest();
            }
            groupfromBD.Notes = group.GroupNotes;
            groupfromBD.Name = group.GroupName;
            groupfromBD.IsClose = group.IsClose;
            groupfromBD.AvatarURL = group.GroupAvatarURL;
            _GroupService.UpdateGroup(groupfromBD);
            return RedirectToAction("GroupPage", new { UserId = 1, GroupPageId = group.Id});
        }
        [HttpGet]
        public IActionResult DeleteGroup(int id)
        {
            _GroupService.RemoveGroupById(id);
            return RedirectToAction("MyGroups", new { id = 1 }) ;
        }
        [HttpGet]
        public IActionResult UnsubscribeFromGroup(int id)
        {
            int UserId = 1;
            _UserService.UserUnsubscribe(1, id);
            return RedirectToAction("MyGroups", new { id = UserId });
        }
        [HttpGet]
        public IActionResult RequestJoinGroup(int id)
        {
            int UserId = 1;

            return RedirectToAction("GroupPage", new { UserId = UserId, GroupPageId = id });
        }
    }
}
