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
                        GroupNotes = group.Notes,
                        GroupAdminName = _UserService.GetUserById(group.AdminId).Email
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
            model.GroupAdminUsername = _UserService.GetUserById(mygroup.AdminId).Name + " " + _UserService.GetUserById(mygroup.AdminId).Surname;
            model.GroupAdminId = mygroup.AdminId.ToString();
            model.GroupAvatarURL = mygroup.AvatarURL;
            model.GroupCountFollowers = mygroup.CountFollowers;
            model.GroupCreateDate = mygroup.CreateDate;
            model.GroupId = GroupId;
            model.GroupName = mygroup.Name;
            model.GroupNotes = mygroup.Notes;
            model.IsClose = mygroup.IsClose;
            model.GroupCountFollowersRequests = 0;
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
                    model.GroupCountFollowersRequests++;
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
                if(groupfromBD.IsClose && !group.IsClose)    //Close => open group
                {
                    foreach(var u in groupfromBD.UserGroup)
                    {
                        if (!u.ConsistInGroup)
                        {
                            u.ConsistInGroup = true;
                        }
                    }
                }
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
        public IActionResult UnsubscribeFromGroup(int id)     //отписаться
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
                _GroupService.UpdateGroup(group);
            }
            _UserService.UpdateUser(MyUser);
            return RedirectToAction("GroupPage", new { GroupId = id });
        }
        //Post post
        [HttpPost]
        public async Task<IActionResult> Send(GroupsViewModel groupsViewModel) //отправить посты
        {
            int Userid = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(Userid);
            if (groupsViewModel.NewPost.Text == null)
            {
                return RedirectToAction("GroupPage", new { GroupId = groupsViewModel.GroupId });
            }
            else
            {
                var UserGroups = MyUser.UserGroup;
                foreach (var gr in UserGroups)
                {
                    if (gr.GroupId == groupsViewModel.GroupId)
                    {
                        groupsViewModel.UserIsModerator = gr.IsModerator;
                        break;
                    }
                }
                var result = await _authorizationService.AuthorizeAsync(User, groupsViewModel, "OwnerOrModeratorGroupPolicy");
                if (result.Succeeded)
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
                return Forbid();
            }
        }
        [HttpGet]
        public IActionResult SubmittedRequests() //вкладка с исходящими запросами в группы
        {
            ViewData["PageName"] = "Requests";
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
        public IActionResult ShowSubscribers(int id) //показать всех попищиков
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
        [HttpGet]
        public async Task<IActionResult> RequestsInGroup(int id)   // активные запросы в группу
        {
            int Userid = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(Userid);
            var MyGroup = _GroupService.GetGroupById(id);

            if (MyGroup == null || !MyGroup.IsClose) return BadRequest();

            GroupsViewModel model = new();
            model.GroupAdminName = _UserService.GetUserById(MyGroup.AdminId).Name;
            var UserGroups = _UserService.GetUserById(Userid).UserGroup;
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
                model.GroupAdminName = _UserService.GetUserById(MyGroup.AdminId).Email;
                model.GroupAvatarURL = MyGroup.AvatarURL;
                model.GroupCountFollowers = MyGroup.CountFollowers;
                model.GroupCreateDate = MyGroup.CreateDate;
                model.GroupId = id;
                model.GroupName = MyGroup.Name;
                model.Users = new();
                foreach (var u in MyGroup.UserGroup)
                {
                    if (u.ConsistInGroup == false)
                    {
                        model.Users.Add(u.User);
                    }
                }
                return View(model);
            }
            return Forbid();
        }
        [HttpGet]
        public async Task<IActionResult> AddFollowerInGroup(int GroupId,int UserAddId)           //принять чела в группу
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(UserId);
            var group = _GroupService.GetGroupById(GroupId);
            var UserForAdding = _UserService.GetUserById(UserAddId);
            if (group == null || UserForAdding == null || !group.IsClose) return BadRequest();

            GroupsViewModel model = new();
            model.GroupAdminName = _UserService.GetUserById(group.AdminId).Email;
            var UserGroups = MyUser.UserGroup;
            foreach (var gr in UserGroups)
            {
                if (gr.GroupId == GroupId)
                {
                    model.UserIsModerator = gr.IsModerator;
                    break;
                }
            }
            var result = await _authorizationService.AuthorizeAsync(User, model, "OwnerOrModeratorGroupPolicy");
            if (result.Succeeded)
            {
                foreach (UserGroup ug in UserForAdding.UserGroup)
                {
                    if (ug.GroupId == GroupId)
                    {
                        ug.ConsistInGroup = true;
                        _GroupService.UpdateGroup(group);
                        _UserService.UpdateUser(UserForAdding);
                        break;
                    }
                }
                return RedirectToAction("RequestsInGroup", new { id = GroupId });
            }
            return Forbid();
        }
        [HttpGet]
        public async Task<IActionResult> CancelFollowerFromGroup(int GroupId, int UserAddId)           //отклонить запрос в группу
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(UserId);
            var group = _GroupService.GetGroupById(GroupId);
            var UserForRemoving = _UserService.GetUserById(UserAddId);
            if (group == null || UserForRemoving == null || !group.IsClose) return BadRequest();

            GroupsViewModel model = new();
            model.GroupAdminName = _UserService.GetUserById(group.AdminId).Email;
            var UserGroups = MyUser.UserGroup;
            foreach (var gr in UserGroups)
            {
                if (gr.GroupId == GroupId)
                {
                    model.UserIsModerator = gr.IsModerator;
                    break;
                }
            }
            var result = await _authorizationService.AuthorizeAsync(User, model, "OwnerOrModeratorGroupPolicy");
            if (result.Succeeded)
            {
                foreach (UserGroup ug in UserForRemoving.UserGroup)
                {
                    if (ug.GroupId == GroupId)
                    {
                        UserForRemoving.UserGroup.Remove(ug);
                        _UserService.UpdateUser(UserForRemoving);
                        break;
                    }
                }
                return RedirectToAction("RequestsInGroup", new { id = GroupId });
            }
            return Forbid();
        }
        [HttpGet]
        public IActionResult Moderators(int id)           //модерирование
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(UserId);
            var group = _GroupService.GetGroupById(id);
            if (group == null || MyUser == null) return BadRequest();

            GroupsViewModel model = new();
            model.GroupAdminName = _UserService.GetUserById(group.AdminId).Email;
            model.Moderators = new();
            model.GroupId = id;
            model.GroupName = group.Name;
            model.GroupCountFollowers = group.CountFollowers;
            model.GroupAvatarURL = group.AvatarURL;
            model.GroupCreateDate = group.CreateDate;

            var result = model.GroupAdminName == MyUser.Email;
            if (result)
            {
                foreach (UserGroup ug in group.UserGroup)
                {
                    if (ug.IsModerator)
                    {
                        if (ug.UserId != UserId)
                        {
                            model.Moderators.Add(new()
                            {
                                AvatarURL = ug.User.AvatarURL,
                                 UserId = ug.User.Id,
                                Name = ug.User.Name,
                                Surname = ug.User.Surname,
                                IsModerator = true
                            }) ;
                        }
                    }
                }
                foreach (UserGroup ug in group.UserGroup)
                {
                    if (!ug.IsModerator)
                    {
                        model.Moderators.Add(new()
                        {
                            AvatarURL = ug.User.AvatarURL,
                            UserId = ug.User.Id,
                            Name = ug.User.Name,
                            Surname = ug.User.Surname,
                            IsModerator = false
                        });
                    }
                }
                return View(model);
            }
            return Forbid();
        }
        [HttpPost]
        public IActionResult Moderators(GroupsViewModel model)           //модерирование
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(UserId);
            var group = _GroupService.GetGroupById(model.GroupId);
            if (group == null || MyUser == null) return BadRequest();

            model.GroupAdminName = _UserService.GetUserById(group.AdminId).Email;

            var result = model.GroupAdminName == MyUser.Email;
            if (result)
            {
                _GroupService.SetModeratorsToGroup(model.GroupId, model.Moderators.Where(p=>p.IsModerator).Select(moderator=> moderator.UserId));

                return RedirectToAction("Moderators", new { id = model.GroupId});
            }
            return Forbid();
        }
        [HttpGet]
        public async Task <IActionResult> DeletePost(int PostId) //remove post
        {
            int UserId = _userManager.Users.SingleOrDefault(id => id.Email == User.Identity.Name).AppUserId;
            var MyUser = _UserService.GetUserById(UserId);
            var PostForRemoving = _PostService.GetPostsById(PostId);
            if (PostForRemoving.GroupId == null) return BadRequest();

            var group = _GroupService.GetGroupById((int)PostForRemoving.GroupId);
            if (group == null)
            {
                return BadRequest();
            }

            GroupsViewModel model = new();
            model.GroupAdminName = _UserService.GetUserById(group.AdminId).Email;
            var UserGroups = MyUser.UserGroup;
            foreach (var gr in UserGroups)
            {
                if (gr.GroupId == PostForRemoving.GroupId)
                {
                    model.UserIsModerator = gr.IsModerator;
                    break;
                }
            }
            var result = await _authorizationService.AuthorizeAsync(User, model, "OwnerOrModeratorGroupPolicy");
            if (result.Succeeded)
            {
                _PostService.DeletePost(PostId);
                return RedirectToAction("GroupPage", new { PostForRemoving.GroupId });
            }
            return Forbid();
        }
    }
}
