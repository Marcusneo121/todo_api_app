namespace todo_api_app.Entities;

using System.ComponentModel.DataAnnotations.Schema;

[Table("user_token")]
public class UserToken : BaseEntity
{
    // [Column("id")]
    // public int Id { get; set; }

    [Column("refresh_token")]
    public string? RefreshToken { get; set; }

    [Column("expired_date")]
    public DateTime? ExpiredDate { get; set; }

    [Column("is_token_valid")]
    public bool IsTokenValid { get; set; }

    // [Column("user_id")]
    // public int UserId { get; set; }
    // public User User { get; set; } = null!;
}
