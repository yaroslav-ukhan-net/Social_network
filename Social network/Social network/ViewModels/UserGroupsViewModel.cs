using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.ViewModels
{
    public class UserGroupsViewModel
    {
        public virtual List<GroupsViewModel> Groups { get; set; }
    }
}
