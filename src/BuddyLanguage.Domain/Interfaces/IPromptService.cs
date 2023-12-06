using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IPromptService
    {
        string GetPromptForGrammarMistakes(Language nativeLanguage);

        string GetPromptForLearningWords(Language nativeLanguage, Language targetLanguage);
    }
}
