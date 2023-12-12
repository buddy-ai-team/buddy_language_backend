using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuddyLanguage.Domain.Exceptions.WordEntity;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services
{
    public class StatisticsService
    {
        private readonly IUnitOfWork _uow;

        public StatisticsService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public virtual async Task<int?> GetCountWordsLearned(Guid userId, CancellationToken cancellationToken)
        {
            return await _uow.WordEntityRepository.GetNumberWordsLearned(userId, cancellationToken);
        }

        public virtual async Task<int?> GetCountWordsLearning(Guid userId, CancellationToken cancellationToken)
        {
            return await _uow.WordEntityRepository.GetNumberWordsLearning(userId, cancellationToken);
        }

        public virtual async Task<int?> GetTotalCountMessages(string id, Guid topicId, CancellationToken cancellationToken)
        {
            return await _uow.MessageRepository.GetMessageCount(id, topicId, cancellationToken);
        }

        public virtual async Task<int?> GetCountDaysCommunication(string id, CancellationToken cancellationToken)
        {
            return await _uow.MessageRepository.GetNumbersDaysCommunication(id, cancellationToken);
        }
    }
}
