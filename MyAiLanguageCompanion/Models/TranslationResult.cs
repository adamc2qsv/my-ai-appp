namespace MyAiLanguageCompanion;

public enum SupportedLanguage
{
    English,
    Spanish,
    Dutch,
    Russian
}

public static class SupportedLanguageExtensions
{
    public static string GetDisplayName(this SupportedLanguage language)
    {
        return language switch
        {
            SupportedLanguage.English => "🇬🇧 English",
            SupportedLanguage.Spanish => "🇪🇸 Spanish",
            SupportedLanguage.Dutch => "🇳🇱 Dutch",
            SupportedLanguage.Russian => "🇷🇺 Russian",
            _ => language.ToString()
        };
    }
}
