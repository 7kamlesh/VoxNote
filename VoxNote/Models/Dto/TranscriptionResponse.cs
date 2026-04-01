namespace VoxNote.Models.Dto;

public class TranscriptionResponse
{
    public bool Success { get; set; }

    public string Text { get; set; } = string.Empty;

    public string? Error { get; set; }
}
