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
    public class GroupController : Controller
    {
        private readonly UserService _UserService;
        private readonly GroupService _GroupService;
        private readonly PostService _PostService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public GroupController(UserService userService,
            GroupService groupService,
            PostService postService,
            UserManager<AppUser> userManager,
             IAuthorizationService authorizationService)
        {
            _UserService = userService;
            _GroupService = groupService;
            _PostService = postService;
            _userManager = userManager;
            _authorizationService = authorizationService;
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

            model.Groups = new List<GroupsViewModel>();

            foreach (var group in AllGroups)
            {
                if (MyUser.UserGroup.Any(g => g.GroupId == group.Id && g.ConsistInGroup))
                {
                    model.Groups.Add(new GroupsViewModel()
                    {

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
            model.GroupAvatarURL = "https://i.imgur.com/CZYVK0J.png";
            return View("EditGroup", model);
        }
        [HttpPost]
        public IActionResult CreateGroup(GroupsViewModel groupsView)
        {
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            Group newgroup = new Group()
            {
                AdminId = id,
                AvatarURL = groupsView.GroupAvatarURL,
                CountFollowers = 1,
                Name = groupsView.GroupName,
                Notes = groupsView.GroupNotes,
                CreateDate = DateTime.Now,
                IsClose = groupsView.IsClose
            };
            _GroupService.CreateGroup(newgroup);

            var myuser = _UserService.GetUserById(id);
            var newUserGroup = new UserGroup() { Group = newgroup, User = myuser, IsModerator = true, ConsistInGroup = true };
            myuser.UserGroup.Add(newUserGroup);

            _UserService.UpdateUser(myuser);

            return RedirectToAction("MyGroups");
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

            model.Groups = new List<GroupsViewModel>();

            foreach (var group in AllGroups)
            {
                if (!(MyUser.UserGroup.Any(g => g.GroupId == group.Id)))
                {
                    model.Groups.Add(new GroupsViewModel()
                    {
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
        public IActionResult GroupPage(int GroupId)
        {
            ViewData["ActionCloseGroup"] = "RequestJoinGroup";
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var UserFromReposit = _UserService.GetUserById(UserId);
            if (UserFromReposit == null)
            {
                return BadRequest();
            }
            var mygroup = _GroupService.GetGroupById(GroupId);
            if (mygroup == null)
            {
                return BadRequest();
            }
            var AllPosts = _PostService.GetAllPosts();
            GroupsViewModel model = new();
            model.GroupAdminName = _UserService.GetUserById(mygroup.AdminId).Email;
            model.GroupAvatarURL = mygroup.AvatarURL;
            model.GroupCountFollowers = mygroup.CountFollowers;
            model.GroupCreateDate = mygroup.CreateDate;
            model.GroupId = GroupId;
            model.GroupName = mygroup.Name;
            model.GroupNotes = mygroup.Notes;
            model.IsClose = mygroup.IsClose;
            model.Posts = new List<PostViewModel>();
            foreach (var user in mygroup.UserGroup)
            {
                var GroupUsers = user.ConsistInGroup;
                if (GroupUsers)
                {
                    if (user.UserId == UserId)
                    {
                        model.UserIsModerator = user.IsModerator;
                        model.MyUserConsistInGroup = user.ConsistInGroup;
                    }
                }
                else
                {
                    if (user.UserId == UserId)//Отправил заявку в закрытую группу?
                    {
                        ViewData["ActionCloseGroup"] = "UnsubscribeFromGroup";
                    }
                }
            }
            foreach (var post in AllPosts) //add all Group Posts
            {
                if (post.GroupId == GroupId)
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
        public async Task<IActionResult> EditGroup(int id)
        {
            int MyUserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            ViewData["Action"] = "EditGroup";
            var MyGroupInformation = _GroupService.GetGroupById(id);

            GroupsViewModel model = new();
            model.GroupNotes = MyGroupInformation.Notes;
            model.GroupId = id;
            model.GroupAvatarURL = MyGroupInformation.AvatarURL;
            model.GroupName = MyGroupInformation.Name;
            model.IsClose = MyGroupInformation.IsClose;
            model.GroupAdminName = _UserService.GetUserById(MyGroupInformation.AdminId).Email;
            var UserGroups = _UserService.GetUserById(MyUserId).UserGroup;
            foreach (var gr in UserGroups)
            {
                if (gr.GroupId == id)
                {
                    model.UserIsModerator = gr.IsModerator;
                    break;
                }
            }
            var result = await _authorizationService.AuthorizeAsync(User, model, "OwnerOrModeratorGroupPolicy");
            if (result.Succeeded)
            {
                return View(model);
            }
            return Forbid();
        }

        [HttpPost]
        public async Task<IActionResult> EditGroup(GroupsViewModel group)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Action"] = "EditGroup";
                return View("EditGroup", group);
            }

            var result = await _authorizationService.AuthorizeAsync(User, group, "OwnerOrModeratorGroupPolicy");
            if (result.Succeeded)
            {
                var groupfromBD = _GroupService.GetGroupById(group.GroupId);
                groupfromBD.Notes = group.GroupNotes;
                groupfromBD.Name = group.GroupName;
                groupfromBD.IsClose = group.IsClose;
                groupfromBD.AvatarURL = group.GroupAvatarURL;
                _GroupService.UpdateGroup(groupfromBD);
                return RedirectToAction("GroupPage", new { GroupId = group.GroupId });
            }
            return Forbid();
        }
        [HttpGet]
        public IActionResult DeleteGroup(int id)
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(UserId);
            var group = _GroupService.GetGroupById(id);
            if (group == null)
            {
                return BadRequest();
            }
            if (MyUser.Id == group.AdminId || User.IsInRole("Admin"))
            {
                _GroupService.RemoveGroupById(id);
                return RedirectToAction("MyGroups");
            }
            return Forbid();
        }
        [HttpGet]
        public IActionResult UnsubscribeFromGroup(int id) //отписаться
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var group = _GroupService.GetGroupById(id);
            if (UserId != group.AdminId)
            {
                _UserService.UserUnsubscribe(UserId, id);
                return RedirectToAction("GroupPage", new { GroupId = id });
            }
            return Forbid();
        }
        [HttpGet]
        public IActionResult RequestJoinGroup(int id)           //отправить заявку на вступление в группу
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(UserId);
            var group = _GroupService.GetGroupById(id);
            if (group == null)
            {
                return BadRequest();
            }
            if (group.IsClose)
            {
                UserGroup requestjoin = new() { UserId = UserId, GroupId = id, ConsistInGroup = false, IsModerator = false };
                MyUser.UserGroup.Add(requestjoin);
            }
            else
            {
                UserGroup requestjoin = new() { UserId = UserId, GroupId = id, ConsistInGroup = true, IsModerator = false };
                MyUser.UserGroup.Add(requestjoin);
                group.CountFollowers++;
                _GroupService.UpdateGroup(group);
            }
            _UserService.UpdateUser(MyUser);
            return RedirectToAction("GroupPage", new { GroupId = id });
        }
        //Post post
        [HttpPost]
        public IActionResult Send(GroupsViewModel groupsViewModel)
        {
            int Userid = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            if (groupsViewModel.NewPost.Text == null)
            {
                return RedirectToAction("GroupPage", new { GroupId = groupsViewModel.GroupId });
            }
            else
            {
                _PostService.CreatePost(new Post()
                {
                    PostTime = DateTime.Now,
                    UserId = Userid,
                    Text = groupsViewModel.NewPost.Text,
                    GroupId = groupsViewModel.GroupId
                });
                return RedirectToAction("GroupPage", new { GroupId = groupsViewModel.GroupId });
            }
        }
        [HttpGet]
        public IActionResult SubmittedRequests()
        {
            ViewData["PageName"] = "Отправленные запросы";
            int id = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var AllGroups = _GroupService.GetAllGroups();
            var MyUser = _UserService.GetUserById(id);
            var model = new UserGroupsViewModel();

            if (MyUser == null)
            {
                return BadRequest();
            }

            model.Groups = new List<GroupsViewModel>();

            foreach (var group in AllGroups)
            {
                if (MyUser.UserGroup.Any(g => g.GroupId == group.Id && !g.ConsistInGroup))
                {
                    model.Groups.Add(new GroupsViewModel()
                    {
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
        public IActionResult ShowSubscribers(int id)
        {
            int Userid = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(Userid);
            var MyGroup = _GroupService.GetGroupById(id);

            if (MyGroup == null) return BadRequest();

            bool UserHaveAccess = false;
                
            foreach(var u in MyUser.UserGroup)
            {
                if(u.GroupId == id){
                    UserHaveAccess = u.ConsistInGroup;
                    break;
                }
            }

            if(MyGroup.IsClose && !UserHaveAccess)
            {
                return Forbid();
            }

            GroupsViewModel model = new();
            model.GroupId = id;
            model.GroupName = MyGroup.Name;
            model.GroupAdminName = _UserService.GetUserById(MyGroup.AdminId).Email;
            model.GroupAvatarURL = MyGroup.AvatarURL;
            model.GroupCountFollowers = MyGroup.CountFollowers;
            model.GroupCreateDate = MyGroup.CreateDate;
            model.GroupNotes = MyGroup.Notes;
            model.Users = new();
            foreach(var sub in MyGroup.UserGroup)
            {
                if (sub.ConsistInGroup) model.Users.Add(sub.User);
            }


            return View(model);
        }
    }
}
