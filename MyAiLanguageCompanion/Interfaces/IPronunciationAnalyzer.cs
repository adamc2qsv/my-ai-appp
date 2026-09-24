namespace MyAiLanguageCompanion;

public interface ITextToSpeech
{
    List<string> GetVoices();
    Task SpeakWordAsync(string text, SupportedLanguage language, double rate, CancellationToken cancellationToken = default);
    Task SpeakSentenceAsync(string text, SupportedLanguage language, double rate, CancellationToken cancellationToken = default);
}
