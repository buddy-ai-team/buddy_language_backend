using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<int> GetMessageCount(string id, Guid topicId, CancellationToken cancellationToken);

        Task<int> GetNumbersDaysCommunication(string id, CancellationToken cancellationToken);
    }
}
