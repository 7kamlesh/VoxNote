using Microsoft.AspNetCore.Mvc;
using VoxNote.Services;

namespace VoxNote.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranscriptionController : ControllerBase
{
    private readonly ITranscriptionService _transcriptionService;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "audio/webm",
        "audio/wav",
        "audio/mpeg",
        "audio/mp4",
        "audio/ogg"
    };

    private const long MaxFileSizeBytes = 25 * 1024 * 1024; // 25 MB (Whisper limit)

    public TranscriptionController(ITranscriptionService transcriptionService)
    {
        _transcriptionService = transcriptionService;
    }

    /// <summary>
    /// Accepts an audio file and returns the transcribed text.
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Transcribe(IFormFile audio)
    {
        if (audio is null || audio.Length == 0)
            return BadRequest(new { error = "No audio file provided." });

        if (!IsAllowedContentType(audio.ContentType))
            return BadRequest(new { error = $"Unsupported audio format: {audio.ContentType}." });

        if (audio.Length > MaxFileSizeBytes)
            return BadRequest(new { error = "File exceeds the 25 MB size limit." });

        await using var stream = audio.OpenReadStream();
        var result = await _transcriptionService.TranscribeAsync(stream, audio.FileName);

        if (!result.Success)
            return StatusCode(502, new { error = result.Error });

        return Ok(result);
    }

    private static bool IsAllowedContentType(string contentType)
    {
        // Strip parameters like ";codecs=opus" to get the base media type
        var baseType = contentType.Split(';', 2)[0].Trim();
        return AllowedContentTypes.Contains(baseType);
    }
}
