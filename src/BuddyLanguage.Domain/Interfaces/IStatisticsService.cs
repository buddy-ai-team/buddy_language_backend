using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuddyLanguage.ExternalStatisticsServiceLib;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IStatisticsService
    {
        Task<Statistics> GetCountOfDaysAndMessages(string id, CancellationToken cancellationToken);
    }
}
