using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
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

        public UserController(UserService userService)
        {
            _userService = userService;
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

            return View(ToViewModel(User));
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
                Surname = user.Surname
            };
        }
        private User ToModel(UserViewModel userView)
        {
            return new User()
            {
                BirthDate = userView.BirthDate,
                Surname = userView.Surname,
                PhoneNumber = userView.PhoneNumber,
                AvatarURL = userView.AvatarURL,
                Id = userView.Id,
                Name = userView.Name,
                Notes = userView.Notes
            };
        }
    }
}
