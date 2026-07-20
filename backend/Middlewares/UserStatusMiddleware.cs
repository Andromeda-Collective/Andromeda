using Andromeda.Common.Services.Cache.User;
using Andromeda.Entities;
using Andromeda.Enums;
using Andromeda.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Andromeda.Middlewares;

public sealed class UserStatusMiddleware
{
    private readonly RequestDelegate _next;

    public UserStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserCacheService userCache, UserManager<User> userManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.GetUserId();
            var snapshot = await userCache.GetAsync(userId, context.RequestAborted);

            if (snapshot is null)
            {
                var user = await userManager.FindByIdAsync(userId.ToString());
                if (user is null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? nameof(Roles.User);
                snapshot = new CachedUserSnapshot(user.State, role);
                await userCache.SetAsync(userId, snapshot, context.RequestAborted);
            }

            if (snapshot.State == UserState.Banned)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "حساب کاربری شما مسدود شده است" });
                return;
            }
        }

        await _next(context);
    }
}