namespace MyAiLanguageCompanion;

public interface ITranslator
{
    Task<TranslationResult> TranslateAsync(string text, SupportedLanguage targetLanguage, SupportedLanguage? detectedSourceLanguage = null, SupportedLanguage? manualSourceLanguage = null, CancellationToken cancellationToken = default);
    ModelStatus GetModelStatus();
}
