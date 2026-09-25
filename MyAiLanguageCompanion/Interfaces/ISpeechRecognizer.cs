namespace MyAiLanguageCompanion;

public interface ISpeechRecognizer
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    Task<string> RecognizeOnceAsync(CancellationToken cancellationToken = default);
    ModelStatus GetModelStatus();
}
