using CinemaAPI.Connection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorize : Attribute, IAsyncActionFilter
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorize(int[] roleID)
        {
            _allowedRoles = roleID;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var header) ||
                string.IsNullOrWhiteSpace(header))
            {
                context.Result = new JsonResult(new { error = "Не передан токен" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            var token = header.ToString().Trim();
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new JsonResult(new { error = "Неверный формат токена" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            var db = context.HttpContext.RequestServices.GetRequiredService<ContextDb>();

            var session = await db.Sessions
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);

            if (session == null)
            {
                context.Result = new JsonResult(new { error = "Сессия не найдена" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            if (!_allowedRoles.Contains(session.User.RoleID))
            {
                context.Result = new JsonResult(new { error = "Недостаточно прав" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            context.HttpContext.Items["CurrentUser"] = session.User;
            context.HttpContext.Items["Session"] = session;

            await next();
        }
    }
}
