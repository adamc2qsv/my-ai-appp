namespace MyAiLanguageCompanion;

public sealed class PronunciationAnalyzer : IPronunciationAnalyzer
{
    public PracticeAssessment Analyze(string targetText, string attemptedText)
    {
        var targetWords = SplitWords(targetText);
        var attemptedWords = SplitWords(attemptedText);
        var results = new List<PracticeWordResult>();

        for (var i = 0; i < Math.Max(targetWords.Count, attemptedWords.Count); i++)
        {
            var targetWord = i < targetWords.Count ? targetWords[i] : string.Empty;
            var attemptWord = i < attemptedWords.Count ? attemptedWords[i] : string.Empty;

            var status = EvaluateWord(targetWord, attemptWord);
            results.Add(new PracticeWordResult(targetWord, status));
        }

        var problem = results.FirstOrDefault(x => x.Status.Contains("Needs practice", StringComparison.OrdinalIgnoreCase) || x.Status.Contains("Unclear", StringComparison.OrdinalIgnoreCase));
        return new PracticeAssessment
        {
            Results = results,
            ProblematicWord = problem?.Word
        };
    }

    private static List<string> SplitWords(string text)
    {
        return text.Split(new[] { ' ', '\t', '\r', '\n', ',', '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private static string EvaluateWord(string targetWord, string attemptWord)
    {
        if (string.IsNullOrWhiteSpace(targetWord))
        {
            return "Good";
        }

        if (string.IsNullOrWhiteSpace(attemptWord))
        {
            return "Unclear";
        }

        if (string.Equals(targetWord, attemptWord, StringComparison.OrdinalIgnoreCase))
        {
            return "Good";
        }

        var distance = LevenshteinDistance(targetWord, attemptWord);
        return distance <= 2 ? "Needs practice" : "Unclear";
    }

    private static int LevenshteinDistance(string first, string second)
    {
        if (string.IsNullOrEmpty(first)) return second.Length;
        if (string.IsNullOrEmpty(second)) return first.Length;

        var previous = Enumerable.Range(0, second.Length + 1).ToArray();
        var current = new int[second.Length + 1];

        for (var i = 1; i <= first.Length; i++)
        {
            current[0] = i;
            for (var j = 1; j <= second.Length; j++)
            {
                var insert = current[j - 1] + 1;
                var delete = previous[j] + 1;
                var replace = previous[j - 1] + (first[i - 1] == second[j - 1] ? 0 : 1);
                current[j] = Math.Min(insert, Math.Min(delete, replace));
            }

            Array.Copy(current, previous, current.Length);
        }

        return previous[second.Length];
    }
}
