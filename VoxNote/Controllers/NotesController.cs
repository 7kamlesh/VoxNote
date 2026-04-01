using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxNote.Models.Dto;
using VoxNote.Services;

namespace VoxNote.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Returns all notes for the current user, most recent first.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notes = await _noteService.GetAllByUserAsync(GetUserId());
        return Ok(notes);
    }

    /// <summary>
    /// Returns a single note by id (only if owned by current user).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var note = await _noteService.GetByIdAsync(id, GetUserId());
        if (note is null)
            return NotFound(new { error = $"Note with id {id} not found." });

        return Ok(note);
    }

    /// <summary>
    /// Creates a new note for the current user.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { error = "Note content is required." });

        var note = await _noteService.CreateAsync(request, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    /// <summary>
    /// Updates an existing note (only if owned by current user).
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { error = "Note content is required." });

        var note = await _noteService.UpdateAsync(id, request, GetUserId());
        if (note is null)
            return NotFound(new { error = $"Note with id {id} not found." });

        return Ok(note);
    }

    /// <summary>
    /// Deletes a note by id (only if owned by current user).
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _noteService.DeleteAsync(id, GetUserId());
        if (!deleted)
            return NotFound(new { error = $"Note with id {id} not found." });

        return NoContent();
    }
}
