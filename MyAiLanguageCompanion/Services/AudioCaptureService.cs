using System.Globalization;
using System.Speech.Recognition;

namespace MyAiLanguageCompanion;

public sealed class WindowsSpeechRecognizer : ISpeechRecognizer
{
    private SpeechRecognitionEngine? _engine;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_engine is not null)
        {
            return Task.CompletedTask;
        }

        _engine = new SpeechRecognitionEngine(new CultureInfo("en-US"));
        _engine.LoadGrammar(new DictationGrammar());
        _engine.SetInputToDefaultAudioDevice();
        _engine.RecognizeAsync(RecognizeMode.Multiple);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_engine is not null)
        {
            _engine.RecognizeAsyncStop();
            _engine.Dispose();
            _engine = null;
        }

        return Task.CompletedTask;
    }

    public Task<string> RecognizeOnceAsync(CancellationToken cancellationToken = default)
    {
        if (_engine is null)
        {
            return Task.FromResult(string.Empty);
        }

        var result = _engine.Recognize();
        return Task.FromResult(result?.Text ?? string.Empty);
    }
}
