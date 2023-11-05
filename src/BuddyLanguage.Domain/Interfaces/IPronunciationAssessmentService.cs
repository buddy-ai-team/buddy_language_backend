using BuddyLanguage.Domain.Entities;

namespace BuddyLanguage.Domain.Interfaces;

public interface IPronunciationAssessmentService
{
    /// <summary>
    /// Получить оценку произношения
    /// </summary>
    /// <param name="voiceMessage">Голосовое сообщение в виде набора бойт.
    /// Поддерживаемые форматы: ogg, wav
    /// </param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Оценка произношения для каждого слова</returns>
    Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentAsync(
        byte[] voiceMessage, CancellationToken cancellationToken);
}
