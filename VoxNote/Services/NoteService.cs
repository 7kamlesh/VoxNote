using Microsoft.EntityFrameworkCore;
using VoxNote.Data;
using VoxNote.Models;
using VoxNote.Models.Dto;

namespace VoxNote.Services;

public class NoteService : INoteService
{
    private readonly AppDbContext _db;

    public NoteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<NoteResponse>> GetAllByUserAsync(int userId)
    {
        return await _db.Notes
            .Include(n => n.User)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => MapToResponse(n))
            .ToListAsync();
    }

    public async Task<NoteResponse?> GetByIdAsync(int id, int userId)
    {
        var note = await _db.Notes.Include(n => n.User).FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        return note is null ? null : MapToResponse(note);
    }

    public async Task<NoteResponse> CreateAsync(CreateNoteRequest request, int userId)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Notes.Add(note);
        await _db.SaveChangesAsync();

        await _db.Entry(note).Reference(n => n.User).LoadAsync();
        return MapToResponse(note);
    }

    public async Task<NoteResponse?> UpdateAsync(int id, UpdateNoteRequest request, int userId)
    {
        var note = await _db.Notes.Include(n => n.User).FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note is null)
            return null;

        note.Title = request.Title;
        note.Content = request.Content;
        note.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return MapToResponse(note);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note is null)
            return false;

        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();

        return true;
    }

    private static NoteResponse MapToResponse(Note note) => new()
    {
        Id = note.Id,
        Title = note.Title,
        Content = note.Content,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt,
        UserId = note.UserId,
        Username = note.User?.Username ?? string.Empty
    };
}
