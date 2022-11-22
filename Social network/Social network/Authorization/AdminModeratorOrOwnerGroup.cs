using Microsoft.AspNetCore.Authorization;
using Social_network.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.Authorization
{
    public class AdminModeratorOrOwnerGroup : AuthorizationHandler<CustomGroupOwnerClaim, GroupsViewModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomGroupOwnerClaim requirement, GroupsViewModel group)
        {
            if (context.User.IsInRole("Admin"))                      // Admin
            {
                context.Succeed(requirement);
            }

            else if (context.User.Identity?.Name == group.GroupAdminName)      // Owner
            {
                context.Succeed(requirement);
            }
            else if(group.UserIsModerator)                                    // Moderator 
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
