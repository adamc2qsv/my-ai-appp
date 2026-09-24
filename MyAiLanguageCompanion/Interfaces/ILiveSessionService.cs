namespace MyAiLanguageCompanion;

public interface IPronunciationAnalyzer
{
    PracticeAssessment Analyze(string targetText, string attemptedText);
}

public sealed class PracticeAssessment
{
    public List<PracticeWordResult> Results { get; init; } = new();
    public string? ProblematicWord { get; init; }
}
