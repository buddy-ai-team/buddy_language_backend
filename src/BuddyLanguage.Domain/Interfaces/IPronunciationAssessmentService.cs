using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces;

public interface IPronunciationAssessmentService
{
    /// <summary>
    /// Получить оценку произношения
    /// </summary>
    /// <param name="audioData">Голосовое сообщение в виде набора бойт.
    /// Поддерживаемые форматы: PCM, 16 bit, sample rate 16000, mono
    /// </param>
    /// <param name="targetLanguage">язык пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Оценка произношения для каждого слова</returns>
    Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentAsync(
        byte[] audioData,
        Language targetLanguage,
        CancellationToken cancellationToken);
}
