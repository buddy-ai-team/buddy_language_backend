using BuddyLanguage.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.HttpModels.Responses.WordEntity
{
    public record WordEntityResponse(Guid id, Guid accountId, string word, WordEntityStatus status);
}
