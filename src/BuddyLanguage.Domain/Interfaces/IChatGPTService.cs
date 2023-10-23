using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IChatGPTService
    {
        Task<string> GetAnswerOnTopic(string userMessage, Guid userId, CancellationToken cancellation);
        Task<string> GetAnswer(string userMessage, CancellationToken cancellationToken);
        Task<string> GetAnswer(string prompt, string userMessage, CancellationToken cancellationToken);
    }
}
