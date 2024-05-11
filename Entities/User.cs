namespace todo_api_app.Entities;

using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("email")]
    public required string Email { get; set; }

    [Column("password")]
    public required string Password { get; set; }

    //This 2 below is One to One Relationship
    //This is how it works in CS
    public int UserTokenId { get; set; }
    public UserToken? UserToken { get; set; }
}
