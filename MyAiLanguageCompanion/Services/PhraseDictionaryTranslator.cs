namespace MyAiLanguageCompanion;

public sealed class OfflineLanguageDetector : ILanguageDetector
{
    private static readonly string[] DutchWords = ["ik", "je", "het", "niet", "wat", "hoe", "gaat", "gaat", "hallo", "dankje", "welkom"];
    private static readonly string[] SpanishWords = ["hola", "gracias", "por", "que", "como", "donde", "buenos", "dias", "buenas", "noches"];
    private static readonly string[] EnglishWords = ["hello", "the", "what", "where", "please", "thanks", "good", "morning", "evening", "i", "you"];
    private static readonly string[] RussianWords = ["привет", "как", "где", "спасибо", "пожалуйста", "хорошо", "сегодня", "здравствуйте", "я", "ты"];

    public SupportedLanguage DetectLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return SupportedLanguage.English;
        }

        var lowered = text.ToLowerInvariant();
        if (ContainsCyrillic(lowered))
        {
            return SupportedLanguage.Russian;
        }

        var score = new Dictionary<SupportedLanguage, int>
        {
            [SupportedLanguage.English] = CountMatches(lowered, EnglishWords),
            [SupportedLanguage.Spanish] = CountMatches(lowered, SpanishWords),
            [SupportedLanguage.Dutch] = CountMatches(lowered, DutchWords),
            [SupportedLanguage.Russian] = CountMatches(lowered, RussianWords)
        };

        return score.OrderByDescending(x => x.Value).First().Key;
    }

    public bool IsLowConfidence(string text)
    {
        var language = DetectLanguage(text);
        return language switch
        {
            SupportedLanguage.Russian when !ContainsCyrillic(text) => true,
            _ => false
        };
    }

    private static bool ContainsCyrillic(string value) => value.Any(c => c >= 0x0400 && c <= 0x04FF);

    private static int CountMatches(string text, IEnumerable<string> tokens)
    {
        var score = 0;
        foreach (var token in tokens)
        {
            if (text.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                score++;
            }
        }

        return score;
    }
}
