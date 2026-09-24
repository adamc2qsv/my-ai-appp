# AI Language Companion

A local-first Windows desktop language companion built with .NET 8 and WPF.

## Features implemented

- Live share flow with microphone and system-audio capture abstraction
- Offline language detection for English, Spanish, Dutch, and Russian
- Phrase-based local translation for common sentence patterns
- Word and sentence playback via Windows TTS
- Pronunciation practice feedback using word-level similarity analysis
- Dark modern WPF interface with Live Share, Translate, Practice, and Settings tabs

## Important limitation

This repository is structured for a Windows desktop environment. The app uses Windows-specific APIs (`WPF`, `System.Speech`, and WASAPI-compatible capture patterns), so it must be built on Windows 10/11. Full live audio capture, process-specific loopback capture, and local speech recognition are best-effort features that depend on the machine's installed audio devices and OS support.

## Build (Windows 10/11)

```powershell
dotnet restore
# open in Visual Studio, or run:
dotnet build MyAiLanguageCompanion.sln
```

## Run

```powershell
dotnet run --project MyAiLanguageCompanion/MyAiLanguageCompanion.csproj
```

## Notes

- No paid API keys are required.
- Model downloads are not forced at startup.
- Raw live audio is not persisted by default.
