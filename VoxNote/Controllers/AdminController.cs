using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxNote.Models;
using VoxNote.Services;

namespace VoxNote.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]



public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// Returns all registered users with note counts.
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Returns all notes for a specific user (read-only).
    /// </summary>
    [HttpGet("users/{userId:int}/notes")]
    public async Task<IActionResult> GetNotesByUser(int userId)
    {
        var notes = await _adminService.GetNotesByUserAsync(userId);
        return Ok(notes);
    }

    /// <summary>
    /// Returns all notes from all users (read-only).
    /// </summary>
    [HttpGet("notes")]
    public async Task<IActionResult> GetAllNotes()
    {
        var notes = await _adminService.GetAllNotesAsync();
        return Ok(notes);
    }
}
