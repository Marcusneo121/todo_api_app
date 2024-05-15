using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using todo_api_app.Data;
using todo_api_app.Dtos;
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
            message = "Get Todos successfully.",
        });
    }

    // GET api/todo/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Todo>> GetByID(int id)
    {
        int userId = int.Parse(HttpContext.Items["user_id"]?.ToString()!);

        var todo = await _dbContext.Users
            .Include(x => x.Todos)
            .Where(x => x.Id == userId)
            .SelectMany(a => a.Todos)
            .Select(u => new
            {
                u.Id,
                u.Title,
                u.Description,
                u.IsCompleted
            })
            .Where(b => b.Id == id).FirstOrDefaultAsync();

        if (todo == null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Todos not found.",
            });
        }

        return Ok(new
        {
            data = todo,
            status = 200,
            message = "Get Todo successfully.",
        });
    }

    // POST api/todo
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Todo>> Post(TodoDto todoDto)
    {
        int userId = int.Parse(HttpContext.Items["user_id"]?.ToString()!);

        var findUser = _dbContext.Users.FirstOrDefault(e => e.Id == userId);

        if (findUser == null)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "User doesn't exist",
            });
        }

        Console.WriteLine($"IsCompleted : {todoDto.IsCompleted}");

        var newTodo = new Todo
        {
            Title = todoDto.Title,
            Description = todoDto.Description,
            IsCompleted = todoDto.IsCompleted,
        };

        findUser.Todos.Add(newTodo);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByID), new { id = newTodo.Id }, todoDto);
    }

    // PUT api/todo/5
    [HttpPut("{id}")]
    public void Put(int id, Todo todoDto)
    {

    }

    // DELETE api/todo/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        int userId = int.Parse(HttpContext.Items["user_id"]?.ToString()!);

        var findUser = await _dbContext.Users
            .Include(x => x.Todos)
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync();

        if (findUser == null)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "User doesn't exist",
            });
        }

        var findTodo = findUser.Todos.FirstOrDefault(e => e.Id == id);

        if (findTodo == null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Todo not found.",
            });
        }

        findUser.Todos.Remove(findTodo);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/todo
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteAll()
    {
        int userId = int.Parse(HttpContext.Items["user_id"]?.ToString()!);

        var findUser = await _dbContext.Users
            .Include(x => x.Todos)
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync();

        if (findUser == null)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "User doesn't exist",
            });
        }

        // var findTodo = findUser.Todos
        //     .Select(u => new
        //     {
        //         u.Id,
        //         u.Title,
        //         u.Description,
        //         u.IsCompleted
        //     }).ToList();
        // return Ok(new
        // {
        //     data = findTodo,
        //     status = 200,
        //     message = "Logout successfully.",
        // });
        // if (findTodo.IsNullOrEmpty())
        // {
        //     return NotFound(new
        //     {
        //         status = 404,
        //         message = "No Todos found.",
        //     });
        // }

        findUser.Todos.RemoveAll(x => x.UserId == userId);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
