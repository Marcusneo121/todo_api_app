using Microsoft.EntityFrameworkCore;

namespace todo_api_app.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
