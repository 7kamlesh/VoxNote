namespace VoxNote.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = Roles.User;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Note> Notes { get; set; } = [];
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
