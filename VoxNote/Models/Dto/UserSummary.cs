namespace VoxNote.Models.Dto;

public class UserSummary
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int NoteCount { get; set; }

    public DateTime CreatedAt { get; set; }
}
