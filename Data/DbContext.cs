using todo_api_app.Entities;
using Microsoft.EntityFrameworkCore;

namespace todo_api_app.Data;

public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserToken> UserTokens => Set<UserToken>();

    public DbSet<Todo> Todos => Set<Todo>();
}
