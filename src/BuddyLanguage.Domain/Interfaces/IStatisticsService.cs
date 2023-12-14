using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IStatisticsService<TStatisticsResponse>
    {
        Task<TStatisticsResponse> GetCountOfDaysAndMessages(string id, CancellationToken cancellationToken);
    }
}
