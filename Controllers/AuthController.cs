using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_api_app.Data;
using todo_api_app.Dtos;
using todo_api_app.Entities;
using todo_api_app.Utils;

namespace todo_api_app.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly IConfiguration _configuration;
    private readonly DBContext _dbContext;
    private readonly JWTManagerUtils _jwtManagerUtils;
    const string GetUserEndpointName = "GetUser";
    public AuthController(ILogger<TodoController> logger, IConfiguration configuration, DBContext dBContext, JWTManagerUtils jwtManagerUtils)
    {
        _logger = logger;
        _configuration = configuration;
        _dbContext = dBContext;
        _jwtManagerUtils = jwtManagerUtils;
    }

    // POST sign-up
    [HttpPost("signup")]
    public async Task<ActionResult<IEnumerable<User>>> Signup(SignUpDto signUp)
    {

        var emailCheck = _dbContext.Users.Where(x => x.Email == signUp.Email).ToListAsync().Result;
        // var username = await _dbContext.Users.FindAsync();

        if (emailCheck.Any())
        {
            return BadRequest(new
            {
                // data = emailCheck,
                status = 400,
                message = "Email already registered.",
            });
        }

        byte[] salted = AuthUtils.SaltProvider();

        var userData = new User()
        {
            Email = signUp.Email,
            Password = AuthUtils.PasswordHashing(signUp.Password, salted),
            Name = signUp.Name,
            PasswordSalt = Convert.ToBase64String(salted),
        };

        await _dbContext.Users.AddAsync(userData);
        await _dbContext.SaveChangesAsync();


        return Ok(new
        {
            data = userData,
            status = 200,
            message = "Sign up successful.",
        });
    }

    // POST login
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto login)
    {

        var emailCheck = _dbContext.Users.Where(x => x.Email == login.Email).ToListAsync().Result;

        if (emailCheck.Any())
        {
            User userExtractedData = emailCheck.First();
            var saltUserInputPassword = AuthUtils.PasswordHashing(login.Password, Convert.FromBase64String(userExtractedData.PasswordSalt));

            if (saltUserInputPassword == userExtractedData.Password)
            {

                var refreshTokenValidCheck = _dbContext.Users
                .Include(u => u.UserToken)
                .FirstOrDefault(u => u.Id == userExtractedData.Id);

                if (refreshTokenValidCheck?.UserToken?.IsTokenValid == true)
                {
                    refreshTokenValidCheck.UserToken!.IsTokenValid = false;
                    await _dbContext.SaveChangesAsync();
                }


                // Can generate Refresh and Access Token Already
                string? acccessTokenGenerated = _jwtManagerUtils.GenerateAccessToken(
                    new AccessTokenDto(Id: userExtractedData.Id,
                    Name: userExtractedData.Name,
                    Email: userExtractedData.Email
                    )
                );

                RefreshTokenDto? refreshTokenGenerated = _jwtManagerUtils.GenerateRefreshToken(
                    new AccessTokenDto(Id: userExtractedData.Id,
                    Name: userExtractedData.Name,
                    Email: userExtractedData.Email)
                );

                if (acccessTokenGenerated != null && refreshTokenGenerated != null)
                {
                    //Update old Refresh Token isValid Column to false;
                    var userTokenData = new UserToken()
                    {
                        RefreshToken = refreshTokenGenerated.RefreshToken,
                        ExpiredDate = refreshTokenGenerated.ExpiredDate,
                        IsTokenValid = true,
                    };

                    var entityToUpdate = _dbContext.Users.FirstOrDefault(e => e.Id == userExtractedData.Id);
                    if (entityToUpdate != null)
                    {
                        entityToUpdate.UserToken = userTokenData;
                    }
                    await _dbContext.SaveChangesAsync();

                    return Ok(new
                    {
                        data = new
                        {
                            access_token = acccessTokenGenerated,
                            refresh_token = refreshTokenGenerated.RefreshToken,
                            refresh_expiration_date = refreshTokenGenerated.ExpiredDate,
                        },
                        status = 200,
                        message = "Login successfully.",
                    });
                }
                else
                {
                    return Unauthorized(new
                    {
                        status = 401,
                        message = "User doesn't exist",
                    });
                }
            }
            else
            {
                return Unauthorized(new
                {
                    status = 401,
                    message = "Password incorrect. Please try again.",
                });
            }
        }
        else
        {
            return BadRequest(new
            {
                status = 400,
                message = "This account doesn't exist.",
            });
        }
    }

    // Get Access Token (a.k.a generate-token)
    [HttpGet("generate-token")]
    public ActionResult GenerateToken(GenerateTokenDto gt)
    {
        try
        {
            ClaimsPrincipal claims = _jwtManagerUtils.ValidateExtractRefreshTokenIdentity(gt.RefreshToken);
            Console.WriteLine($"Claims: {claims.Identity?.IsAuthenticated}");
            Console.WriteLine($"Claims: {claims.FindFirst(ClaimTypes.Name)?.Value!}");

            string? name = claims.FindFirst(ClaimTypes.Name)?.Value!;
            string? id = claims.FindFirst(ClaimTypes.Sid)?.Value!;
            string? email = claims.FindFirst(ClaimTypes.Email)?.Value!;

            if (name != null && id != null && email != null)
            {

                var tokenValidCheck = _dbContext.Users
                .Include(u => u.UserToken)
                .FirstOrDefault(u => u.Id == int.Parse(id));

                if (tokenValidCheck == null)
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "User not found.",
                    });
                }
                else
                {
                    if (tokenValidCheck.UserToken?.IsTokenValid == false)
                    {
                        return Unauthorized(new
                        {
                            status = 401,
                            message = "Invalid Refresh Token.",
                        });
                    }
                    else
                    {
                        string? acccessTokenGenerated = _jwtManagerUtils.GenerateAccessToken(
                                            new AccessTokenDto(Id: int.Parse(id),
                                            Name: name,
                                            Email: email));
                        return Ok(new
                        {
                            access_token = acccessTokenGenerated,
                            status = 200,
                            message = "Ok",
                        });
                    }
                }
            }
            else
            {
                return Unauthorized(new
                {
                    status = 401,
                    message = "Invalid Refresh Token.",
                });
            }
        }
        catch (Exception ex)
        {
            return Unauthorized(new
            {
                status = 401,
                message = ex.Message,
            });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        int userId = int.Parse(HttpContext.Items["user_id"]?.ToString()!);
        var userToUpdate = _dbContext.Users
                .Include(u => u.UserToken)
                .FirstOrDefault(u => u.Id == userId);
        if (userToUpdate == null)
        {
            return BadRequest(new
            {
                status = 400,
                message = "User not found.",
            });
        }
        else
        {
            userToUpdate.UserToken!.IsTokenValid = false;
            await _dbContext.SaveChangesAsync();
            return Ok(new
            {
                status = 200,
                message = "Logout successfully.",
            });
        }
    }
}
