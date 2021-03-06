﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Server.Security
{
    public class SuperAdminHandler : AuthorizationHandler<ManageAdminRolesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesRequirement requirement)
        {
            if (context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
