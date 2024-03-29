﻿using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IPromptService
    {
        string GetNameForDefaultRole();

        string GetPromptForDefaultRole();

        string GetPromptForGrammarMistakes(Language nativeLanguage);

        string GetPromptForLearningWords(Language nativeLanguage, Language targetLanguage);
    }
}
