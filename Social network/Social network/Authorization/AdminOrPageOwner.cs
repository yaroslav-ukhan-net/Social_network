using Microsoft.AspNetCore.Authorization;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social_network.Authorization
{
    public class AdminOrPageOwner : AuthorizationHandler<CustomPageOwnerClaim, User>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CustomPageOwnerClaim requirement,
            User myuser)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            if (context.User.Identity?.Name == myuser.Email)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
