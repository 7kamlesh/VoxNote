using Microsoft.EntityFrameworkCore;
using VoxNote.Data;
using VoxNote.Models.Dto;

namespace VoxNote.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;

    public AdminService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserSummary>> GetAllUsersAsync()
    {
        return await _db.Users
            .OrderBy(u => u.Username)
            .Select(u => new UserSummary
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                NoteCount = u.Notes.Count,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<NoteResponse>> GetNotesByUserAsync(int userId)
    {
        return await _db.Notes
            .Include(n => n.User)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                UserId = n.UserId,
                Username = n.User.Username
            })
            .ToListAsync();
    }

    public async Task<List<NoteResponse>> GetAllNotesAsync()
    {
        return await _db.Notes
            .Include(n => n.User)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                UserId = n.UserId,
                Username = n.User.Username
            })
            .ToListAsync();
    }
}
