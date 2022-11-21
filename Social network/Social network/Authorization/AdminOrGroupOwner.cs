using Microsoft.AspNetCore.Authorization;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.Authorization
{
    public class AdminOrGroupOwner : AuthorizationHandler<CustomGroupOwnerClaim, Group>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomGroupOwnerClaim requirement, Group group)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            //if (context.User.Identity?.Name == myuser.Email)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
