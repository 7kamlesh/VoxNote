using System.Net.Http.Headers;
using System.Text.Json;
using VoxNote.Models.Dto;

namespace VoxNote.Services;

/// <summary>
/// Transcription service using the OpenAI Whisper API.
/// Requires "Whisper:ApiKey" in configuration.
/// </summary>
public class WhisperTranscriptionService : ITranscriptionService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhisperTranscriptionService> _logger;
    private readonly string? _apiKey;

    private const string WhisperEndpoint = "https://api.openai.com/v1/audio/transcriptions";

    public WhisperTranscriptionService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WhisperTranscriptionService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Whisper:ApiKey"];
    }

    public async Task<TranscriptionResponse> TranscribeAsync(Stream audioStream, string fileName)
    {
        if (string.IsNullOrWhiteSpace(_apiKey) || _apiKey == "sk-paste-your-key-here" || _apiKey == "YOUR_OPENAI_API_KEY_HERE")
        {
            return new TranscriptionResponse
            {
                Success = false,
                Error = "Whisper API key is not configured. Using browser-based transcription instead."
            };
        }

        try
        {
            using var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(audioStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/webm");
            content.Add(streamContent, "file", fileName);
            content.Add(new StringContent("whisper-1"), "model");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(WhisperEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Whisper API error {StatusCode}: {Body}", response.StatusCode, errorBody);
                return new TranscriptionResponse
                {
                    Success = false,
                    Error = $"Transcription service returned {(int)response.StatusCode}."
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WhisperResult>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new TranscriptionResponse
            {
                Success = true,
                Text = result?.Text ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transcription failed.");
            return new TranscriptionResponse
            {
                Success = false,
                Error = "An unexpected error occurred during transcription."
            };
        }
    }

    private sealed class WhisperResult
    {
        public string Text { get; set; } = string.Empty;
    }
}
