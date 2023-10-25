using BuddyLanguage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IWordEntityRepository : IRepository<WordEntity>
    {
        Task<IReadOnlyList<WordEntity>> GetWordsByAccountId(Guid accountId, CancellationToken cancellationToken);
    }
}
