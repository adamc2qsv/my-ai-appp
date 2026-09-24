namespace MyAiLanguageCompanion;

public interface ILiveSessionService
{
    Task StartAsync(string source, CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    string CurrentTranscript { get; }
}
