using VoxNote.Models.Dto;

namespace VoxNote.Services;

public interface IAdminService
{
    Task<List<UserSummary>> GetAllUsersAsync();
    Task<List<NoteResponse>> GetNotesByUserAsync(int userId);
    Task<List<NoteResponse>> GetAllNotesAsync();
}
