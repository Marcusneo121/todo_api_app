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
    const string GetUserEndpointName = "GetUser";
    public AuthController(ILogger<TodoController> logger, IConfiguration configuration, DBContext dBContext)
    {
        _logger = logger;
        _configuration = configuration;
        _dbContext = dBContext;
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
        // string saltedString = Convert.ToBase64String(salted);
        // byte[] testSalt = Convert.FromBase64String(saltedString);

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
            message = "Sign up successful.",
        });
        // return Results.CreatedAtRoute(GetUserEndpointName, new { id = userData.Id }, userData);
        // var users = await _dbContext.Users
        // .Where(u => u.Name == signUp.Name)
        // .Select(u => new
        // {
        //     u.Id,
        //     u.Name,
        //     u.Email,
        //     u.Password
        // })
        // .ToListAsync();
        // if (users == null || !users.Any())
        // {
        //     // User not found, return a custom JSON response
        //     return Ok(new { message = "User not found" });
        // }
        // User found, return the user object
        // return Ok(new { data = users, message = "Sign up successful." });
    }

    // POST login
    [HttpPost("login")]
    // public void Login([FromBody] string value)
    public ActionResult Login(LoginDto login)
    {

        var emailCheck = _dbContext.Users.Where(x => x.Email == login.Email).ToListAsync().Result;

        if (emailCheck.Any())
        {
            User userExtractedData = emailCheck.First();
            var saltUserInputPassword = AuthUtils.PasswordHashing(login.Password, Convert.FromBase64String(userExtractedData.PasswordSalt));

            if (saltUserInputPassword == userExtractedData.Password)
            {
                // Can generate Refresh and Access Token Already
                return Ok(new
                {
                    data = new
                    {
                        emailCheck.First().Password,
                        emailCheck.First().PasswordSalt,
                        saltUserInputPassword
                    },
                    message = "Login successfully.",
                });
            }
            else
            {
                return Unauthorized(new
                {
                    // data = emailCheck,
                    status = 401,
                    message = "Password incorrect. Please try again.",
                });
            }
        }
        else
        {
            return BadRequest(new
            {
                // data = emailCheck,
                status = 400,
                message = "This account doesn't exist.",
            });
        }

        // return Ok(new
        // {
        //     data = login,
        //     message = "Login Ok",
        // });
    }

    // Get Access Token (a.k.a generate-token)
    [HttpGet("generate-token")]
    public ActionResult GenerateToken(LoginDto login)
    {
        return Ok(new
        {
            // data = login,
            message = "Ok",
        });
    }
}
