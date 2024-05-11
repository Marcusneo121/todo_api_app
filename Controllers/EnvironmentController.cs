using Microsoft.AspNetCore.Mvc;

namespace todo_api_app.Controllers;

[ApiController]
[Route("api/env")]
public class EnvironmentController : ControllerBase
{
    private readonly ILogger<EnvironmentController> _logger;
    private readonly IConfiguration _configuration;

    public EnvironmentController(ILogger<EnvironmentController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult GetEnv()
    {
        var envname = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var appsettingsData = _configuration.GetConnectionString("DefaultConnection");

        var response = new
        {
            env = envname,
            appsettingdata = appsettingsData
        };

        return Ok(response);
    }
}
