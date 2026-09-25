namespace MyAiLanguageCompanion;

public interface ITextToSpeech
{
    List<string> GetVoices();
    Task SpeakWordAsync(string text, SupportedLanguage language, double rate = 1.0, CancellationToken cancellationToken = default);
    Task SpeakSentenceAsync(string text, SupportedLanguage language, double rate = 1.0, CancellationToken cancellationToken = default);
    Task PauseAsync(CancellationToken cancellationToken = default);
    Task ResumeAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    ModelStatus GetModelStatus();
}
