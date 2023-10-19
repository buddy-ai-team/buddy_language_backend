using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IChatGPTService
    {
        Task<string> GetAnswerFromChatGPT(string userMessage, Guid userId, CancellationToken cancellation);
    }
}
