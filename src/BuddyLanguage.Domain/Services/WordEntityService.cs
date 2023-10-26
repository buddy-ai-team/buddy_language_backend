using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.WordEntity;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services
{
    public class WordEntityService
    {
        private readonly ILogger<WordEntityService> _logger;
        private readonly IUnitOfWork _uow;

        public WordEntityService(IUnitOfWork uow, ILogger<WordEntityService> logger)
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

        public virtual Task<IReadOnlyList<WordEntity>> GetWordsByAccountId(Guid accountId, CancellationToken cancellationToken)
        {
            return _uow.WordEntityRepository.GetWordsByAccountId(accountId, cancellationToken);
        }

        public virtual async Task<WordEntity> UpdateWordEntityStatusById(Guid id, WordEntityStatus status, CancellationToken cancellationToken)
        {
            var word = await _uow.WordEntityRepository.GetById(id, cancellationToken);

            if (word is null)
            {
                throw new WordEntityNotFoundException("Word with given id not found");
            }

            word.WordStatus = status;

            await _uow.WordEntityRepository.Update(word, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return await _uow.WordEntityRepository.GetById(word.Id, cancellationToken);
        }

        public virtual async Task<WordEntity> AddWord(Guid accountId, string word, WordEntityStatus status, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new WordEntityNameUndefinedException("Name of word was undefined");

            var wordVar = new WordEntity(Guid.NewGuid(), accountId, word, status);

            await _uow.WordEntityRepository.Add(wordVar, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return await _uow.WordEntityRepository.GetById(wordVar.Id, cancellationToken);
        }
    }
}
