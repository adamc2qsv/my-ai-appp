namespace MyAiLanguageCompanion;

public interface IAudioCaptureService
{
    bool IsCapturing { get; }
    List<string> AvailableInputDevices { get; }
    List<string> AvailableOutputDevices { get; }
    List<string> AvailableApplicationSources { get; }

    Task RefreshAsync();
    Task StartMicrophoneAsync(CancellationToken cancellationToken = default);
    Task StartSystemAudioAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
