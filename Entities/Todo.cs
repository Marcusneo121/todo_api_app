namespace todo_api_app.Entities;

using System.ComponentModel.DataAnnotations.Schema;

[Table("todo")]
public class Todo : BaseEntity
{
    // [Column("id")]
    // public int Id { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }
    public User? User { get; set; }
}
