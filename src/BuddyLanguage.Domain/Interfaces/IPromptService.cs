using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IPromptService
    {
        string GetNameForDefaultRole();

        string GetPromptForDefaultRole();

        string GetPromptToTranslateTextIntoNativeLanguage(Language nativeLanguage);

        string GetPromptForGrammarMistakes(Language nativeLanguage);

        string GetPromptForLearningWords(Language nativeLanguage, Language targetLanguage);
    }
}
