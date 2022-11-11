using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services;
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
        public GroupController(UserService userService, GroupService groupService)
        {
            _UserService = userService;
            _GroupService = groupService;
        }
        //Get:Group/MyGroups/id
        [HttpGet]
        public IActionResult MyGroups(int id)
        {
            ViewData["PageName"] = "MyGroups";
            var AllGroups = _GroupService.GetAllGroups();
            var MyUser = _UserService.GetUserById(id);
            var model = new UserGroupsViewModel();
            
            if(MyUser == null)
            {
                return BadRequest();
            }

            model.Id = MyUser.Id;
            model.Name = MyUser.Name;
            model.Surname = MyUser.Surname;
            model.AvatarURL = MyUser.AvatarURL;
            model.Groups = new List<GroupsViewModel>();

            foreach(var group in AllGroups)
            {
                if(MyUser.UserGroup.Any(g=> g.GroupId == group.Id))
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
        public IActionResult DeleteGroup(int MyUserId, int DeleteGroup)
        {
            //if(admin)
            _UserService.UserUnsubscribe(MyUserId, DeleteGroup);
            return RedirectToAction("MyGroups", MyUserId);
        }
        [HttpGet]
        public IActionResult CreateGroup(int Id)
        {
            var model = new GroupsViewModel();
            model.GroupAdminId = Id;

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

            };
            _GroupService.CreateGroup(newgroup);
            
            var myuser = _UserService.GetUserById(groupsView.GroupAdminId);
            var asd = new UserGroup() { Group = newgroup, User = myuser };
            myuser.UserGroup.Add(asd);

            _UserService.UpdateUser(myuser);

            return RedirectToAction("MyGroups", new { Id = groupsView.GroupAdminId });
        }
        //Get:Group/MyGroups/id
        [HttpGet]
        public IActionResult NewGroups(int id)
        {
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
                        GroupNotes = group.Notes
                    });
                }
            }
            return View("MyGroups",model);
        }
        [HttpGet]
        public IActionResult AddGroup(int MyUserId, int AddGroupId)
        {
            //if(admin)
            //_UserService.UserUnsubscribe(MyUserId, AddGroupId);
            return RedirectToAction("MyGroups", MyUserId);
        }
        
    }
}
