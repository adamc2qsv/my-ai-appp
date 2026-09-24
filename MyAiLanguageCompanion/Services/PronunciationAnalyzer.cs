using System.Speech.Synthesis;

namespace MyAiLanguageCompanion;

public sealed class WindowsTextToSpeechService : ITextToSpeech
{
    private readonly SpeechSynthesizer _synthesizer = new();

    public List<string> GetVoices()
    {
        return _synthesizer.GetInstalledVoices().Select(v => v.VoiceInfo.Name).ToList();
    }

    public Task SpeakWordAsync(string text, SupportedLanguage language, double rate, CancellationToken cancellationToken = default)
    {
        var speechText = string.IsNullOrWhiteSpace(text) ? "hello" : text;
        _synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, GetCulture(language));
        _synthesizer.Rate = ConvertRate(rate);
        _synthesizer.SpeakAsync(speechText);
        return Task.CompletedTask;
    }

    public Task SpeakSentenceAsync(string text, SupportedLanguage language, double rate, CancellationToken cancellationToken = default)
    {
        var speechText = string.IsNullOrWhiteSpace(text) ? "hello" : text;
        _synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, GetCulture(language));
        _synthesizer.Rate = ConvertRate(rate);
        _synthesizer.SpeakAsync(speechText);
        return Task.CompletedTask;
    }

    private static int ConvertRate(double rate)
    {
        return rate switch
        {
            0.75 => -2,
            1.0 => 0,
            1.25 => 2,
            _ => 0
        };
    }

    private static System.Globalization.CultureInfo GetCulture(SupportedLanguage language)
    {
        return language switch
        {
            SupportedLanguage.English => System.Globalization.CultureInfo.GetCultureInfo("en-US"),
            SupportedLanguage.Spanish => System.Globalization.CultureInfo.GetCultureInfo("es-ES"),
            SupportedLanguage.Dutch => System.Globalization.CultureInfo.GetCultureInfo("nl-NL"),
            SupportedLanguage.Russian => System.Globalization.CultureInfo.GetCultureInfo("ru-RU"),
            _ => System.Globalization.CultureInfo.GetCultureInfo("en-US")
        };
    }
}
