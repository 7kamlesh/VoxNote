using VoxNote.Models;
using VoxNote.Models.Dto;

namespace VoxNote.Services;

public interface INoteService
{
    Task<List<NoteResponse>> GetAllByUserAsync(int userId);
    Task<NoteResponse?> GetByIdAsync(int id, int userId);
    Task<NoteResponse> CreateAsync(CreateNoteRequest request, int userId);
    Task<NoteResponse?> UpdateAsync(int id, UpdateNoteRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
