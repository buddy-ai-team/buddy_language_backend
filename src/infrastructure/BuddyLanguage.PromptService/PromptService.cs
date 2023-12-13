using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.PromptServices
{
    public class PromptService : IPromptService
    {
        public string GetNameForDefaultRole()
        {
            return "Friend";
        }

        public string GetPromptForDefaultRole()
        {
            return "Keep up a conversation with me as if you were my friend.";
        }

        public string GetPromptForGrammarMistakes(Language nativeLanguage)
        {
            return $"Я хочу чтобы ты выступил в роли корректора. " +
                   $"Найди очень точно и верно грамматические ошибки (максимум 3) в этом тексте " +
                   $"и сформулируй их подбробно на {nativeLanguage} языке, " +
                   $"а также улучшения и исправления текста." +
                   $"Не нужно воспринимать за грамматические ошибки слова, " +
                   $"написанные на {nativeLanguage} языке." +
                   $"Запиши их в поле \"GrammaMistakes\".";
        }

        public string GetPromptForLearningWords(Language nativeLanguage, Language targetLanguage)
        {
            return $"Я предоставлю тебе тексты, которые ты должен будешь проверить дважды " +
                   $"на наличие {nativeLanguage} слов." +
                   $"Посчитай количество {nativeLanguage} слов, а также найди " +
                   $"абсолютно все {nativeLanguage} слова из текста, если они есть." +
                   $"Затем запиши эти слова, а также " +
                   $"перевод этих слов на {targetLanguage} в поле \"StudiedWords\"." +
                   $"Пожалуйста, убедись, что ты нашел абсолютно все слова.";
        }
    }
}
