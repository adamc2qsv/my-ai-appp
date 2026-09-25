namespace MyAiLanguageCompanion;

public interface ILanguageDetector
{
    SupportedLanguage DetectLanguage(string text);
    bool IsLowConfidence(string text);
    ModelStatus GetModelStatus();
}
