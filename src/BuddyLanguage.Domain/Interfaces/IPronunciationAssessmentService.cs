using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces;

public interface IPronunciationAssessmentService
{
    /// <summary>
    /// Получить оценку произношения
    /// </summary>
    /// <param name="audioDataPcm">Голосовое сообщение в виде набора бойт.
    /// Поддерживаемые форматы: PCM, 16 bit, sample rate 16000, mono
    /// </param>
    /// <param name="targetLanguage">язык пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Оценка произношения для каждого слова</returns>
    Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentFromRawPcmAsync(
        byte[] audioDataPcm,
        Language targetLanguage,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получить оценку произношения
    /// </summary>
    /// <param name="audioData">Голосовое сообщение в виде набора бойт.
    /// Поддерживаемые форматы: OGG Opus, sample rate 48000, mono
    /// </param>
    /// <param name="targetLanguage">язык пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Оценка произношения для каждого слова</returns>
    Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentFromOggAsync(
        byte[] audioData,
        Language targetLanguage,
        CancellationToken cancellationToken);
}
