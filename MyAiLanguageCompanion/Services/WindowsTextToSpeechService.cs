namespace MyAiLanguageCompanion;

public sealed class PhraseDictionaryTranslator : ITranslator
{
    private readonly ILanguageDetector _languageDetector;

    private readonly Dictionary<string, Dictionary<SupportedLanguage, string>> _phrases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["hello"] = new() { [SupportedLanguage.English] = "hello", [SupportedLanguage.Spanish] = "hola", [SupportedLanguage.Dutch] = "hallo", [SupportedLanguage.Russian] = "привет" },
        ["how are you?"] = new() { [SupportedLanguage.English] = "how are you?", [SupportedLanguage.Spanish] = "¿cómo estás?", [SupportedLanguage.Dutch] = "hoe gaat het?", [SupportedLanguage.Russian] = "как вы?" },
        ["thank you"] = new() { [SupportedLanguage.English] = "thank you", [SupportedLanguage.Spanish] = "gracias", [SupportedLanguage.Dutch] = "dank je", [SupportedLanguage.Russian] = "спасибо" },
        ["good morning"] = new() { [SupportedLanguage.English] = "good morning", [SupportedLanguage.Spanish] = "buenos días", [SupportedLanguage.Dutch] = "goedemorgen", [SupportedLanguage.Russian] = "доброе утро" },
        ["where is the station?"] = new() { [SupportedLanguage.English] = "where is the station?", [SupportedLanguage.Spanish] = "¿dónde está la estación?", [SupportedLanguage.Dutch] = "waar is het station?", [SupportedLanguage.Russian] = "где станция?" },
        ["i do not understand"] = new() { [SupportedLanguage.English] = "i do not understand", [SupportedLanguage.Spanish] = "no entiendo", [SupportedLanguage.Dutch] = "ik begrijp het niet", [SupportedLanguage.Russian] = "я не понимаю" }
    };

    public PhraseDictionaryTranslator(ILanguageDetector languageDetector)
    {
        _languageDetector = languageDetector;
    }

    public Task<TranslationResult> TranslateAsync(string text, SupportedLanguage targetLanguage, SupportedLanguage? detectedSourceLanguage = null, SupportedLanguage? manualSourceLanguage = null, CancellationToken cancellationToken = default)
    {
        var source = manualSourceLanguage ?? detectedSourceLanguage ?? _languageDetector.DetectLanguage(text);
        var trimmed = text.Trim();
        var resultText = trimmed;
        var notes = "Local phrase dictionary translation.";

        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            foreach (var pair in _phrases)
            {
                if (string.Equals(pair.Key, trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    if (pair.Value.TryGetValue(targetLanguage, out var translated))
                    {
                        resultText = translated;
                        notes = "Matched a local phrase dictionary entry.";
                    }

                    break;
                }
            }
        }

        if (string.Equals(resultText, trimmed, StringComparison.Ordinal))
        {
            notes = "The text was not found in the local phrase table; this is a best-effort offline translation only.";
        }

        return Task.FromResult(new TranslationResult(source, targetLanguage, trimmed, resultText, notes, true));
    }
}
