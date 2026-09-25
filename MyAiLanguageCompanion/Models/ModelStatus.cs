namespace MyAiLanguageCompanion;

public enum ModelState
{
    Ready,
    Missing,
    Unavailable,
    Error
}

public sealed record ModelStatus(ModelState State, string DisplayName, string Detail)
{
    public bool IsReady => State == ModelState.Ready;
}

public sealed record TranslationModelStatus(ModelState State, string Detail)
{
    public bool IsReady => State == ModelState.Ready;
}
