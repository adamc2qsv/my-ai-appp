namespace MyAiLanguageCompanion;

public sealed class OfflineLanguageDetector : ILanguageDetector
{
    private static readonly IReadOnlyDictionary<SupportedLanguage, string[]> Markers = new Dictionary<SupportedLanguage, string[]>
    {
        [SupportedLanguage.English] = ["the", "and", "what", "where", "please", "thanks", "with", "you", "are"],
        [SupportedLanguage.Spanish] = ["el", "la", "los", "las", "que", "hola", "gracias", "por", "para", "como"],
        [SupportedLanguage.Dutch] = ["de", "het", "een", "ik", "je", "niet", "wat", "hoe", "gaat", "dank"],
        [SupportedLanguage.Russian] = ["и", "в", "не", "что", "как", "привет", "спасибо", "пожалуйста"]
    };

    public SupportedLanguage DetectLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return SupportedLanguage.English;
        if (text.Any(c => c is >= '\u0400' and <= '\u04ff')) return SupportedLanguage.Russian;
        var words = text.ToLowerInvariant().Split([' ', '\t', '\r', '\n', ',', '.', '!', '?', ';', ':'], StringSplitOptions.RemoveEmptyEntries);
        return Markers.Select(pair => (Language: pair.Key, Score: words.Count(word => pair.Value.Contains(word))))
            .OrderByDescending(item => item.Score).First().Language;
    }

    public bool IsLowConfidence(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return true;
        var words = text.Split([' ', '\t', '\r', '\n', ',', '.', '!', '?'], StringSplitOptions.RemoveEmptyEntries);
        var scores = Markers.Values.Select(markers => words.Count(word => markers.Contains(word.ToLowerInvariant()))).OrderByDescending(score => score).ToArray();
        return scores.Length == 0 || scores[0] == 0 || (scores.Length > 1 && scores[0] == scores[1]);
    }

    public ModelStatus GetModelStatus() => new(ModelState.Ready, "Language detection", "Local marker-based detection for English, Spanish, Dutch, and Russian.");
}
