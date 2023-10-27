using BuddyLanguage.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.HttpModels.Responses.User
{
    public record UserResponse(Guid id, string firstName, string lastName, string telegramId);
}
