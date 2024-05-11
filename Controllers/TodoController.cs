using Microsoft.AspNetCore.Mvc;

namespace todo_api_app.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly IConfiguration _configuration;

    public TodoController(ILogger<TodoController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    // GET api/todo
    [HttpGet]
    public ActionResult<IEnumerable<string>> Get()
    {
        return new string[] { "value1", "value2" };
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
