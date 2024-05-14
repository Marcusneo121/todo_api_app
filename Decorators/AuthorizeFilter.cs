using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using todo_api_app.Utils;

namespace todo_api_app.Decorators;

public class AuthorizeFilterMiddleware : IAuthorizationFilter
{
    private readonly JWTManagerUtils _jwtManagerUtils;

    public AuthorizeFilterMiddleware(JWTManagerUtils jwtManagerUtils)
    {
        _jwtManagerUtils = jwtManagerUtils;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the [Authorize] attribute is applied to the action.
        var isAuthorize = context.ActionDescriptor.EndpointMetadata
            .Any(em => em is AuthorizeAttribute);

        if (isAuthorize)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var claimsPrincipal = _jwtManagerUtils.ValidateExtractAccessTokenIdentity(token);
                    var userId = claimsPrincipal.FindFirst(ClaimTypes.Sid)?.Value;
                    context.HttpContext.Items["user_id"] = userId;
                }
                catch (Exception)
                {
                    context.Result = new JsonResult(new
                    {
                        status = 401,
                        message = "Unauthorized access, Invalid token.",
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        ContentType = "application/json"
                    };
                    // context.Result = new UnauthorizedObjectResult(new
                    // {
                    //     status = 401,
                    //     message = "Unauthorized access, Token not found.",
                    // });
                }
            }
            else
            {
                context.Result = new JsonResult(new
                {
                    status = 401,
                    message = "Unauthorized access, Invalid token.",
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ContentType = "application/json"
                };
            }
        }
    }
}