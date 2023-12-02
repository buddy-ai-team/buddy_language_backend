#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace BuddyLanguage.Domain.GptDataModels.Answers
{
    public class WordAnswer
    {
        public int WordsCount { get; set; }

        public Dictionary<string, string> StudiedWords { get; set; }
    }
}
