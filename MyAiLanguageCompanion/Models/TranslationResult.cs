namespace MyAiLanguageCompanion;

public sealed record TranslationResult(
    SupportedLanguage SourceLanguage,
    SupportedLanguage TargetLanguage,
    string OriginalText,
    string TranslatedText,
    string Notes,
    bool IsLocalOnly = true);
