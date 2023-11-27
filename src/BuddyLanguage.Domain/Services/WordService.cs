using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.WordEntity;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services
{
    public class WordService : IWordService
    {
        private readonly ILogger<WordService> _logger;
        private readonly IUnitOfWork _uow;

        public WordService(IUnitOfWork uow, ILogger<WordService> logger)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<WordEntity> GetWordById(Guid id, CancellationToken cancellationToken)
        {
            var word = await _uow.WordEntityRepository.GetById(id, cancellationToken);

            if (word is null)
            {
                throw new WordEntityNotFoundException("Word with given id not found");
            }

            return word;
        }

        public virtual Task<IReadOnlyList<WordEntity>> GetWordsByAccountId(Guid userId, CancellationToken cancellationToken)
        {
            return _uow.WordEntityRepository.GetWordsByUserId(userId, cancellationToken);
        }

        public virtual async Task<WordEntity> UpdateWordEntityById(
            Guid id,
            string word,
            string? translation,
            Language language,
            WordEntityStatus status,
            CancellationToken cancellationToken)
        {
            var wordVar = await _uow.WordEntityRepository.GetById(id, cancellationToken);

            if (wordVar is null)
            {
                throw new WordEntityNotFoundException("Word with given id not found");
            }

            wordVar.Word = word;
            wordVar.Translation = translation;
            wordVar.Language = language;
            wordVar.WordStatus = status;

            await _uow.WordEntityRepository.Update(wordVar, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return await _uow.WordEntityRepository.GetById(wordVar.Id, cancellationToken);
        }

        public virtual async Task<WordEntity> AddWord(
            Guid accountId,
            string word,
            string? translation,
            Language language,
            WordEntityStatus status,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(word, nameof(word));

            var wordVar = new WordEntity(Guid.NewGuid(), accountId, word, language, status, translation);

            await _uow.WordEntityRepository.Add(wordVar, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return wordVar;
        }
    }
}
