using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using todo_api_app.Data;
using todo_api_app.Entities;

namespace todo_api_app.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly IConfiguration _configuration;
    private readonly DBContext _dbContext;

    public TodoController(ILogger<TodoController> logger, IConfiguration configuration, DBContext dBContext)
    {
        _logger = logger;
        _configuration = configuration;
        _dbContext = dBContext;
    }

    // GET api/todo
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Todo>>> Get()
    {
        int userId = int.Parse(HttpContext.Items["user_id"]?.ToString()!);

        var todos = await _dbContext.Users
        .Include(x => x.Todos)
        .Where(a => a.Id == userId)
        .SelectMany(a => a.Todos).Select(u => new
        {
            u.Id,
            u.Title,
            u.Description,
            u.IsCompleted
        }).ToListAsync();
        // var todos = _dbContext.Users
        //             .Include(a => a.Todos)
        //             .Where(a => a.Id == userId)
        //             .OrderByDescending(a => a.Id)
        //             .Take(1)
        //             .SelectMany(a => a.Todos)
        //             .ToListAsync();

        if (todos == null)
        {
            return BadRequest(new
            {
                status = 400,
                message = "User not found.",
            });
        }
        // string resultConverted = JsonConvert.SerializeObject(todos);
        // Console.WriteLine($"Testing Get : {resultConverted}");
        return Ok(new
        {
            data = todos,
            status = 200,
            message = "Get successfully.",
        });
    }

    // GET api/todo/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        return "value";
    }

    // POST api/todo
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/todo/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/todo/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
