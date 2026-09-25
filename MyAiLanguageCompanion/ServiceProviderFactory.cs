using Microsoft.Extensions.DependencyInjection;

namespace MyAiLanguageCompanion;

public static class ServiceProviderFactory
{
    public static IServiceProvider Create()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILanguageDetector, OfflineLanguageDetector>();
        services.AddSingleton<ITextToSpeech, WindowsTextToSpeechService>();
        services.AddSingleton<ITranslator, PhraseDictionaryTranslator>();
        services.AddSingleton<IPronunciationAnalyzer, PronunciationAnalyzer>();
        services.AddSingleton<IAudioCaptureService, AudioCaptureService>();
        services.AddSingleton<ISpeechRecognizer, WindowsSpeechRecognizer>();
        services.AddSingleton<ILiveSessionService, LiveSessionService>();

        return services.BuildServiceProvider();
    }
}
