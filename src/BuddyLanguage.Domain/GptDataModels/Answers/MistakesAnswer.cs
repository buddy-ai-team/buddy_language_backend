#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace BuddyLanguage.Domain.GptDataModels.Answers;

public class MistakesAnswer
{
    public int GrammaMistakesCount { get; set; }

    public string[] GrammarMistakes { get; set; }

    public override string ToString() => string.Join(",", GrammarMistakes);
}
