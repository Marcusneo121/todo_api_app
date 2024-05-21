using todo_api_app.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace todo_api_app.Data;

public class DBContext : DbContext
{

    private readonly UserContextService _userContextService;

    public DBContext(DbContextOptions<DBContext> options, UserContextService userContextService)
        : base(options)
    {
        _userContextService = userContextService;
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserToken> UserTokens => Set<UserToken>();

    public DbSet<Todo> Todos => Set<Todo>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _userContextService.GetCurrentUserId();
        var intCurrentUserId = int.Parse(currentUserId == null ? "0" : currentUserId.ToString());

        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            ((BaseEntity)entityEntry.Entity).UpdatedBy = intCurrentUserId;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                ((BaseEntity)entityEntry.Entity).CreatedBy = intCurrentUserId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
