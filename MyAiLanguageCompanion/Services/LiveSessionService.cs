using NAudio.Wave;

namespace MyAiLanguageCompanion;

public sealed class AudioCaptureService : IAudioCaptureService
{
    private WaveInEvent? _microphoneCapture;
    private WasapiLoopbackCapture? _loopbackCapture;

    public bool IsCapturing { get; private set; }
    public List<string> AvailableInputDevices { get; private set; } = new();
    public List<string> AvailableOutputDevices { get; private set; } = new();
    public List<string> AvailableApplicationSources { get; private set; } = new();

    public Task RefreshAsync()
    {
        AvailableInputDevices = Enumerable.Range(0, WaveIn.DeviceCount)
            .Select(i => WaveIn.GetCapabilities(i).ProductName)
            .ToList();

        AvailableOutputDevices = new List<string> { "System output" };
        AvailableApplicationSources = new List<string> { "Discord", "Roblox", "Chrome", "Game" };
        return Task.CompletedTask;
    }

    public Task StartMicrophoneAsync(CancellationToken cancellationToken = default)
    {
        _microphoneCapture = new WaveInEvent
        {
            WaveFormat = new WaveFormat(16000, 1)
        };

        _microphoneCapture.DataAvailable += (_, _) => { };
        _microphoneCapture.StartRecording();
        IsCapturing = true;
        return Task.CompletedTask;
    }

    public Task StartSystemAudioAsync(CancellationToken cancellationToken = default)
    {
        _loopbackCapture = new WasapiLoopbackCapture();
        _loopbackCapture.DataAvailable += (_, _) => { };
        _loopbackCapture.StartRecording();
        IsCapturing = true;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _microphoneCapture?.StopRecording();
        _microphoneCapture?.Dispose();
        _microphoneCapture = null;

        _loopbackCapture?.StopRecording();
        _loopbackCapture?.Dispose();
        _loopbackCapture = null;

        IsCapturing = false;
        return Task.CompletedTask;
    }
}
