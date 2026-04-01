using VoxNote.Models.Dto;

namespace VoxNote.Services;

public interface ITranscriptionService
{
    Task<TranscriptionResponse> TranscribeAsync(Stream audioStream, string fileName);
}
