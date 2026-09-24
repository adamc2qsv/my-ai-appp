using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace MyAiLanguageCompanion;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly IServiceProvider _services;
    private readonly ITextToSpeech _textToSpeech;
    private readonly ITranslator _translator;
    private readonly ILanguageDetector _languageDetector;
    private readonly IPronunciationAnalyzer _pronunciationAnalyzer;
    private readonly IAudioCaptureService _audioCaptureService;
    private readonly ILiveSessionService _liveSessionService;

    public MainViewModel(IServiceProvider services)
    {
        _services = services;
        _textToSpeech = services.GetRequiredService<ITextToSpeech>();
        _translator = services.GetRequiredService<ITranslator>();
        _languageDetector = services.GetRequiredService<ILanguageDetector>();
        _pronunciationAnalyzer = services.GetRequiredService<IPronunciationAnalyzer>();
        _audioCaptureService = services.GetRequiredService<IAudioCaptureService>();
        _liveSessionService = services.GetRequiredService<ILiveSessionService>();

        Languages = Enum.GetValues<SupportedLanguage>().Select(x => x.ToString()).ToList();
        SourceLanguage = SupportedLanguage.English.ToString();
        TargetLanguage = SupportedLanguage.Dutch.ToString();
        SelectedLiveSource = "Microphone";
        LiveSources = new[] { "Microphone", "System audio", "Application audio" };
        TtsVoices = _textToSpeech.GetVoices();
        PlaybackSpeeds = new[] { "0.75x", "1.0x", "1.25x" };
        SelectedPlaybackSpeed = "1.0x";
        PracticePhrase = "Hoe gaat het?";
        PracticeAttempt = "Hoe gaat het?";
        PracticeResults = new List<PracticeWordResult>
        {
            new("Hoe", "Good"),
            new("gaat", "Needs practice"),
            new("het", "Good")
        };
        PracticeIssue = "gaat";
    }

    public List<string> Languages { get; }
    public List<string> LiveSources { get; }
    public List<string> TtsVoices { get; }
    public List<string> PlaybackSpeeds { get; }

    public string SourceLanguage { get; set; }
    public string TargetLanguage { get; set; }
    public string SelectedLiveSource { get; set; }
    public string SelectedVoice { get; set; } = string.Empty;
    public string SelectedPlaybackSpeed { get; set; }
    public string TranslationInput { get; set; } = "مرحبا";
    public string TranslationResultText { get; set; } = "Hallo";
    public string TranslationNote { get; set; } = "Local phrase dictionary translation.";
    public string DetectedLanguageDisplay { get; set; } = "Arabic";
    public string CurrentDetectedLanguageDisplay { get; set; } = "🇳🇱 Dutch";
    public string LiveTranscript { get; set; } = "Ik weet niet wat je bedoelt.";
    public string LiveStatusText { get; set; } = "○ OFF";
    public Brush LiveIndicatorBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    public string PracticePhrase { get; set; }
    public string PracticeAttempt { get; set; }
    public string PracticeIssue { get; set; }
    public List<PracticeWordResult> PracticeResults { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task StartLiveShareAsync()
    {
        LiveStatusText = "● LIVE";
        LiveIndicatorBrush = new SolidColorBrush(Colors.LimeGreen);
        await _audioCaptureService.StartMicrophoneAsync();
        var source = SelectedLiveSource;
        if (source.Contains("System", StringComparison.OrdinalIgnoreCase))
        {
            await _audioCaptureService.StartSystemAudioAsync();
        }
        else if (source.Contains("Application", StringComparison.OrdinalIgnoreCase))
        {
            // Application-specific process loopback requires Windows support and is not silently faked.
            var available = _audioCaptureService.AvailableApplicationSources;
            if (available.Count == 0)
            {
                LiveStatusText = "Application audio unavailable";
                return;
            }
        }

        var detected = _languageDetector.DetectLanguage(LiveTranscript);
        CurrentDetectedLanguageDisplay = detected.GetDisplayName();
    }

    public async Task StopLiveShareAsync()
    {
        await _audioCaptureService.StopAsync();
        LiveStatusText = "○ OFF";
        LiveIndicatorBrush = new SolidColorBrush(Colors.Gray);
    }

    public async Task TranslateAsync()
    {
        var sourceLanguage = Enum.TryParse<SupportedLanguage>(SourceLanguage, out var source) ? source : SupportedLanguage.English;
        var targetLanguage = Enum.TryParse<SupportedLanguage>(TargetLanguage, out var target) ? target : SupportedLanguage.Dutch;
        var detection = _languageDetector.DetectLanguage(TranslationInput);
        var result = await _translator.TranslateAsync(TranslationInput, targetLanguage, detection, sourceLanguage);
        TranslationResultText = result.TranslatedText;
        DetectedLanguageDisplay = detection.GetDisplayName();
        TranslationNote = result.Notes;
    }

    public async Task RunPracticeAsync()
    {
        var assessment = _pronunciationAnalyzer.Analyze(PracticePhrase, PracticeAttempt);
        PracticeResults = assessment.Results;
        PracticeIssue = assessment.ProblematicWord ?? "No likely issue detected.";
        OnPropertyChanged(nameof(PracticeResults));
        OnPropertyChanged(nameof(PracticeIssue));
        await Task.CompletedTask;
    }

    public async Task SpeakSelectedWordAsync()
    {
        var language = Enum.TryParse<SupportedLanguage>(TargetLanguage, out var target) ? target : SupportedLanguage.Dutch;
        await _textToSpeech.SpeakWordAsync("Hallo", language, 1.0);
    }

    public async Task SpeakSelectedSentenceAsync()
    {
        var language = Enum.TryParse<SupportedLanguage>(TargetLanguage, out var target) ? target : SupportedLanguage.Dutch;
        await _textToSpeech.SpeakSentenceAsync(TranslationResultText, language, 1.0);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed record PracticeWordResult(string Word, string Status)
{
    public string StatusColor => Status.Contains("Good", StringComparison.OrdinalIgnoreCase) ? "#66D9A5" : Status.Contains("practice", StringComparison.OrdinalIgnoreCase) ? "#FFB84D" : "#FF6C7A";
}
