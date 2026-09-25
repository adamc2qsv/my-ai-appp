using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MyAiLanguageCompanion;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly ITextToSpeech _tts;
    private readonly ITranslator _translator;
    private readonly ILanguageDetector _detector;
    private readonly IPronunciationAnalyzer _analyzer;
    private readonly IAudioCaptureService _capture;
    private string _translationInput = "hello";
    private string _translationResult = string.Empty;
    private string _practiceAttempt = string.Empty;
    private string _liveTranscript = string.Empty;
    private bool _isLive;

    public MainViewModel(IServiceProvider services)
    {
        _tts = services.GetRequiredService<ITextToSpeech>();
        _translator = services.GetRequiredService<ITranslator>();
        _detector = services.GetRequiredService<ILanguageDetector>();
        _analyzer = services.GetRequiredService<IPronunciationAnalyzer>();
        _capture = services.GetRequiredService<IAudioCaptureService>();
        Languages = new[] { "Auto", "English", "Spanish", "Dutch", "Russian" };
        LiveSources = new[] { "Microphone", "System audio" };
        PlaybackSpeeds = new[] { "0.75x", "1.0x", "1.25x" };
        SourceLanguage = "Auto";
        TargetLanguage = "Dutch";
        SelectedLiveSource = "Microphone";
        SelectedPlaybackSpeed = "1.0x";
        PracticePhrase = "Hoe gaat het?";
        PracticeResults = new();
        TtsVoices = _tts.GetVoices();
    }

    public IEnumerable<string> Languages { get; }
    public IEnumerable<string> LiveSources { get; }
    public IEnumerable<string> PlaybackSpeeds { get; }
    public IEnumerable<string> TtsVoices { get; }
    public string SourceLanguage { get => _sourceLanguage; set { _sourceLanguage = value; Changed(); } } private string _sourceLanguage = "Auto";
    public string TargetLanguage { get => _targetLanguage; set { _targetLanguage = value; Changed(); } } private string _targetLanguage = "Dutch";
    public string SelectedLiveSource { get => _selectedLiveSource; set { _selectedLiveSource = value; Changed(); } } private string _selectedLiveSource = "Microphone";
    public string SelectedVoice { get; set; } = string.Empty;
    public string SelectedPlaybackSpeed { get => _selectedPlaybackSpeed; set { _selectedPlaybackSpeed = value; Changed(); } } private string _selectedPlaybackSpeed = "1.0x";
    public string TranslationInput { get => _translationInput; set { _translationInput = value; Changed(); } }
    public string TranslationResultText { get => _translationResult; private set { _translationResult = value; Changed(); } }
    public string DetectedLanguageDisplay { get; private set; } = "Not detected";
    public string TranslationNote { get; private set; } = "No translation requested.";
    public string LiveTranscript { get => _liveTranscript; private set { _liveTranscript = value; Changed(); } }
    public string CurrentDetectedLanguageDisplay { get; private set; } = "Language uncertain";
    public string LiveStatusText { get; private set; } = "○ OFF";
    public Brush LiveIndicatorBrush { get; private set; } = Brushes.Gray;
    public string PracticePhrase { get; set; }
    public string PracticeAttempt { get => _practiceAttempt; set { _practiceAttempt = value; Changed(); } }
    public List<PracticeWordResult> PracticeResults { get; private set; }
    public string PracticeIssue { get; private set; } = "No recording analyzed.";
    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task StartLiveShareAsync()
    {
        if (_isLive) return;
        try
        {
            if (SelectedLiveSource == "Microphone") await _capture.StartMicrophoneAsync();
            else await _capture.StartSystemAudioAsync();
            _isLive = true;
            LiveStatusText = "● LIVE • " + SelectedLiveSource;
            LiveIndicatorBrush = Brushes.LimeGreen;
            LiveTranscript = "Capture active. Connect a local Whisper model to transcribe audio.";
            CurrentDetectedLanguageDisplay = "Language uncertain";
            Changed(nameof(LiveStatusText)); Changed(nameof(LiveIndicatorBrush)); Changed(nameof(CurrentDetectedLanguageDisplay));
        }
        catch (Exception ex) { LiveStatusText = "Capture unavailable: " + ex.Message; LiveIndicatorBrush = Brushes.OrangeRed; Changed(nameof(LiveStatusText)); Changed(nameof(LiveIndicatorBrush)); }
    }
    public async Task StopLiveShareAsync() { await _capture.StopAsync(); _isLive = false; LiveStatusText = "○ OFF"; LiveIndicatorBrush = Brushes.Gray; Changed(nameof(LiveStatusText)); Changed(nameof(LiveIndicatorBrush)); }
    public async Task TranslateAsync()
    {
        var target = Parse(TargetLanguage, SupportedLanguage.Dutch);
        var detected = _detector.DetectLanguage(TranslationInput);
        var manual = SourceLanguage == "Auto" ? (SupportedLanguage?)null : Parse(SourceLanguage, SupportedLanguage.English);
        var result = await _translator.TranslateAsync(TranslationInput, target, detected, manual);
        TranslationResultText = result.TranslatedText; DetectedLanguageDisplay = detected.GetDisplayName(); TranslationNote = result.Notes; Changed(nameof(DetectedLanguageDisplay)); Changed(nameof(TranslationNote));
    }
    public Task RunPracticeAsync() { var result = _analyzer.Analyze(PracticePhrase, PracticeAttempt); PracticeResults = result.Results; PracticeIssue = result.ProblematicWord is null ? "No likely word-level issue detected." : result.ProblematicWord; Changed(nameof(PracticeResults)); Changed(nameof(PracticeIssue)); return Task.CompletedTask; }
    public Task SpeakSelectedWordAsync() => _tts.SpeakWordAsync(TranslationResultText.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "", Parse(TargetLanguage, SupportedLanguage.Dutch), Speed);
    public Task SpeakSelectedSentenceAsync() => _tts.SpeakSentenceAsync(TranslationResultText, Parse(TargetLanguage, SupportedLanguage.Dutch), Speed);
    public Task SpeakPracticeAsync() => _tts.SpeakSentenceAsync(PracticePhrase, Parse(TargetLanguage, SupportedLanguage.Dutch), Speed);
    private double Speed => SelectedPlaybackSpeed switch { "0.75x" => .75, "1.25x" => 1.25, _ => 1.0 };
    private static SupportedLanguage Parse(string value, SupportedLanguage fallback) => Enum.TryParse(value, true, out SupportedLanguage parsed) ? parsed : fallback;
    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
}

public sealed record PracticeWordResult(string Word, string Status)
{
    public string StatusColor => Status.Equals("Good", StringComparison.OrdinalIgnoreCase) ? "#66D9A5" : "#FF6C7A";
}
