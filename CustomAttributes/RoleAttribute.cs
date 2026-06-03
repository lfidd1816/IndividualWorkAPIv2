using IndividualWorkAPI.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace IndividualWorkAPI.CustomAttributes;

public class RoleAttribute : Attribute, IAsyncActionFilter
{
    private readonly int[] _allowedRoleIds;
    public RoleAttribute(int[] roleId)
    {
        _allowedRoleIds = roleId;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<ContextDatabase>();
        string? token = context.HttpContext.Request.Headers["authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token.Substring("Bearer ".Length).Trim();
        }
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new JsonResult(new { error = "Session don't transfer" })
                { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        var session = await dbContext.Sessions.Include(s => s.User)
            .FirstOrDefaultAsync(session => session.token == token);

        if (session == null)
        {
            context.Result = new JsonResult(new { error = "Session not found" })
                { StatusCode = StatusCodes.Status401Unauthorized };

            return;
        }

        if (!_allowedRoleIds.Contains(session.User.roleId))
        {
            context.Result = new JsonResult(new { error = "Haven't permissions" })
                { StatusCode = StatusCodes.Status401Unauthorized };

            return;
        }

        await next();
    }
}